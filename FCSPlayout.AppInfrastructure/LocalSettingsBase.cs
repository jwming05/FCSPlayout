using System;
using System.Reflection;

namespace FCSPlayout.AppInfrastructure
{
    [Serializable]
    public class LocalSettingsBase
    {
        [NonSerialized]
        private string _machineName;
        [NonSerialized]
        private string _applicationName;

        public string MachineName
        {
            get
            {
                if (_machineName == null)
                {
                    _machineName = GetMachineId().ToString();
                }
                return _machineName;
            }
        }
        public string ApplicationName
        {
            get
            {
                if (_applicationName == null)
                {
                    string location = Assembly.GetEntryAssembly().Location;
                    _applicationName = System.IO.Path.GetFileNameWithoutExtension(location);
                }
                return _applicationName;
            }
        }

        private Guid GetMachineId()
        {
            Guid machineId = LocalMachineIdManager.Instance.Get();
            if (Guid.Empty.Equals(machineId))
            {
                machineId = Guid.NewGuid();
                LocalMachineIdManager.Instance.Save(machineId);
            }
            return machineId;
        }
    }
}
