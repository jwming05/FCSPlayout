using Prism.Modularity;
using Prism.Regions;

namespace FCSPlayout.MediaFileImporter
{
    class MediaFileModule : IModule
    {
        private IRegionManager _regionManager;
        public MediaFileModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MediaFileRegion, typeof(MediaItemListView));
        }
    }
}
