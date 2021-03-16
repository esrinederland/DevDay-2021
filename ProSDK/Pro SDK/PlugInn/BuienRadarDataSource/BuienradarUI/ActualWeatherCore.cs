using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Desktop.Mapping;
using BuienRadarSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace BuienradarUI
{
    public static class ActualWeatherCore
    {
		#region public methods
		/// <summary>
		/// Load the data from the datasource and create a featurelayer with custom dictionairy renderer.
		/// </summary>
		public static void ShowActualWeather()
		{
			PluginDatasourceConnectionPath pluginDataSource = new PluginDatasourceConnectionPath("BuienRadarDataSource_Datasource", new Uri(@"https://data.buienradar.nl/2.0/feed/json"));

			// Load the data and create a futurelayer
			using (PluginDatastore pluginDataStore = new PluginDatastore(pluginDataSource))
			{
				foreach (string tableName in pluginDataStore.GetTableNames())
				{
					using (Table table = pluginDataStore.OpenTable(tableName))
					{
						if (table is FeatureClass weatherFeatureClass)
						{
							//Add as a layer to the active map or scene.
							FeatureLayer weatherLayer = LayerFactory.Instance.CreateFeatureLayer(weatherFeatureClass, MapView.Active.Map);

							// Create and add the weatherlabels.
							SetLabeling(weatherLayer);

							// Show the layer
							weatherLayer.SetLabelVisibility(true);

							// Create a renderer based on the weather data.
							CIMUniqueValueRenderer weatherRenderer = CreateRedenderer(table);

							// Set the renderer on the featurelayer
							weatherLayer.SetRenderer(weatherRenderer);
						}
					}
				}
			}
		}

		/// <summary>
		/// Create a label for the weather feature layer. 
		/// </summary>
		/// <param name="weatherLayer"></param>
		public static void SetLabeling(FeatureLayer weatherLayer)
		{
			// Get the CIMFeatureLayer definition. 
			CIMFeatureLayer weatherLayerDefinition = weatherLayer.GetDefinition() as CIMFeatureLayer;

			// Get the label classes - we need the first one
			CIMLabelClass weatherLabelClass = weatherLayerDefinition.LabelClasses.FirstOrDefault();

			if (weatherLabelClass != null)
			{
				// Create a new Arcade label expression.
				if (DamlSettings.Default.Celius)
				{
					weatherLabelClass.Expression = "$feature.Temperature + \" c \\r\" +$feature.Humidity + \" % \"";
				}
				else
				{
					weatherLabelClass.Expression = "(($feature.Temperature * 1.8) + 32)+ \" F \\r\" +$feature.Humidity + \" % \"";
				}

				// Create a polygon symbol for the halo
				CIMPolygonSymbol textHalo = SymbolFactory.Instance.ConstructPolygonSymbol(ColorFactory.Instance.WhiteRGB, SimpleFillStyle.Solid);

				// Create text symbol using the halo polygon
				CIMTextSymbol weatherTextSymbol = SymbolFactory.Instance.ConstructTextSymbol(ColorFactory.Instance.BlackRGB, 14, "Corbel", "Regular");
				weatherTextSymbol.HaloSymbol = textHalo;
				weatherTextSymbol.HaloSize = 1;
				weatherTextSymbol.HaloSymbol.SetOutlineColor(ColorFactory.Instance.WhiteRGB);

				// Set the text symbol
				weatherLabelClass.TextSymbol.Symbol = weatherTextSymbol;

				// Update the layer definition
				weatherLayer.SetDefinition(weatherLayerDefinition);
			}
		}
        #endregion

        #region private methods
        /// <summary>
        /// Create a unique value renderer based on the weather 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static CIMUniqueValueRenderer CreateRedenderer(Table table)
		{
			List<string> uniqueIconUrls = new List<string>();
			using (RowCursor cursor = table.Search())
			{
				do
				{
					if (cursor.Current != null)
					{
						string url = cursor.Current["Iconurl"].ToString();
						if (!uniqueIconUrls.Contains(url))
						{
							uniqueIconUrls.Add(url);
						}
					}
				}
				while (cursor.MoveNext());
			}

			CIMUniqueValueGroup uniqueValueGroup = new CIMUniqueValueGroup
			{
				Heading = "Weer type",
				Classes = CreateUniqueValueClasses(uniqueIconUrls)
			};

			CIMUniqueValueRenderer uniqueValueRenderer = new CIMUniqueValueRenderer
			{
				Groups = new CIMUniqueValueGroup[] { uniqueValueGroup },
				Fields = new string[] { "Iconurl" }
			};
			return uniqueValueRenderer;
		}

		/// <summary>
		/// Create unique valueClasses based on the weather url's
		/// </summary>
		/// <param name="imagevalues"></param>
		/// <returns></returns>
		private static CIMUniqueValueClass[] CreateUniqueValueClasses(List<string> imagevalues)
		{
			List<CIMUniqueValueClass> cimclasses = new List<CIMUniqueValueClass>();
			foreach (string imageUrl in imagevalues)
			{
				CIMPictureMarker marker = new CIMPictureMarker()
				{
					Size = 30,
					URL = BuildPictureMarkerURL(new Uri(imageUrl))
				};

				CIMUniqueValue uniqueValue = new CIMUniqueValue
				{
					FieldValues = new string[] { imageUrl }
				};

				CIMUniqueValueClass uniqueValueClass = new CIMUniqueValueClass
				{
					Symbol = SymbolFactory.Instance.ConstructPointSymbol(marker).MakeSymbolReference(),
					Values = new CIMUniqueValue[] { uniqueValue }
				};

				cimclasses.Add(uniqueValueClass);
			}

			return cimclasses.ToArray();
		}

		/// <summary>
		/// Get a picture marker from a url.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		private static string BuildPictureMarkerURL(Uri uri)
		{
			PngBitmapDecoder w = new PngBitmapDecoder(uri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
			BitmapDecoder decoder = BitmapDecoder.Create(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
			StringBuilder builder = new StringBuilder();
			builder.Append("data:");
			builder.Append(decoder.CodecInfo.MimeTypes);
			builder.Append(";base64,");
			builder.Append(ToBase64(decoder));
			return builder.ToString();
		}

		private static string ToBase64(BitmapDecoder decoder)
		{
			BitmapEncoder encoder = BitmapEncoder.Create(decoder.CodecInfo.ContainerFormat);
			foreach (BitmapFrame frame in decoder.Frames)
			{
				encoder.Frames.Add(frame);
			}
			using (MemoryStream stream = new MemoryStream())
			{
				encoder.Save(stream);
				return Convert.ToBase64String(stream.ToArray(), Base64FormattingOptions.InsertLineBreaks);
			}
		}
        #endregion
    }
}
