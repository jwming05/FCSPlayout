using FCSPlayout.Entities;
using System;
using System.Linq;

namespace FCSPlayout.WPFApp
{
    public class SettingsManager
    {
        private static Guid _BMDSwitcherId = Guid.Empty; // int.MinValue;

        public static Guid BMDSwitcherId
        {
            get
            {
                if (_BMDSwitcherId == Guid.Empty)
                {
                    var setting = GetSetting("BMDSwitcherId");
                    if (setting != null)
                    {
                        _BMDSwitcherId =Guid.Parse(setting.Value);
                    }
                }
                return _BMDSwitcherId;
            }
            set
            {
                if (_BMDSwitcherId != value)
                {
                    _BMDSwitcherId = value;
                    SaveWorkstationSetting("BMDSwitcherId", _BMDSwitcherId.ToString());
                }
            }
        }

        public static void SaveWorkstationSetting(string name, string value, string tag=null, string groupName=null)
        {
            PlayoutRepository.
                SaveSetting(MachineName, ApplicationName, groupName, name, value, tag);
        }

        public static void SaveMachineSetting(string name, string value, string tag = null, string groupName = null)
        {
            PlayoutRepository.SaveSetting(MachineName, null, groupName, name, value, tag);
        }

        public static void SaveApplicationSetting(string name, string value, string tag = null, string groupName = null)
        {
            PlayoutRepository.SaveSetting(null, ApplicationName, groupName, name, value, tag);
        }

        public static void SaveGlobalSetting(string name, string value, string tag = null, string groupName = null)
        {
            PlayoutRepository.SaveSetting(null, null, groupName, name, value, tag);
        }

        public static SettingInfo GetSetting(string name,string groupName = null)
        {
            var settings = PlayoutRepository.GetSettings(name, groupName);

            // First, workstation
            var setting = settings.SingleOrDefault(i => i.Scope.MachineName == MachineName && i.Scope.ApplicationName == ApplicationName);
            if (setting != null)
            {
                return setting;
            }

            // Second, application
            setting = settings.SingleOrDefault(i => i.Scope.ApplicationName == ApplicationName);
            if (setting != null)
            {
                return setting;
            }

            // Third, machine
            return settings.SingleOrDefault(i => i.Scope.MachineName == MachineName);
        }

        public static string MachineName { get { return LocalSettings.Instance.MachineName; } }
        public static string ApplicationName { get { return LocalSettings.Instance.ApplicationName; } }
    }
}
