using System;
using Prism.Regions;

namespace FCSPlayout.PlaybillEditor
{
    /// <summary>
    /// MediaItemListView.xaml 的交互逻辑
    /// </summary>
    public partial class DbMediaItemListView : FCSPlayout.WPF.Core.ViewBase
    {
        public DbMediaItemListView()
        {
            InitializeComponent();            
        }

        public DbMediaItemListView(DbMediaItemListViewModel viewModel)
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
