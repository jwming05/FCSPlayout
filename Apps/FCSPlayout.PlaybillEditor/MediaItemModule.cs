using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.PlaybillEditor
{
    public class MediaItemModule : IModule
    {
        private IRegionViewRegistry _viewRegistry;
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public MediaItemModule(IRegionManager regionManager,IUnityContainer container, IRegionViewRegistry viewRegistry)
        {
            _regionManager = regionManager;
            _container = container;
            _viewRegistry = viewRegistry;
        }
        public void Initialize()
        {
            _regionManager.AddToRegion("mainLeftRegion", this._container.Resolve<MediaItemView>());

            _regionManager.RegisterViewWithRegion("mediaItemRegion",typeof(MediaItemListView2));
            _regionManager.RegisterViewWithRegion("mediaItemRegion", typeof(ChannelItemListView));

            //_regionManager.AddToRegion("mediaItemRegion", this._container.Resolve<MediaItemListView2>());
            //_regionManager.AddToRegion("mediaItemRegion", this._container.Resolve<ChannelItemListView>());
            //_viewRegistry.RegisterViewWithRegion("mediaItemRegion", typeof(MediaItemListView2));
        }
    }
}
