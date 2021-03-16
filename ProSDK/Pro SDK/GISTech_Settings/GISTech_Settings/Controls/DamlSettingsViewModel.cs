using ArcGIS.Desktop.Framework.Contracts;
using System.Threading.Tasks;
using BuienRadarSettings;

namespace GISTech_Settings.Controls
{
    internal class DamlSettingsViewModel : Page
    {
        #region members
        private static bool _isGeneralExpanded = true;
        private static bool _isOtherExpanded = true;
        private static bool _isCelsius;
        private static bool _isFahrenheit;

        private bool _origIsCelsius;
        #endregion

        #region properties
        /// <summary>
        /// Text shown inside the page hosted by the property sheet
        /// </summary>
        public string DataUIContent
        {
            get
            {
                return base.Data[0] as string;
            }
            set
            {
                SetProperty(ref base.Data[0], value, () => DataUIContent);
            }
        }

        public bool IsGeneralExpanded
        {
            get
            {
                return _isGeneralExpanded;
            }
            set
            {
                SetProperty(ref _isGeneralExpanded, value, () => IsGeneralExpanded);
            }
        }

        public bool IsOtherExpanded
        {
            get
            {
                return _isOtherExpanded;
            }
            set
            {
                SetProperty(ref _isOtherExpanded, value, () => IsOtherExpanded);
            }
        }

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
                    base.IsModified = true;
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
                    base.IsModified = true;
                    SetProperty(ref _isCelsius, !value, () => IsCelsius);
                }
                
            }
        }
        #endregion

        #region Page Overrides
        /// <summary>
        /// Perform special actions when the page is to be cancelled.
        /// </summary>
        /// <returns>A Task that represents CancelAsync</returns>
        protected override Task CancelAsync()
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Called when the page is destroyed.
        /// </summary>
        protected override void Uninitialize()
        {
        }

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
        /// Invoked when the OK or apply button on the property sheet has been clicked.
        /// </summary>
        /// <returns>A task that represents the work queued to execute in the ThreadPool.</returns>
        /// <remarks>This function is only called if the page has set its IsModified flag to true.</remarks>
        protected override Task CommitAsync()
        {
            if (_origIsCelsius != IsCelsius)
            {
                // save the new settings
                DamlSettings settings = DamlSettings.Default;

                settings.Celius = IsCelsius;

                settings.Save();
            }
            return Task.FromResult(0);
        }
        #endregion
    }
}
