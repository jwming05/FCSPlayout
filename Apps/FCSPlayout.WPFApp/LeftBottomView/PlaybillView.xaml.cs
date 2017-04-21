using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Interactivity.InteractionRequest;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// PlaybillView.xaml 的交互逻辑
    /// </summary>
    public partial class PlaybillView : FCSPlayout.WPF.Core.ViewBase
    {
        private PlaybillViewModel _viewModel;

        public PlaybillView()
        {
            InitializeComponent();
            //pv = this;
        }

        public PlaybillView(PlaybillViewModel viewModel)
            :this()
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
            this.DragEnter += PlaybillView_DragEnter;
            this.DragLeave += PlaybillView_DragLeave;
            this.DragOver += PlaybillView_DragOver;
            this.Drop += PlaybillView_Drop;
        }
        private void PlaybillView_Drop(object sender, DragEventArgs e)
        {
            var elem = dgPlayItems.InputHitTest(e.GetPosition(dgPlayItems));

            DataGridRow row = FindDataGridRow(elem);
            if (row != null)
            {
                if (_viewModel.CanChangeSource(row.Item))
                {
                    if (e.Data.GetDataPresent(typeof(MediaItem).FullName))
                    {
                        _viewModel.ChangeSource((BindablePlayItem)row.Item, (MediaItem)e.Data.GetData(typeof(MediaItem).FullName));
                        //e.Effects = DragDropEffects.Copy;
                        //return;
                    }
                }
            }
        }

        private void PlaybillView_DragOver(object sender, DragEventArgs e)
        {
            var elem = dgPlayItems.InputHitTest(e.GetPosition(dgPlayItems));

            DataGridRow row = FindDataGridRow(elem);
            if (row != null)
            {
                if (_viewModel.CanChangeSource(row.Item))
                {
                    if (e.Data.GetDataPresent(typeof(MediaItem).FullName))
                    {
                        e.Effects = DragDropEffects.Copy;
                        return;
                    }
                }
                
            }
            e.Effects = DragDropEffects.None;
        }

        private void PlaybillView_DragLeave(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void PlaybillView_DragEnter(object sender, DragEventArgs e)
        {
            
        }

        protected override ViewModelBase ViewModel
        {
            get
            {
                return _viewModel;
            }

            set
            {
                _viewModel = (PlaybillViewModel)value;
            }
        }
        //public ICommand AddPlayItemCommand
        //{
        //    get
        //    {
        //        return _viewModel.AddPlayItemCommand;
        //    }
        //}

        public ICommand SavePlaybillCommand
        {
            get { return _viewModel.SavePlaybillCommand; }
        }

        public ICommand LoadPlaybillCommand
        {
            get { return _viewModel.LoadPlaybillCommand; }
        }

        public ICommand CreatePlaybillCommand
        {
            get { return _viewModel.CreatePlaybillCommand; }
        }

        public ICommand DeletePlayItemCommand
        {
            get { return _viewModel.DeletePlayItemCommand; }
        }

        

        //public PlayEngine.IPlaylist PlayItemList
        //{
        //    get
        //    {
        //        return _viewModel.PlayItemList;
        //    }
        //}

        

        //private void OnEditDateTimeInteractionRequestChanged(InteractionRequest<EditDateTimeConfirmation> oldValue, InteractionRequest<EditDateTimeConfirmation> newValue)
        //{
        //    _viewModel.EditDateTimeInteractionRequest = this.EditDateTimeInteractionRequest;
        //}

        

        //private void OnSelectedMediaItemChanged(BindableMediaItem oldValue, BindableMediaItem newValue)
        //{
        //    _viewModel.SelectedMediaItem = this.SelectedMediaItem;
        //}

        //private void OnPlayScheduleInfoChanged(FCSPlayout.WPF.Core.PlayScheduleInfo oldValue, FCSPlayout.WPF.Core.PlayScheduleInfo newValue)
        //{
        //    _viewModel.PlayScheduleInfo = this.PlayScheduleInfo;
        //}

        private void DataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var elem = dgPlayItems.InputHitTest(e.GetPosition(dgPlayItems));
            DataGridRow row = FindDataGridRow(elem);

            if (row != null)
            {
                //this.popup.IsOpen = true;
                //this.EditMediaItemCommand.Execute(row.Item);
                //if (MediaItemDoubleClicked != null)
                //{
                //    MediaItemDoubleClicked(this, new DataGridRowDoubleClickEventArgs(row.Item));
                //}

                var bindablePlayItem = row.Item as BindablePlayItem;
                if (bindablePlayItem != null)
                {
                    _viewModel.SelectedPlayItem = bindablePlayItem;
                }
                //ctxMenu.Tag = row.Item;
                //_viewModel.UpdateMenuCommands();
            }
            else
            {
                e.Handled = true;
            }
        }

        private DataGridRow FindDataGridRow(IInputElement element)
        {
            FrameworkElement fe = element as FrameworkElement;
            DataGridRow result = fe as DataGridRow;
            while (fe != null && result == null)
            {
                var parent = VisualTreeHelper.GetParent(fe);
                fe = parent as FrameworkElement;
                result = fe as DataGridRow;
            }

            return result;
        }

        private void dgPlayItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.SelectedPlayItem = _viewModel.SelectedPlayItem;
        }

        private void GainMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (ctxMenu.Tag != null)
            {
                BindablePlayItem item = ctxMenu.Tag as BindablePlayItem;
                MenuItem menuItem = (MenuItem)sender;
                if (item != null && object.Equals(menuItem.Tag,"Gain"))
                {

                    var audioGain = item.AudioGain; //.Parameters.AudioGain;
                    foreach (MenuItem subItem in menuItem.Items)
                    {
                        subItem.IsChecked = audioGain == int.Parse(subItem.Tag.ToString());
                    }
                }
            }
        }

        private void GainMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ctxMenu.Tag != null)
            {
                BindablePlayItem item = ctxMenu.Tag as BindablePlayItem;
                MenuItem menuItem = e.Source as MenuItem;

                if (item != null && menuItem!=null)
                {
                    try
                    {
                        int volume = int.Parse(menuItem.Tag.ToString());
                        //TODO: 修改AudioGain

                        //item.Parameters.AudioGain = volume;
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
