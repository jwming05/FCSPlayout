using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System.Windows.Input;
using System;
using FCSPlayout.Entities;

namespace FCSPlayout.WPFApp
{
    public class MainViewModel : ShellModelBase // BindableBase
    {
        //private readonly ITimer _timer;

        //private Playbill _playbill;
        //private ObservableCollection<IPlayItem> _playItemCollection;
        //private CollectionView _playItemsView;
        //private IPlayItem _selectedPlayItem;

        private readonly DelegateCommand _collectGarbageCommand;
        private PeriodicalGarbageCollector _garbageCollector = new PeriodicalGarbageCollector();

        private readonly DelegateCommand _editCGItemsCommand;

        //private IPlaylist _playlist;

        public MainViewModel(/*ITimer timer*/ IUserService userService, InteractionRequests interactionRequests, IEventAggregator eventAggregator)
            :base(userService)
        {
            //eventAggregator.GetEvent<PubSubEvent<IPlayableItem>>().Subscribe(Preview);

            this.PreviewInteractionRequest = interactionRequests.PreviewInteractionRequest;
            this.OpenFileInteractionRequest = interactionRequests.OpenFileInteractionRequest;
            this.DisplayMessageInteractionRequest = interactionRequests.DisplayMessageInteractionRequest;
            this.ConfirmationInteractionRequest = interactionRequests.ConfirmationInteractionRequest;
            this.EditDurationInteractionRequest = interactionRequests.EditDurationInteractionRequest;
            this.EditDateTimeInteractionRequest = interactionRequests.EditDateTimeInteractionRequest;
            this.EditCGItemsInteractionRequest = interactionRequests.EditCGItemsInteractionRequest;
            this.SaveFileInteractionRequest = interactionRequests.SaveFileInteractionRequest;
            this.LoadPlaybillInteractionRequest = interactionRequests.LoadPlaybillInteractionRequest;

            //_timer = timer;

            //_playItemCollection = new ObservableCollection<IPlayItem>();
            //_playItemsView = new CollectionView(_playItemCollection);



            //_playbill = new Playbill();
            //_playbill.PlayItemsChanged += Playbill_PlayItemsChanged;

            //_playlist = new Playlist();

            _collectGarbageCommand = new DelegateCommand(ExecuteCollectGarbage);

            _editCGItemsCommand = new DelegateCommand(EditCGItems);
        }

        private void EditCGItems()
        {
            var items = PlayoutRepository.GetCGItems();
            this.EditCGItemsInteractionRequest.Raise(new EditCGItemsConfirmation { Title = "编辑字幕", Items = items.Clone() },
                c=> 
                {
                    if (c.Confirmed)
                    {
                        PlayoutRepository.SaveCGItems(c.Items);
                    }
                });
        }

        //private void Preview(IPlayableItem playableItem)
        //{
        //    this.PreviewInteractionRequest.Raise(new PreviewRequestConfirmation(playableItem) { Title = "预览" },
        //        (c)=> 
        //        {
        //            if (c.Confirmed)
        //            {

        //            }

        //            playableItem.ClosePreview();
        //        });
        //}
        private void ExecuteCollectGarbage()
        {
            _garbageCollector.OnTimer();
        }

        //private void RaiseDisplayMessageInteractionRequest(string title, string message)
        //{
        //    DisplayMessageInteractionRequest.Raise(new Notification { Title = title, Content = message });
        //}


        //private void Playbill_PlayItemsChanged(object sender, EventArgs e)
        //{
        //    _playItemCollection.Clear();
        //    foreach (var item in _playbill/*.GetPlayItems()*/)
        //    {
        //        _playItemCollection.Add(new BindablePlayItem(item));
        //    }
        //}
        #region



        //public CollectionView PlayItemsView
        //{
        //    get
        //    {
        //        return _playItemsView;
        //    }

        //    set
        //    {
        //        _playItemsView = value;
        //    }
        //}


        #endregion

        public IInteractionRequest OpenFileInteractionRequest
        {
            get;private set;
        }


        //public InteractionRequest<EditMediaItemConfirmation> EditMediaItemInteractionRequest
        //{
        //    get
        //    {
        //        return _editMediaItemInteractionRequest;
        //    }
        //}

        public InteractionRequest<EditDurationConfirmation> EditDurationInteractionRequest
        {
            get;private set;
        }

        //public IPlayItem SelectedPlayItem
        //{
        //    get { return _selectedPlayItem; }
        //    set
        //    {
        //        _selectedPlayItem = value;
        //        //this._deletePlayItemCommand.RaiseCanExecuteChanged();
        //    }
        //}

        public IInteractionRequest DisplayMessageInteractionRequest
        {
            get;private set;
        }

        public IInteractionRequest SaveFileInteractionRequest
        {
            get;private set;
        }

        //public ITimer Timer
        //{
        //    get
        //    {
        //        return _timer;
        //    }
        //}

        public ICommand CollectGarbageCommand
        {
            get
            {
                return _collectGarbageCommand;
            }
        }

        public InteractionRequest<EditDateTimeConfirmation> EditDateTimeInteractionRequest
        {
            get;private set;
        }

        public InteractionRequest<EditCGItemsConfirmation> EditCGItemsInteractionRequest
        {
            get;private set;
        }

        public InteractionRequest<LoadPlaybillConfirmation> LoadPlaybillInteractionRequest
        {
            get;private set;
        }

        public InteractionRequest<PreviewRequestConfirmation> PreviewInteractionRequest { get; private set; }
        public InteractionRequest<Confirmation> ConfirmationInteractionRequest { get; private set; }

        public DelegateCommand EditCGItemsCommand
        {
            get
            {
                return _editCGItemsCommand;
            }
        }

        //public IPlaylist Playlist
        //{
        //    get { return _playlist; }
        //    set
        //    {
        //        _playlist = value;
        //        this.OnPropertyChanged(() => this.Playlist);
        //    }
        //}
    }
}