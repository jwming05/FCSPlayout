namespace FCSPlayout.MediaFileImporter
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
    }  
}
