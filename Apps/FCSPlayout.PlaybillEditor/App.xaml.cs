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
        public App():base(ApplicationNames.PlaybillEdition)
        {

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            this.Initialize();
            base.OnStartup(e);

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void Initialize()
        {
            MLLicenseLib.MLLicenseManager.Instance.Timer();

            CheckMedialooks();

            // NOTE: 编单使用主播目录浏览。
            //MediaFilePathResolver.Current.CurrentStorage = MediaFileStorage.Primary;
        }
    }
}
