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
        public App():base(ApplicationNames.PlayService)
        {
        }

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

            CheckMedialooks();

            //FCSPlayout.Common.AppSettings.Instance.MediaFileDirectory = ConfigurationManager.AppSettings["MediaFileDirectory"];

            //MediaFilePathResolver.Current.CurrentStorage = 
            //    (MediaFileStorage)Enum.Parse(typeof(MediaFileStorage), ConfigurationManager.AppSettings["storageType"], true);
            //MediaFileDurationGetter.Current = new MLMediaFileDurationGetter();
            LocalSettings.Instance.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            //Common.GlobalEventAggregator.Instance.RaiseApplicationExit();
        }
    }
}
