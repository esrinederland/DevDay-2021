using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Controls;

namespace PythonUsage
{
    /// <summary>
    /// Interaction logic for PythonFilePaneView.xaml
    /// </summary>
    public partial class PythonFilePaneView : UserControl
    {
        public PythonFilePaneView()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Python files (*.py)|*.py|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                tbxFileName.Text = openFileDialog.FileName;
            }
        }

        private void BtnStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string fileName = tbxFileName.Text;
            BtnStart.IsEnabled = false;
            _ = QueuedTask.Run(() =>
              {
                  try
                  {
                      // Create and format path to Pro
                      var pathProExe = Path.GetDirectoryName((new Uri(Assembly.GetEntryAssembly().CodeBase)).AbsolutePath);
                      if (pathProExe == null)
                      {
                          return;
                      }
                      pathProExe = Uri.UnescapeDataString(pathProExe);
                      pathProExe = Path.Combine(pathProExe, @"Python\envs\arcgispro-py3");
                      // Create and format path to Python
                      var pathPython = Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath);
                      if (pathPython == null)
                      {
                          return;
                      }
                      pathPython = Uri.UnescapeDataString(pathPython);

                      // Create and format process command string.
                      var myCommand = $@"/c """"{Path.Combine(pathProExe, "python.exe")}"" ""{Path.Combine(pathPython, fileName)}""""";

                      // Create process start info, with instruction settings
                      var procStartInfo = new ProcessStartInfo("cmd", myCommand)
                      {
                          RedirectStandardOutput = true,
                          RedirectStandardError = true,
                          UseShellExecute = false,
                          CreateNoWindow = true
                      };
                      // Create process and start it
                      Process proc = new Process
                      {
                          StartInfo = procStartInfo
                      };
                      proc.Start();
                      // Create and format result string
                      string result = proc.StandardOutput.ReadToEnd();
                      string error = proc.StandardError.ReadToEnd();
                      if (!string.IsNullOrEmpty(error))
                      {
                          result += $"{result} Error: {error}";
                      }
                      // Show result string
                      MessageBox.Show(result, "Info");
                  }

                  catch (Exception exc)
                  {
                      // Catch any exception found and display in a message box
                      MessageBox.Show("Exception caught while trying to run Python tool: " + exc.Message);
                      return;
                  }
            });
            BtnStart.IsEnabled = true;
        }
    }
}
