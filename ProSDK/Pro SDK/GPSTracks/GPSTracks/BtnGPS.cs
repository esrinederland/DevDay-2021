using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using System;
using System.IO;

namespace GPSTracks
{
    internal class BtnGPS : Button
    {
        protected override async void OnClick()
        {
            string databasePath = @"E:\GISTech\2021\ProProjects\GPSTracks\GPSTracks.gdb";
            string gpxFile = @"E:\GISTech\2021\ProScripts\clubrit.gpx";
            string outfc = Path.Combine(databasePath, "GPSTracks");

            await QueuedTask.Run(async () =>
            {
                try
                {        
                    // Place parameters into an array
                    var parameters = Geoprocessing.MakeValueArray(gpxFile, outfc);
                    // Place environment settings in an array, in this case, OK to over-write
                    var environments = Geoprocessing.MakeEnvironmentArray(overwriteoutput: true);
                    // Execute the GP tool with parameters
                    var gpResult = await Geoprocessing.ExecuteToolAsync("GPXtoFeatures_conversion", parameters, environments);

                    if (gpResult.IsFailed == false)
                    {
                        parameters = Geoprocessing.MakeValueArray(outfc, "NEW_SELECTION", "\"Type\" = 'TRKPT'");
                        gpResult = await Geoprocessing.ExecuteToolAsync("SelectLayerByAttribute_management", parameters, environments, null, null, GPExecuteToolFlags.None);
                        if (gpResult.IsFailed != true)
                        {
                            parameters = Geoprocessing.MakeValueArray(outfc, Path.Combine(databasePath, "MyRide"), "Name");
                            gpResult = await Geoprocessing.ExecuteToolAsync("PointsToLine_management", parameters, environments);
                        }
                    }
                }
                catch (Exception exc)
                {
                    // Catch any exception found and display in a message box
                    MessageBox.Show("Exception caught while trying to run GP tool: " + exc.Message);
                    return;
                }
            });
        }
    }
}
