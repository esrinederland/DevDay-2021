using ArcGIS.Core.Hosting;
using System;
using System.Collections.Generic;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;

namespace CoreHost
{
	class Program
	{
		//[STAThread] must be present on the Application entry point
		[STAThread]
		static void Main(string[] args)
		{
			//Call Host.Initialize before constructing any objects from ArcGIS.Core
			Host.Initialize();

			Console.WriteLine("CoreHost - Initilized");

			try
			{
				// Opens a file geodatabase. This will open the geodatabase if the folder exists and contains a valid geodatabase.
				using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(@"D:\Projecten\GisTech\Git\Pro SDK\CoreHost\Nederland.gdb"))))
				{
					Console.WriteLine("CoreHost - File Geodatebase found");

					FeatureClass countryFeatureClass = geodatabase.OpenDataset<FeatureClass>("Landsgrens");
					FeatureClass slicesFeatureClass = geodatabase.OpenDataset<FeatureClass>("Slices");

					FeatureClassDefinition countryFeatureClassDefinition = countryFeatureClass.GetDefinition();
					FeatureClassDefinition slicesFeatureClassDefinition = slicesFeatureClass.GetDefinition();

					Console.WriteLine("CoreHost - FeatureClasses found");

					// Create new slices based on the poligon of the Netherlands
					IReadOnlyList<Polygon> slices = null;
					using (RowCursor curs = countryFeatureClass.Search())
					{
						Console.WriteLine("CoreHost - GeometryEngine slicing features");

						while (curs.MoveNext())
						{
							if (curs.Current != null)
							{
								using (Feature feature = (Feature)curs.Current)
								{
									var polygon = curs.Current[countryFeatureClassDefinition.GetShapeField()] as Polygon;
									slices = GeometryEngine.Instance.SlicePolygonIntoEqualParts(polygon, 20, new Random().Next(0, 360), SliceType.Blocks);
								}
							}
						}
					}

					Console.WriteLine("CoreHost - Deleting old slices"); 
					// Delete all existing items. 
					geodatabase.ApplyEdits(() =>
					{
						slicesFeatureClass.DeleteRows(new QueryFilter() { WhereClause = "42 = 42" });
					});

					Console.WriteLine("CoreHost - Saving features");

					// Store new slices.
					geodatabase.ApplyEdits(() =>
					{
						if (slices != null)
						{
							for (int i = 0; i < slices.Count; i++)
							{
								using (RowBuffer rowBuffer = slicesFeatureClass.CreateRowBuffer())
								{
									// Either the field index or the field name can be used in the indexer.
									rowBuffer["Code"] = i.ToString();
									rowBuffer["Landsnaam"] = "Nederland";
									rowBuffer[slicesFeatureClassDefinition.GetShapeField()] = slices[i];

									using (Feature feature = slicesFeatureClass.CreateRow(rowBuffer))
									{
										feature.Store();
									}
								}
							}
						}
					});

					Console.WriteLine("CoreHost - Data saved");
				}

			}
			catch (GeodatabaseNotFoundOrOpenedException)
			{
				Console.WriteLine("File GeoDataBase niet gevonden");
			}
			catch (GeodatabaseEditingException)
			{
				Console.WriteLine("Kan niet schrijven naar de File GeoDataBase");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout {ex}");
			}

			Console.WriteLine("Press any key to continue...");
			Console.ReadLine();
		}
	}
}
