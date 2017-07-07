using Prism.Modularity;
using Prism.Regions;

namespace FCSPlayout.MediaFileImporter
{
    class MediaItemModule:IModule
    {
        private IRegionManager _regionManager;
        public MediaItemModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MediaItemRegion, typeof(DbMediaItemListView));
        }
    }
}
