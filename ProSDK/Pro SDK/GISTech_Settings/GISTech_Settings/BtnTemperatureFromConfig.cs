using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;

namespace GISTech_Settings
{
    internal class BtnTemperatureFromConfig : Button
    {
        protected override void OnClick()
        {
            MessageBox.Show($"De temperatuur is in {ConfigReader.Celius}");
        }
    }
}
