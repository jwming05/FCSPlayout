using FCSPlayout.Domain;
using System.Windows;
using System;
using Microsoft.Win32;
using System.Linq;

namespace FCSPlayout.AppInfrastructure
{
    public abstract class WPFApplicationBase: Application,ITimerAware
    {
        protected WPFApplicationBase(string applicationName)
        {
            this.Name = applicationName;
        }

        protected WPFApplicationBase(WPFApplicationConfiguration configuration)
        {
            if (configuration.RequireGarbageCollection)
            {
                if (configuration.GarbageCollectionInterval != null)
                {
                    this.GarbageCollector = new PeriodicalGarbageCollector(configuration.GarbageCollectionInterval.Value);
                }
                else
                {
                    this.GarbageCollector = new PeriodicalGarbageCollector();
                }
            }
        }

        public PeriodicalGarbageCollector GarbageCollector { get; private set; }

        public void RegisterApplicationExitAware(IApplicationExitAware aware)
        {
        }

        public string Name { get; set; }

        public void OnTimer()
        {
            if (this.GarbageCollector != null)
            {
                this.GarbageCollector.OnTimer();
            }
        }

        public static new WPFApplicationBase Current
        {
            get { return (WPFApplicationBase)Application.Current; }
        }

        protected static void CheckMedialooks()
        {
            RegistryKey root = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\MediaPlayer\Player\Schemes\mplatform");
            if (root == null)
            {
                root = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\MediaPlayer\Player\Schemes\mplatform");
            }

            if (root != null)
            {
                using (root)
                {
                    var valueNames = root.GetValueNames();
                    string name = "Runtime";
                    if (valueNames.Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)))
                    {
                        var value = root.GetValue(name);
                        //var t = value.GetType();

                        if (value == null || !object.Equals(value, 1))
                        {
                            root.SetValue(name, 1, RegistryValueKind.DWord);
                            root.Flush();
                        }
                    }
                    else
                    {
                        root.SetValue(name, 1, RegistryValueKind.DWord);
                        root.Flush();
                    }
                }
            }
        }
    }
}
