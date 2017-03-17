using Microsoft.Win32;
using System;

namespace FCSPlayout.AppInfrastructure
{
    public abstract class LocalMachineIdManager
    {
        private static LocalMachineIdManager _instance;

        public static LocalMachineIdManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    return DefaultLocalMachineIdManager.Instance;
                }
                return _instance;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (_instance != null)
                {
                    throw new InvalidOperationException(typeof(LocalMachineIdManager).FullName +
                        ".Instance already initialized.");
                }
                _instance = value;
            }
        }

        public abstract void Save(Guid machineId);
        public abstract Guid Get();

        public abstract void Reset();

        public abstract void Delete();
        public sealed class DefaultLocalMachineIdManager : LocalMachineIdManager
        {
            private new static readonly DefaultLocalMachineIdManager _instance = new DefaultLocalMachineIdManager();

            public new static DefaultLocalMachineIdManager Instance
            {
                get
                {
                    return _instance;
                }
            }

            private const string _machineIdKeyPath = "software\\fcsplayout";
            private const string _machineIdValueName = "MachineId";

            private DefaultLocalMachineIdManager()
            {
            }

            public override void Save(Guid machineId)
            {
                using (RegistryKey machineIdKey = OpenMachineIdKey())
                {
                    machineIdKey.SetValue(_machineIdValueName, machineId.ToString());
                }
            }

            public override Guid Get()
            {
                using (RegistryKey machineIdKey = OpenMachineIdKey())
                {
                    Guid machineId = Guid.Empty;
                    object value = machineIdKey.GetValue(_machineIdValueName);
                    if (value == null)
                    {
                        machineIdKey.SetValue(_machineIdValueName, machineId.ToString());
                    }
                    else if (!Guid.TryParse((string)value, out machineId))
                    {
                        return Guid.Empty;
                    }

                    return machineId;
                }
            }

            public override void Reset()
            {
                using (RegistryKey machineIdKey = OpenMachineIdKey(false))
                {
                    if (machineIdKey != null)
                    {
                        machineIdKey.SetValue(_machineIdValueName, Guid.Empty.ToString());
                    }
                }
            }

            public override void Delete()
            {
                Registry.LocalMachine.DeleteSubKeyTree(_machineIdKeyPath, false);
            }

            private RegistryKey OpenMachineIdKey(bool createWhenExists = true)
            {
                RegistryKey machineIdKey = Registry.LocalMachine.OpenSubKey(_machineIdKeyPath, true);
                if (machineIdKey == null)
                {
                    Registry.LocalMachine.CreateSubKey(_machineIdKeyPath);
                }
                return machineIdKey;
            }
        }
    }
}
