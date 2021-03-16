using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace PythonUsage
{
    internal class PythonFilePaneViewModel : DockPane
    {
        private const string _dockPaneID = "PythonUsage_PythonFilePane";

        protected PythonFilePaneViewModel()
        {
        }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _title = "Python bestand selectie";
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                SetProperty(ref _title, value, () => Title);
            }
        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class PythonFilePane_ShowButton : Button
    {
        protected override void OnClick()
        {
            PythonFilePaneViewModel.Show();
        }
    }
}
