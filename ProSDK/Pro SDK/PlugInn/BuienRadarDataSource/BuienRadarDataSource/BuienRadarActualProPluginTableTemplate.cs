using ArcGIS.Core.Data;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Geometry;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BuienRadarDataSource
{
	public class BuienRadarActualProPluginTableTemplate : PluginTableTemplate
	{
		private BuienRadarActualProPluginCursorTemplate ActualWeather { get; set; } = null;

		public BuienRadarActualProPluginTableTemplate(string data)
		{
			// Convert the json data
			BuienRadarData jsonData = JsonConvert.DeserializeObject<BuienRadarData>(data);

			// Filter 0 values (some stations return false data)
			IEnumerator<Stationmeasurement> enumeralble = jsonData.Actual.Stationmeasurements.Where(item => item.Humidity != 0).ToList().GetEnumerator();

			// Create a cursor. 
			ActualWeather = new BuienRadarActualProPluginCursorTemplate(enumeralble);
		}

		/// <summary>
		/// Create the fields provided with the datasource, this can be static as in this example or dynamic from the datasource
		/// </summary>
		/// <returns></returns>
		public override IReadOnlyList<PluginField> GetFields()
		{
			// Create the list of PluginFields for this currently opened
			var pluginFields = new List<PluginField>();

			pluginFields.Add(new PluginField() { Name = "OID", AliasName = "OID", FieldType = FieldType.OID });
			pluginFields.Add(new PluginField() { Name = "StationName", AliasName = "Naam", FieldType = FieldType.String });
			pluginFields.Add(new PluginField() { Name = "Temperature", AliasName = "Temperatuur", FieldType = FieldType.Double });
			pluginFields.Add(new PluginField() { Name = "Time", AliasName = "Tijd", FieldType = FieldType.Double });
			pluginFields.Add(new PluginField() { Name = "Description", AliasName = "Weertype", FieldType = FieldType.Double });
			pluginFields.Add(new PluginField() { Name = "Iconurl", AliasName = "Icoon", FieldType = FieldType.Double });
			pluginFields.Add(new PluginField() { Name = "Humidity", AliasName = "Vochtigheid", FieldType = FieldType.Double });
			pluginFields.Add(new PluginField() { Name = "Sunpower", AliasName = "Zonnekracht", FieldType = FieldType.Double });
			pluginFields.Add(new PluginField() { Name = "Airpressure", AliasName = "Luchtdruk", FieldType = FieldType.Double });
			pluginFields.Add(new PluginField() { Name = "Visibility", AliasName = "Zicht", FieldType = FieldType.Double });
			pluginFields.Add(new PluginField() { Name = "GEOMETRY", AliasName = "GEOMETRY", FieldType = FieldType.Geometry });

			return pluginFields;
		}

		/// <summary>
		/// Return the name of the datasource
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Actuele weersituatie";
		}

		/// <summary>
		/// For this demo we return all items without a query
		/// </summary>
		/// <param name="queryFilter"></param>
		/// <returns></returns>
		public override PluginCursorTemplate Search(QueryFilter queryFilter)
		{
			ActualWeather.Reset();

			return ActualWeather;
		}

		/// <summary>
		/// For this item we return all items without a spatial query
		/// </summary>
		/// <param name="spatialQueryFilter"></param>
		/// <returns></returns>
		public override PluginCursorTemplate Search(SpatialQueryFilter spatialQueryFilter)
		{
			ActualWeather.Reset();

			return ActualWeather;
		}

		/// <summary>
		/// Specify the GeometryType, in this case we support points. 
		/// </summary>
		/// <returns></returns>
		public override GeometryType GetShapeType()
		{
			return GeometryType.Point;
		}
	}
}