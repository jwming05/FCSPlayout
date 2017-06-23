using Prism.Modularity;
using Prism.Regions;

namespace FCSPlayout.PlaybillEditor
{
    class PlaybillModule : IModule
    {
        private IRegionManager _regionManager;

        public PlaybillModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("mainRightRegion", typeof(PlaybillView));
        }
    }
}
