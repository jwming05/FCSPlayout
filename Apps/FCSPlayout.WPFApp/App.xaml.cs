using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace FCSPlayout.WPFApp
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

            //PlayoutRepository.GetSettings();

            ChannelInfo channel = PlayoutRepository.GetAutoPaddingChannel();
            if (channel != null)
            {
                PlayoutConfiguration.Current.AutoPaddingMediaSource= new ChannelMediaSource(channel);
                //Playbill.PaddingMediaSource = new ChannelMediaSource(channel);
            }

            this.Initialize();
            base.OnStartup(e);
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private void Initialize()
        {
            MLLicenseLib.MLLicenseManager.Instance.Timer();
            FCSPlayout.PlayEngine.PlayoutRecordService.Current = DefaultPlayoutRecordService.Instance;

            CheckMediaLooks();

            //FCSPlayout.Common.AppSettings.Instance.MediaFileDirectory = ConfigurationManager.AppSettings["MediaFileDirectory"];

            MediaFilePathResolver.Current.CurrentStorage = 
                (MediaFileStorage)Enum.Parse(typeof(MediaFileStorage), ConfigurationManager.AppSettings["storageType"], true);
            MediaFileDurationGetter.Current = new MLMediaFileDurationGetter();
            LocalSettings.Instance.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            //Common.GlobalEventAggregator.Instance.RaiseApplicationExit();
        }

        //private static void CheckMediaLooks()
        //{
        //    using (RegistryKey root = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\MediaPlayer\Player\Schemes\mplatform"))
        //    {
        //        if (root == null)
        //        {
        //            using (var root2 = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\MediaPlayer\Player\Schemes\mplatform"))
        //            {
        //                //root2 = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\MediaPlayer\Player\Schemes\mplatform", true);
        //                if (root2 != null)
        //                {
        //                    root2.SetValue("Runtime", 1, RegistryValueKind.DWord);
        //                    root2.Flush();
        //                }
        //            }
        //        }
        //    }
        //}

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
