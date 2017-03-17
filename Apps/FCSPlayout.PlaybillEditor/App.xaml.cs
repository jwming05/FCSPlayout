using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;

namespace FCSPlayout.PlaybillEditor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : WPFApplicationBase
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //MediaSourcePathResolver.Current = new DefaultMediaSourcePathResolver(
            //    ConfigurationManager.AppSettings["primaryStorage"],
            //    ConfigurationManager.AppSettings["secondaryStorage"]);

            this.Initialize();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            //Common.GlobalEventAggregator.Instance.RaiseApplicationExit();
        }

        private void Initialize()
        {
            MLLicenseLib.MLLicenseManager.Instance.Timer();

            CheckMediaLooks();

            //FCSPlayout.Common.AppSettings.Instance.MediaFileDirectory = ConfigurationManager.AppSettings["MediaFileDirectory"];

            // NOTE: 编单使用主播目录浏览。
            MediaFilePathResolver.Current.CurrentStorage = MediaFileStorage.Primary;

            MediaFileDurationGetter.Current = new MLMediaFileDurationGetter();


            //LocalSettings.Instance.Initialize();
        }

        private static void CheckMediaLooks()
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
