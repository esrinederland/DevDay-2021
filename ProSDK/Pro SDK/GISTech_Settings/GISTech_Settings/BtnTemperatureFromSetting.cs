using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using BuienradarUI;
using System.Linq;

namespace GISTech_Settings
{
    internal class BtnTemperatureFromSetting : Button
    {
        protected override void OnClick()
        {
            ReloadTemperatureLabel();
        }

        public static void ReloadTemperatureLabel()
        {
            QueuedTask.Run(() =>
            {
                // Check to see if the selected layer is a feature layer, if not, then prompt and exit.
                if (!(MapView.Active.GetSelectedLayers().First() is FeatureLayer featLayer))
                {
                    MessageBox.Show("Selecteer een feature layer in de Contents pane.", "Info");
                    return;
                }

                ActualWeatherCore.SetLabeling(featLayer);
            });
        }
    }
}
