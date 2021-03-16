using Microsoft.Win32;
using System.Configuration;
using System.IO;

namespace GISTech_Settings
{
    public static class ConfigReader
    {
        private static Configuration customConfig = null;

        #region public properties
        public static string Celius
        {
            get
            {
                return GetValueFromConfig("Celius");
            }
        }
        #endregion

        #region private methods
        private static string GetValueFromConfig(string key)
        {
            string configFile = "";

            if (customConfig != null)
            {
                return customConfig.AppSettings.Settings[key].Value;
            }

            // Get configfile from registry
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"Software\Esri Nederland\GISTech"))
            {
                if (registryKey != null)
                {
                    configFile = (string)registryKey.GetValue("Config");
                    if (File.Exists(configFile))
                    {
                        ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
                        {
                            ExeConfigFilename = configFile
                        };
                        customConfig = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                        return customConfig.AppSettings.Settings[key].Value;
                    }
                }
            }
            return "Er is geen waarde";
        }
        #endregion
    }
}
