using FCSPlayout.WPFApp.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPFApp
{
    class PlayoutModule : IModule
    {
        private IRegionManager _regionManager;

        public PlayoutModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("playoutRegion", typeof(PlayoutView));
        }
    }
}
