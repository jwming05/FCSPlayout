using System;
using Prism.Regions;

namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// MediaItemListView.xaml 的交互逻辑
    /// </summary>
    public partial class MediaItemListView : FCSPlayout.WPF.Core.ViewBase
    {
        public MediaItemListView()
        {
            InitializeComponent();            
        }

        public MediaItemListView(MediaItemListViewModel viewModel)
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
