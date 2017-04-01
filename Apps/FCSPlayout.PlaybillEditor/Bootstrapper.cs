using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Microsoft.Practices.Unity;
using System.Windows;

namespace FCSPlayout.PlaybillEditor
{
    class Bootstrapper:Prism.Unity.UnityBootstrapper
    {
        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ((Prism.Modularity.ModuleCatalog)this.ModuleCatalog).AddModule(typeof(MediaItemModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.RegisterInstance<IUserService>(new UserServiceAdapter());
            this.Container.RegisterInstance<IMediaFileImageResolver>(new MediaFileImageResolver());
            this.Container.RegisterInstance<IMediaFilePathResolver>(new MediaFilePathResolver(MediaFileStorage.Primary));
        }

        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
        }
    }
}
