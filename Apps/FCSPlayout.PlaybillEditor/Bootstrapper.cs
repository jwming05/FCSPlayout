using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using Microsoft.Practices.Unity;
using Prism.Regions;
using System.Windows;

namespace FCSPlayout.PlaybillEditor
{
    class Bootstrapper:Prism.Unity.UnityBootstrapper
    {
        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ((Prism.Modularity.ModuleCatalog)this.ModuleCatalog).AddModule(typeof(MediaItemModule));
            ((Prism.Modularity.ModuleCatalog)this.ModuleCatalog).AddModule(typeof(PlaybillModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.RegisterInstance(new InteractionRequests());

            this.Container.RegisterInstance<IUserService>(new UserServiceAdapter());
            this.Container.RegisterInstance<IMediaFileImageResolver>(new MediaFileImageResolver());
            this.Container.RegisterInstance<IMediaFilePathResolver>(new MediaFilePathResolver(MediaFileStorage.Primary));
            this.Container.RegisterInstance<IPlayoutConfiguration>(PlayoutConfiguration.Current);

            this.Container.RegisterInstance(PlayoutRepository.GetMPlaylistSettings());

            this.Container.RegisterInstance<IMediaFileService>(new DefaultMediaFileService(null));
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            this.Container.Resolve<IRegionViewRegistry>().RegisterViewWithRegion("previewRegion", typeof(PreviewPlayControl));
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
