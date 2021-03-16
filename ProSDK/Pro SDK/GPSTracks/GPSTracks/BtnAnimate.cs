using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPSTracks
{
    internal class BtnAnimate : Button
    {

        #region Constants
        private const double ANIMATION_APPEND_TIME = 3; //seconds
        #endregion

        #region Fields/Properties
        private static readonly double _keyframeHeading = 0;
        private static readonly double _cameraZOffset = 5000;
        private static readonly double _totalDuration = 60;
        #endregion

        protected override async void OnClick()
        {
            FeatureLayer ftrLayer = (MapView.Active.Map.Layers.First(layer => layer.Name.Equals("MyRide")) as FeatureLayer);
            ProjectionTransformation transformation = await QueuedTask.Run(() => ProjectionTransformation.Create(ftrLayer.GetSpatialReference(), MapView.Active.Map.SpatialReference));
            SpatialReference layerSpatRef = await QueuedTask.Run(() => ftrLayer.GetSpatialReference());

            Polyline lineGeom = await GetPolyFineFromLayer(ftrLayer);
            //couldn't get the selected feature
            if (lineGeom == null)
            {
                return;
            }

            var animation = MapView.Active.Map.Animation;
            var cameraTrack = animation.Tracks.OfType<CameraTrack>().First();
            var keyframes = cameraTrack.Keyframes;

            //Get segment list for line
            ReadOnlyPartCollection polylineParts = lineGeom.Parts;

            //get total segment count and determine path length
            double pathLength = 0;
            int segmentCount = 0;
            IEnumerator<ReadOnlySegmentCollection> segments = polylineParts.GetEnumerator();
            while (segments.MoveNext())
            {
                ReadOnlySegmentCollection seg = segments.Current;
                foreach (Segment s in seg)
                {
                    double length3D = Math.Sqrt((s.EndPoint.X - s.StartPoint.X) * (s.EndPoint.X - s.StartPoint.X) +
                                                (s.EndPoint.Y - s.StartPoint.Y) * (s.EndPoint.Y - s.StartPoint.Y) +
                                                (s.EndPoint.Z - s.StartPoint.Z) * (s.EndPoint.Z - s.StartPoint.Z));
                    pathLength += length3D;
                    segmentCount += 1;
                }
            }

            await CreateKeyframes_AtVertices(MapView.Active, layerSpatRef, transformation, cameraTrack, segments, segmentCount, pathLength);
        }

        private static async Task<Polyline> GetPolyFineFromLayer(FeatureLayer ftrLayer)
        {
            return await QueuedTask.Run(() =>
            {
                RowCursor result = ftrLayer.Search();
                if (result != null && result.MoveNext())
                {
                    Polyline plyLine = null;
                    using (Feature selectedFtr = result.Current as Feature)
                    {
                        plyLine = selectedFtr.GetShape().Clone() as Polyline;
                    }
                    return plyLine;
                }
                return null;
            });
        }

        private static async Task CreateCameraKeyframe(MapPoint orig_cameraPoint, ProjectionTransformation transformation, CameraTrack cameraTrack, TimeSpan currentTimespanValue)
        {
            await QueuedTask.Run(() =>
            {
                Keyframe keyFrame = null;
                MapPoint projected_cameraPoint = (MapPoint)GeometryEngine.Instance.ProjectEx(orig_cameraPoint, transformation);

                var camera = new Camera(projected_cameraPoint.X, projected_cameraPoint.Y, _cameraZOffset, _keyframeHeading, null, CameraViewpoint.LookAt);
                keyFrame = cameraTrack.CreateKeyframe(camera, currentTimespanValue, AnimationTransition.FixedArc, .5);
            });
        }


        //Use this method if you want keyframes ONLY at line vertices. This is good if the line is highly densified.
        //However, you will get sharp turns at corners because there is no attempt to smooth the animation
        public static async Task CreateKeyframes_AtVertices(MapView mapView, SpatialReference layerSpatRef, ProjectionTransformation transformation,
                                                            CameraTrack cameraTrack, IEnumerator<ReadOnlySegmentCollection> segments,
                                                            int segmentCount, double pathLength)
        {
            double segmentLength = 0;
            int num_iterations = 0;
            segments.Reset();

            //process each segment depending upon its type - straight line or arc
            while (segments.MoveNext())
            {
                ReadOnlySegmentCollection seg = segments.Current;
                double accumulatedDuration = mapView.Map.Animation.Duration.TotalSeconds + ((mapView.Map.Animation.Duration.TotalSeconds > 0) ? ANIMATION_APPEND_TIME : 0);

                foreach (Segment s in seg)
                {
                    segmentLength = Math.Sqrt((s.EndPoint.X - s.StartPoint.X) * (s.EndPoint.X - s.StartPoint.X) +
                                              (s.EndPoint.Y - s.StartPoint.Y) * (s.EndPoint.Y - s.StartPoint.Y) +
                                              (s.EndPoint.Z - s.StartPoint.Z) * (s.EndPoint.Z - s.StartPoint.Z));

                    double segmentDuration = (_totalDuration / pathLength) * segmentLength;

                    MapPoint startPt = await QueuedTask.Run(() => MapPointBuilder.CreateMapPoint(s.StartPoint.X, s.StartPoint.Y, s.StartPoint.Z, layerSpatRef));
                    MapPoint endPt = await QueuedTask.Run(() => MapPointBuilder.CreateMapPoint(s.EndPoint.X, s.EndPoint.Y, s.EndPoint.Z, layerSpatRef));

                    //create keyframe at start vertex of path in map space
                    double timeSpanValue = accumulatedDuration;
                    TimeSpan keyframeTimespan = TimeSpan.FromSeconds(timeSpanValue);
                    await CreateCameraKeyframe(startPt, transformation, cameraTrack, keyframeTimespan);

                    //Create a keyframe at end point of segment only for the end point of last segment
                    //Otherwise we will get duplicate keyframes at end of one segment and start of the next one
                    if (num_iterations == segmentCount - 1)
                    {
                        timeSpanValue = accumulatedDuration + segmentDuration;
                        keyframeTimespan = TimeSpan.FromSeconds(timeSpanValue);

                        await CreateCameraKeyframe(endPt, transformation, cameraTrack, keyframeTimespan);
                    }

                    accumulatedDuration += segmentDuration;
                    num_iterations++;
                }
            }
        }
    }
}
