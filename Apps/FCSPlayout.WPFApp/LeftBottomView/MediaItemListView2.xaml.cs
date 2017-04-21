using System;
using Prism.Regions;

namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// MediaItemListView.xaml 的交互逻辑
    /// </summary>
    public partial class MediaItemListView2 : FCSPlayout.WPF.Core.ViewBase
    {
        public MediaItemListView2()
        {
            InitializeComponent();            
        }

        public MediaItemListView2(MediaItemListViewModel2 viewModel)
            :this()
        {
            this.DataContext = viewModel;
        }

        public string Title
        {
            get { return "文件库"; }
        }
    }  
}
