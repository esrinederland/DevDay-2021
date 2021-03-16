using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.IO;
using System.Linq;

namespace UnitTesting
{
    internal class BtnTest : Button
    {
        protected override async void OnClick()
        {
            await QueuedTask.Run(async () =>
            {
                var pointLayer = (MapView.Active.Map.Layers.First(layer => layer.Name.Equals("Points")) as FeatureLayer);

                if (pointLayer == null)
                {
                    MessageBox.Show("Point layer can not be found...", "Error", System.Windows.MessageBoxButton.OK);
                    return;
                }

                try
                {
                    // Get the path to the layer's feature class and path to a new 200-foot buffer feature class
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
                    var gpResult = await Geoprocessing.ExecuteToolAsync("Buffer_analysis", parameters, environments);
                    // Show a messagebox with the results
                    Geoprocessing.ShowMessageBox(gpResult.Messages, "GP Meldingen", gpResult.IsFailed ? GPMessageBoxStyle.Error : GPMessageBoxStyle.Default);
                    MessageBox.Show("Lets Unit test here..");
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
