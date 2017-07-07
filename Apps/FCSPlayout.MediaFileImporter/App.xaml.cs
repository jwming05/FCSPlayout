using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace FCSPlayout.MediaFileImporter
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : WPFApplicationBase
    {
        public App()
            :base(ApplicationNames.FileImporter)
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.Initialize();
            base.OnStartup(e);

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private void Initialize()
        {
            MLLicenseLib.MLLicenseManager.Instance.Timer();
            CheckMedialooks();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }   
    }
}