namespace BuienRadarSettings
{
    public partial class DamlSettings
    {
        public DamlSettings()
        {
            // // To add event handlers for saving and changing settings, uncomment the lines below:            
            if (UpgradeNeeded)
            {
                UpgradeNeeded = false;
                Save();
                Upgrade();
            }
        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            // Add code to handle the SettingChangingEvent event here.
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Add code to handle the SettingsSaving event here.
        }
    }
}
