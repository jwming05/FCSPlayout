using FCSPlayout.WPF.Core;
using Prism.Modularity;
using Prism.Regions;

namespace FCSPlayout.MediaFileImporter
{
    class PreviewModule : IModule
    {
        private IRegionManager _regionManager;
        public PreviewModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.PreviewRegion, typeof(PreviewPlayControl));
        }
    }
}
