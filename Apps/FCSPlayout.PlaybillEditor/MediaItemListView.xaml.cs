using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using FCSPlayout.WPFApp.Models;
using FCSPlayout.WPFApp.ViewModels;
using Prism.Interactivity.InteractionRequest;
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
    /// MediaItemListView.xaml 的交互逻辑
    /// </summary>
    public partial class MediaItemListView : FCSPlayout.WPF.Core.ViewBase
    {

        public static readonly DependencyProperty SelectedMediaItemProperty =
            DependencyProperty.Register("SelectedMediaItem", typeof(BindableMediaItem), typeof(MediaItemListView), new FrameworkPropertyMetadata(null));


        private MediaItemListViewModel _viewModel;

        public MediaItemListView()
        {
            InitializeComponent();
            _viewModel = new MediaItemListViewModel();
            _viewModel.SelectedMediaItemChanged += ViewModel_SelectedMediaItemChanged;
            this.DataContext = _viewModel;
            this.dgMediaItem.MouseMove += DgMediaItem_MouseMove;
            mv = this;
        }

        private void DgMediaItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var elem = dgMediaItem.InputHitTest(e.GetPosition(dgMediaItem));

                DataGridRow row = FindDataGridRow(elem);
                if (row != null)
                {
                    var item = row.Item as BindableMediaItem;
                    if (item != null)
                    {
                        DragDrop.DoDragDrop(row, item.MediaItem, DragDropEffects.Copy);
                    }
                }
            }
        }

        protected override ViewModelBase ViewModel
        {
            get
            {
                return _viewModel;
            }

            set
            {
                _viewModel = (MediaItemListViewModel)value;
            }
        }
        public BindableMediaItem SelectedMediaItem
        {
            get { return (BindableMediaItem)GetValue(SelectedMediaItemProperty); }
            set { SetValue(SelectedMediaItemProperty, value); }
        }

        private void ViewModel_SelectedMediaItemChanged(object sender, EventArgs e)
        {
            this.SelectedMediaItem = _viewModel.SelectedMediaItem;
        }

        public IInteractionRequest AddNullMediaItemInteractionRequest
        {
            get
            {
                return _viewModel.AddNullMediaItemInteractionRequest;
            }
        }

        public IInteractionRequest AddChannelMediaItemsInteractionRequest
        {
            get
            {
                return _viewModel.AddChannelMediaItemsInteractionRequest;
            }
        }

        public ICommand AddMediaItemCommand
        {
            get
            {
                return _viewModel.AddMediaItemCommand;
            }
        }

        public ICommand AddNullMediaItemCommand
        {
            get
            {
                return _viewModel.AddNullMediaItemCommand;
            }
        }

        public ICommand AddChannelMediaItemsCommand
        {
            get
            {
                return _viewModel.AddChannelMediaItemsCommand;
            }
        }

        public ICommand DeleteMediaItemCommand
        {
            get
            {
                return _viewModel.DeleteMediaItemCommand;
            }
        }

        public ICommand EditMediaItemCommand
        {
            get
            {
                return _viewModel.EditMediaItemCommand;
            }
        }

        public ICommand SaveMediaItemsCommand
        {
            get
            {
                return _viewModel.SaveMediaItemsCommand;
            }
        }

        public ICommand LoadMediaItemsCommand
        {
            get
            {
                return _viewModel.LoadMediaItemsCommand;
            }
        }

        private void dgMediaItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //
        }

        private void dgMediaItem_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            //
        }

        private void dgMediaItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var elem=dgMediaItem.InputHitTest(e.GetPosition(dgMediaItem));

            DataGridRow row = FindDataGridRow(elem);
            if (row != null)
            {
                //this.EditMediaItemCommand.Execute(row.Item);
                if (MediaItemDoubleClicked != null)
                {
                    MediaItemDoubleClicked(this, new DataGridRowDoubleClickEventArgs(row.Item));
                }
            }
        }

        private DataGridRow FindDataGridRow(IInputElement element)
        {
            FrameworkElement fe = element as FrameworkElement;
            DataGridRow result = fe as DataGridRow;
            while (fe != null && result==null)
            {
                var parent=VisualTreeHelper.GetParent(fe);
                fe = parent as FrameworkElement;
                result = fe as DataGridRow;
            }

            return result;
        }

        public event EventHandler<DataGridRowDoubleClickEventArgs> MediaItemDoubleClicked;
        public static MediaItemListView mv;
    }

    public class DataGridRowDoubleClickEventArgs:EventArgs
    {
        public DataGridRowDoubleClickEventArgs(object item)
        {
            this.Item = item;
        }
        public object Item { get; private set; }
    }

   
}
