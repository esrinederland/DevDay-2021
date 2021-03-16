using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProUnitTests
{
    [TestClass]
    public class BufferTests
    {
        private static Map _map;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            QueuedTask.Run(async () =>
            {
                Project project = await Project.OpenAsync(@"E:\GISTech\2021\ProProjects\UnitTest\UnitTest\UnitTest.aprx");
                _map = project.GetItems<MapProjectItem>().First().GetMap();

            }).Wait();
        }

        [TestMethod]
        public async Task BufferPointLayerTest()
        {
            IGPResult gpResult = null;

            await QueuedTask.Run(async () =>
            {
                FeatureLayer pointLayer = _map.Layers.First(layer => layer.Name.Equals("Points")) as FeatureLayer;
                string FLPath = pointLayer.GetFeatureClass().GetDatastore().GetPath().AbsolutePath;
                var FLPathCombine = Path.GetFullPath(FLPath);
                string name = pointLayer.GetFeatureClass().GetName();
                string infc = Path.Combine(FLPathCombine, name);
                string outfc = Path.Combine(FLPathCombine, "Buffer_" + pointLayer.Name);
                // Place parameters into an array
                var parameters = Geoprocessing.MakeValueArray(infc, outfc, "100 Meter");
                // Place environment settings in an array, in this case, OK to over-write
                var environments = Geoprocessing.MakeEnvironmentArray(overwriteoutput: true);
                // Execute the GP tool with parameters
                gpResult = await Geoprocessing.ExecuteToolAsync("Buffer_analysis", parameters, environments);
            });

            Assert.IsNotNull(gpResult, "GPResult is null");
            Assert.AreEqual(0, gpResult.ErrorCode, "GP Tool failed or cancelled");
        }

        [TestMethod]
        public async Task IntersectTest()
        {
            IGPResult gpResult = null;
            Geometry lijnShape = null;
            Geometry buffer50Shape = null;
            Geometry buffer150Shape = null;

            FeatureLayer lineLayer = _map.Layers.First(layer => layer.Name.Equals("Lines")) as FeatureLayer;

            await QueuedTask.Run(async () =>
            {
                FeatureLayer pointLayer = _map.Layers.First(layer => layer.Name.Equals("Points")) as FeatureLayer;
                string FLPath = pointLayer.GetFeatureClass().GetDatastore().GetPath().AbsolutePath;
                var FLPathCombine = Path.GetFullPath(FLPath);
                string name = pointLayer.GetFeatureClass().GetName();
                string infc = Path.Combine(FLPathCombine, name);
                string outfc = Path.Combine(FLPathCombine, $"Buffer_{pointLayer.Name}_50");
                // Place parameters into an array
                var parameters = Geoprocessing.MakeValueArray(infc, outfc, "50 Meter");
                // Place environment settings in an array, in this case, OK to over-write
                var environments = Geoprocessing.MakeEnvironmentArray(overwriteoutput: true);
                // Execute the GP tool with parameters
                gpResult = await Geoprocessing.ExecuteToolAsync("Buffer_analysis", parameters, environments);

                var gdb = pointLayer.GetTable().GetDatastore() as Geodatabase;
                var fclBuffer50 = gdb.OpenDataset<FeatureClass>("Buffer_Points_50");

                outfc = Path.Combine(FLPathCombine, $"Buffer_{pointLayer.Name}_150");
                parameters = Geoprocessing.MakeValueArray(infc, outfc, "150 Meter");
                gpResult = await Geoprocessing.ExecuteToolAsync("Buffer_analysis", parameters, environments);

                var fclBuffer150 = gdb.OpenDataset<FeatureClass>("Buffer_Points_150");

                var temp = lineLayer.Search();
                temp.MoveNext();
                lijnShape = (temp.Current as Feature).GetShape();

                temp = fclBuffer50.Search();
                temp.MoveNext();
                buffer50Shape = (temp.Current as Feature).GetShape();

                temp = fclBuffer150.Search();
                temp.MoveNext();
                buffer150Shape = (temp.Current as Feature).GetShape();
            });

            Assert.IsNotNull(gpResult, "GPResult is null");
            Assert.AreEqual(0, gpResult.ErrorCode, "GP Tool failed or cancelled");

            Assert.IsTrue(GeometryEngine.Instance.Intersects(lijnShape, buffer150Shape), "De punten en lijnen intersecten niet op 150 meter");
            Assert.IsTrue(GeometryEngine.Instance.Intersects(lijnShape, buffer50Shape), "De punten en lijnen intersecten niet op 50 meter");            
        }
    }
}
