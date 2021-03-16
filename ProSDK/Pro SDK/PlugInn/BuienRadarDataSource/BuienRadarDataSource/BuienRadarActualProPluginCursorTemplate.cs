using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Geometry;
using System.Collections.Generic;

namespace BuienRadarDataSource
{
	public class BuienRadarActualProPluginCursorTemplate : PluginCursorTemplate
	{
		IEnumerator<Stationmeasurement> StationMeasurements { get; set; } = null;

		public BuienRadarActualProPluginCursorTemplate(IEnumerator<Stationmeasurement> enumerator)
		{
			StationMeasurements = enumerator;
		}

		/// <summary>
		/// Get the current row, convert the data to a plugin row
		/// Project the data in the right projection. 
		/// </summary>
		/// <returns></returns>
		public override PluginRow GetCurrentRow()
		{
			if (StationMeasurements != null && StationMeasurements.Current != null)
			{
				// Create a point
				MapPointBuilder location = new MapPointBuilder(StationMeasurements.Current.Lon, StationMeasurements.Current.Lat, new SpatialReferenceBuilder(4326).ToSpatialReference());

				// Convert point to rd point. 
				Geometry locationRD = GeometryEngine.Instance.Project(location.ToGeometry(), SpatialReferenceBuilder.CreateSpatialReference(28992));

				// Convert attribute data 
				var listOfRowValues = new List<object>();
				listOfRowValues.Add(StationMeasurements.Current.Stationid);
				listOfRowValues.Add(StationMeasurements.Current.Stationname);
				listOfRowValues.Add(StationMeasurements.Current.Temperature);
				listOfRowValues.Add(StationMeasurements.Current.Timestamp);
				listOfRowValues.Add(StationMeasurements.Current.Weatherdescription);
				listOfRowValues.Add(StationMeasurements.Current.Iconurl);
				listOfRowValues.Add(StationMeasurements.Current.Humidity);
				listOfRowValues.Add(StationMeasurements.Current.Sunpower);
				listOfRowValues.Add(StationMeasurements.Current.Airpressure);
				listOfRowValues.Add(StationMeasurements.Current.Visibility);
				
				// Add the geometry
				listOfRowValues.Add(locationRD);

				return new PluginRow(listOfRowValues);
			}

			return new PluginRow();
		}

		/// <summary>
		/// Reset the cursor to the first item. 
		/// </summary>
		public void Reset()
		{
			StationMeasurements.Reset();
		}

		/// <summary>
		/// Move to the next item. 
		/// </summary>
		/// <returns></returns>
		public override bool MoveNext()
		{
			return StationMeasurements.MoveNext();
		}
	}
}