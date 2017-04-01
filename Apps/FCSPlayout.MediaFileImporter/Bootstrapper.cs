﻿using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;
using System.Windows;

namespace FCSPlayout.MediaFileImporter
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            this.Container.RegisterInstance(PlayoutRepository.GetMPlaylistSettings());
            this.Container.RegisterInstance(new InteractionRequests());
            this.Container.RegisterInstance<IMediaFileImageResolver>(new MediaFileImageResolver());
            this.Container.RegisterInstance<IUserService>(new UserServiceAdapter());
            this.Container.RegisterInstance<IMediaFilePathResolver>(new MediaFilePathResolver(MediaFileStorage.Primary));
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();

            var viewRegistry = this.Container.Resolve<IRegionViewRegistry>();
            viewRegistry.RegisterViewWithRegion("previewRegion", typeof(PlayControl2));
            viewRegistry.RegisterViewWithRegion("mediaItemListRegion", typeof(MediaItemListView));
            viewRegistry.RegisterViewWithRegion("mediaItemList2Region", typeof(MediaItemListView2));
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