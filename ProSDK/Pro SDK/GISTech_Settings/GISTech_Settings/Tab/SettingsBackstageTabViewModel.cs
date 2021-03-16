using ArcGIS.Desktop.Framework.Contracts;
using BuienRadarSettings;
using System.Threading.Tasks;


namespace GISTech_Settings.Tab
{
    internal class SettingsBackstageTabViewModel : BackstageTab
    {
        #region members
        private static bool _isCelsius;
        private static bool _isFahrenheit;

        private bool _origIsCelsius;
        #endregion

        #region constructor
        public SettingsBackstageTabViewModel()
        {
            // nothing
        }
        #endregion

        #region BackstageTab Overrides
        /// <summary>
        /// Called when the backstage tab is selected.
        /// </summary>
        protected override Task InitializeAsync()
        {
            // get the default settings
            DamlSettings settings = DamlSettings.Default;
            // assign to the values binding to the controls
            _isCelsius = settings.Celius;
            _isFahrenheit = !_isCelsius;

            // keep track of the original values (used for comparison when saving)
            _origIsCelsius = _isCelsius;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Called when the backstage tab is unselected.
        /// </summary>
        protected override Task UninitializeAsync()
        {
            if (_origIsCelsius != _isCelsius)
            {
                // save the new settings
                DamlSettings settings = DamlSettings.Default;

                settings.Celius = IsCelsius;

                settings.Save();

                BtnTemperatureFromSetting.ReloadTemperatureLabel();
            }

            return base.UninitializeAsync();
        }

        #endregion

        #region properties
        public bool IsCelsius
        {
            get
            {
                return _isCelsius;
            }
            set
            {
                if (SetProperty(ref _isCelsius, value, () => IsCelsius))
                {
                    SetProperty(ref _isFahrenheit, !value, () => IsFahrenheit);
                }
            }
        }

        public bool IsFahrenheit
        {
            get
            {
                return _isFahrenheit;
            }
            set
            {
                if (SetProperty(ref _isFahrenheit, value, () => IsFahrenheit))
                {
                    SetProperty(ref _isCelsius, !value, () => IsCelsius);
                }

            }
        }
        #endregion
    }
}
