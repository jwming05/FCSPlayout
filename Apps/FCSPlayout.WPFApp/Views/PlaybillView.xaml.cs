using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using FCSPlayout.WPFApp.Models;
using FCSPlayout.WPFApp.ViewModels;
using Prism.Interactivity.InteractionRequest;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FCSPlayout.WPFApp.Views
{
    /// <summary>
    /// PlaybillView.xaml 的交互逻辑
    /// </summary>
    public partial class PlaybillView : FCSPlayout.WPF.Core.ViewBase
    {
        public static readonly DependencyProperty PlayScheduleInfoProperty =
            DependencyProperty.Register("PlayScheduleInfo", typeof(FCSPlayout.WPF.Core.PlayScheduleInfo), typeof(PlaybillView),
                new FrameworkPropertyMetadata(null, OnPlayScheduleInfoPropertyChanged));

        public IPlaylist2 Playlist
        {
            get { return (IPlaylist2)GetValue(PlaylistProperty); }
            set { SetValue(PlaylistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Playlist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register("Playlist", typeof(IPlaylist2), typeof(PlaybillView),
                new FrameworkPropertyMetadata(null, OnPlaylistPropertyChanged));
        private static void OnPlaylistPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlaybillView)dpObj).OnPlaylistChanged((IPlaylist)e.OldValue, (IPlaylist)e.NewValue);
        }

        private void OnPlaylistChanged(IPlaylist oldValue, IPlaylist newValue)
        {
            _viewModel.Playlist = this.Playlist;
            //throw new NotImplementedException();
        }

        internal void OnForcePlay(ForcePlayEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.OnForcePlay(e.CurrentPlayItem, e.CurrentRemainRange, e.ForcePlayItem);
            }
        }

        private static void OnPlayScheduleInfoPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlaybillView)dpObj).OnPlayScheduleInfoChanged((FCSPlayout.WPF.Core.PlayScheduleInfo)e.OldValue, (FCSPlayout.WPF.Core.PlayScheduleInfo)e.NewValue);
        }

        public static readonly DependencyProperty SelectedMediaItemProperty =
            DependencyProperty.Register("SelectedMediaItem", typeof(BindableMediaItem), typeof(PlaybillView),
                new FrameworkPropertyMetadata(null, OnSelectedMediaItemPropertyChanged));

        private static void OnSelectedMediaItemPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlaybillView)dpObj).OnSelectedMediaItemChanged((BindableMediaItem)e.OldValue, (BindableMediaItem)e.NewValue);
        }

        public static readonly DependencyProperty DisplayMessageInteractionRequestProperty =
            DependencyProperty.Register("DisplayMessageInteractionRequest", typeof(InteractionRequest<Notification>), typeof(PlaybillView),
                new FrameworkPropertyMetadata(null, OnDisplayMessageInteractionRequestPropertyChanged));

        private static void OnDisplayMessageInteractionRequestPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlaybillView)dpObj).OnDisplayMessageInteractionRequestChanged((InteractionRequest<Notification>)e.OldValue, (InteractionRequest<Notification>)e.NewValue);
        }

        public static readonly DependencyProperty EditDateTimeInteractionRequestProperty =
            DependencyProperty.Register("EditDateTimeInteractionRequest", typeof(InteractionRequest<EditDateTimeConfirmation>), typeof(PlaybillView),
                new FrameworkPropertyMetadata(null, OnEditDateTimeInteractionRequestPropertyChanged));

        private static void OnEditDateTimeInteractionRequestPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlaybillView)dpObj).OnEditDateTimeInteractionRequestChanged((InteractionRequest<EditDateTimeConfirmation>)e.OldValue, (InteractionRequest<EditDateTimeConfirmation>)e.NewValue);
        }

        public static readonly DependencyProperty EditCGItemsInteractionRequestProperty =
            DependencyProperty.Register("EditCGItemsInteractionRequest", typeof(InteractionRequest<EditCGItemsConfirmation>), typeof(PlaybillView),
                new FrameworkPropertyMetadata(null, OnEditCGItemsInteractionRequestPropertyChanged));

        private static void OnEditCGItemsInteractionRequestPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlaybillView)dpObj).OnEditCGItemsInteractionRequestChanged((InteractionRequest<EditCGItemsConfirmation>)e.OldValue, (InteractionRequest<EditCGItemsConfirmation>)e.NewValue);
        }

        public static readonly DependencyProperty LoadPlaybillInteractionRequestProperty =
            DependencyProperty.Register("LoadPlaybillInteractionRequest", typeof(InteractionRequest<LoadPlaybillConfirmation>), typeof(PlaybillView),
                new FrameworkPropertyMetadata(null, OnLoadPlaybillInteractionRequestProperty));

        private static void OnLoadPlaybillInteractionRequestProperty(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlaybillView)dpObj).OnLoadPlaybillInteractionRequestChanged((InteractionRequest<LoadPlaybillConfirmation>)e.OldValue, (InteractionRequest<LoadPlaybillConfirmation>)e.NewValue);
        }

        private void OnLoadPlaybillInteractionRequestChanged(InteractionRequest<LoadPlaybillConfirmation> oldValue, InteractionRequest<LoadPlaybillConfirmation> newValue)
        {
            _viewModel.LoadPlaybillInteractionRequest = this.LoadPlaybillInteractionRequest;
        }

        public IPlayItem SelectedPlayItem
        {
            get { return (IPlayItem)GetValue(SelectedPlayItemProperty); }
            set { SetValue(SelectedPlayItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPlayItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPlayItemProperty =
            DependencyProperty.Register("SelectedPlayItem", typeof(IPlayItem), typeof(PlaybillView), new FrameworkPropertyMetadata(null));

        private PlaybillViewModel _viewModel;

        public PlaybillView()
        {
            InitializeComponent();

            _viewModel = new PlaybillViewModel();
            this.DataContext = _viewModel;
            this.DragEnter += PlaybillView_DragEnter;
            this.DragLeave += PlaybillView_DragLeave;
            this.DragOver += PlaybillView_DragOver;
            this.Drop += PlaybillView_Drop;
            w1 = this;
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
        public ICommand AddPlayItemCommand
        {
            get
            {
                return _viewModel.AddPlayItemCommand;
            }
        }

        public ICommand SavePlaybillCommand
        {
            get { return _viewModel.SavePlaybillCommand; }
        }

        public ICommand LoadPlaybillCommand
        {
            get { return _viewModel.LoadPlaybillCommand; }
        }

        public ICommand DeletePlayItemCommand
        {
            get { return _viewModel.DeletePlayItemCommand; }
        }

        public BindableMediaItem SelectedMediaItem
        {
            get { return (BindableMediaItem)GetValue(SelectedMediaItemProperty); }
            set { SetValue(SelectedMediaItemProperty, value); }
        }

        public InteractionRequest<Notification> DisplayMessageInteractionRequest
        {
            get { return (InteractionRequest<Notification>)GetValue(DisplayMessageInteractionRequestProperty); }
            set { SetValue(DisplayMessageInteractionRequestProperty, value); }
        }

        public InteractionRequest<EditDateTimeConfirmation> EditDateTimeInteractionRequest
        {
            get { return (InteractionRequest<EditDateTimeConfirmation>)GetValue(EditDateTimeInteractionRequestProperty); }
            set { SetValue(EditDateTimeInteractionRequestProperty, value); }
        }

        public InteractionRequest<EditCGItemsConfirmation> EditCGItemsInteractionRequest
        {
            get { return (InteractionRequest<EditCGItemsConfirmation>)GetValue(EditCGItemsInteractionRequestProperty); }
            set { SetValue(EditCGItemsInteractionRequestProperty, value); }
        }

        public InteractionRequest<LoadPlaybillConfirmation> LoadPlaybillInteractionRequest
        {
            get { return (InteractionRequest<LoadPlaybillConfirmation>)GetValue(LoadPlaybillInteractionRequestProperty); }
            set { SetValue(LoadPlaybillInteractionRequestProperty, value); }
        }

        public FCSPlayout.WPF.Core.PlayScheduleInfo PlayScheduleInfo
        {
            get { return (FCSPlayout.WPF.Core.PlayScheduleInfo)GetValue(PlayScheduleInfoProperty); }
            set { SetValue(PlayScheduleInfoProperty, value); }
        }

        //public IPlaylist PlayItemList
        //{
        //    get
        //    {
        //        return _viewModel.PlayItemList;
        //    }
        //}

        public ICommand ForcePlayCommand
        {
            get { return _viewModel.ForcePlayCommand; }
        }

        public InteractionRequest<Notification> ForcePlayRequest
        {
            get { return _viewModel.ForcePlayRequest; }
        }

        private void OnDisplayMessageInteractionRequestChanged(InteractionRequest<Notification> oldValue, InteractionRequest<Notification> newValue)
        {
            _viewModel.DisplayMessageInteractionRequest = this.DisplayMessageInteractionRequest;
        }

        private void OnEditDateTimeInteractionRequestChanged(InteractionRequest<EditDateTimeConfirmation> oldValue, InteractionRequest<EditDateTimeConfirmation> newValue)
        {
            _viewModel.EditDateTimeInteractionRequest = this.EditDateTimeInteractionRequest;
        }

        private void OnEditCGItemsInteractionRequestChanged(InteractionRequest<EditCGItemsConfirmation> oldValue, InteractionRequest<EditCGItemsConfirmation> newValue)
        {
            _viewModel.EditCGItemsInteractionRequest = this.EditCGItemsInteractionRequest;
        }

        private void OnSelectedMediaItemChanged(BindableMediaItem oldValue, BindableMediaItem newValue)
        {
            _viewModel.SelectedMediaItem = this.SelectedMediaItem;
        }

        private void OnPlayScheduleInfoChanged(FCSPlayout.WPF.Core.PlayScheduleInfo oldValue, FCSPlayout.WPF.Core.PlayScheduleInfo newValue)
        {
            _viewModel.PlayScheduleInfo = this.PlayScheduleInfo;
        }

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


                ctxMenu.Tag = row.Item;
                _viewModel.UpdateMenuCommands();
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
                    var fileSource = item.Source as IFileMediaSource;
                    if (fileSource != null)
                    {
                        var volume = fileSource.AudioGain;
                        foreach (MenuItem subItem in menuItem.Items)
                        {
                            subItem.IsEnabled = true;
                            subItem.IsChecked = volume == int.Parse(subItem.Tag.ToString());
                        }
                    }
                    else
                    {
                        foreach (MenuItem subItem in menuItem.Items)
                        {
                            subItem.IsEnabled = false;
                            subItem.IsChecked = false;
                        }
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
                    var fileSource = item.Source as IFileMediaSource;
                    if (fileSource != null)
                    {
                        try
                        {
                            int volume = int.Parse(menuItem.Tag.ToString());
                            fileSource.AudioGain = volume;
                            //item.Parameters.AudioGain = volume;
                        }
                        catch
                        {

                        }
                    }
                    
                }
            }
        }

        public static PlaybillView w1;
    }
}
