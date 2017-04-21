using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using FCSPlayout.Domain;
using FCSPlayout.AppInfrastructure;
using Prism.Modularity;
using System.Configuration;
using FCSPlayout.WPF.Core;
using FCSPlayout.Entities;

namespace FCSPlayout.WPFApp
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.RegisterInstance<IUserService>(new UserServiceAdapter());
            this.Container.RegisterInstance(new InteractionRequests());

            MediaFileStorage currentStorage =
                (MediaFileStorage)Enum.Parse(typeof(MediaFileStorage), ConfigurationManager.AppSettings["storageType"], true);
            this.Container.RegisterInstance<IMediaFilePathResolver>(new MediaFilePathResolver(currentStorage));
            this.Container.RegisterInstance<IMediaFileImageResolver>(new MediaFileImageResolver());

            this.Container.RegisterInstance<IPlayoutConfiguration>(PlayoutConfiguration.Current);
            this.Container.RegisterInstance(PlayoutRepository.GetMPlaylistSettings());
        }

        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ((ModuleCatalog)this.ModuleCatalog).AddModule(typeof(MediaItemModule));
            ((ModuleCatalog)this.ModuleCatalog).AddModule(typeof(PlaybillModule));
            ((ModuleCatalog)this.ModuleCatalog).AddModule(typeof(PlayoutModule));
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();    
        }
    }
}