﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BuienRadarSettings {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
    public sealed partial class DamlSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static DamlSettings defaultInstance = ((DamlSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new DamlSettings())));
        
        public static DamlSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Celius {
            get {
                return ((bool)(this["Celius"]));
            }
            set {
                this["Celius"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UpgradeNeeded {
            get {
                return ((bool)(this["UpgradeNeeded"]));
            }
            set {
                this["UpgradeNeeded"] = value;
            }
        }
    }
}
