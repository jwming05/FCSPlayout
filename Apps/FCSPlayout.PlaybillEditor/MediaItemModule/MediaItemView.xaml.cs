using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FCSPlayout.PlaybillEditor
{
    /// <summary>
    /// MediaItemView.xaml 的交互逻辑
    /// </summary>
    public partial class MediaItemView : UserControl
    {
        public MediaItemView()
        {
            InitializeComponent();
        }

        public MediaItemView(MediaItemViewModel viewModel, IRegionManager regionManager)
            :this()
        {
            this.DataContext = viewModel;
            this.RegionManager = regionManager;
        
            regionManager.Regions.CollectionChanged += Regions_CollectionChanged;
        }

        private void Regions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.RegionManager.Regions.ContainsRegionWithName("mediaItemRegion"))
            {
                RegionManager.Regions.CollectionChanged -= Regions_CollectionChanged;
                IRegion region = RegionManager.Regions["mediaItemRegion"];
                region.Context = this.DataContext;
            }
        }

        public IRegionManager RegionManager
        {
            get; private set;
        }
    }
}
