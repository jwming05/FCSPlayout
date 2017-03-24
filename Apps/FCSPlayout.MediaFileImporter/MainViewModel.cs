using FCSPlayout.AppInfrastructure;

using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace FCSPlayout.MediaFileImporter
{
    public class MainViewModel : BindableBase
    {
       

        private readonly InteractionRequest<EditMediaItemConfirmation> _editMediaItemInteractionRequest;
        private readonly InteractionRequest<EditDurationConfirmation> _editDurationInteractionRequest;

        private readonly InteractionRequest<OpenFileDialogConfirmation> _openFileInteractionRequest;
        private readonly InteractionRequest<Notification> _displayMessageInteractionRequest;
        private readonly InteractionRequest<SaveFileDialogConfirmation> _saveFileInteractionRequest;
        private readonly ITimer _timer;

        private readonly InteractionRequest<EditDateTimeConfirmation> _editDateTimeInteractionRequest;
        private readonly InteractionRequest<EditCGItemsConfirmation> _editCGItemsInteractionRequest;

        //private readonly BindableMediaFileItem _mediaFileChannelId;
        //private Playbill _playbill;
        //private ObservableCollection<IPlayItem> _playItemCollection;
        //private CollectionView _playItemsView;
        //private IPlayItem _selectedPlayItem;

        private readonly DelegateCommand _collectGarbageCommand;
        private PeriodicalGarbageCollector _garbageCollector = new PeriodicalGarbageCollector();
        private InteractionRequest<PlayableItemPreviewNotification> _previewInteractionRequest;

        private DelegateCommand<object> _previewCommand;
        private PlayableItemPreviewNotification _previewNotification;

        public MainViewModel(ITimer timer)
        {

            _openFileInteractionRequest = new InteractionRequest<OpenFileDialogConfirmation>();
            _displayMessageInteractionRequest = new InteractionRequest<Notification>();

            _saveFileInteractionRequest = new InteractionRequest<SaveFileDialogConfirmation>();

            _timer = timer;
           
            //_playItemCollection = new ObservableCollection<IPlayItem>();
            //_playItemsView = new CollectionView(_playItemCollection);



            //_playbill = new Playbill();
            //_playbill.PlayItemsChanged += Playbill_PlayItemsChanged;

            _collectGarbageCommand = new DelegateCommand(ExecuteCollectGarbage);

            _editMediaItemInteractionRequest = new InteractionRequest<EditMediaItemConfirmation>();
            _editDurationInteractionRequest = new InteractionRequest<EditDurationConfirmation>();

            _editDateTimeInteractionRequest = new InteractionRequest<EditDateTimeConfirmation>();
            _editCGItemsInteractionRequest = new InteractionRequest<EditCGItemsConfirmation>();

            _previewInteractionRequest = new InteractionRequest<PlayableItemPreviewNotification>();

            _previewCommand = new DelegateCommand<object>(Preview, CanPreview);
        }

        private void ExecuteCollectGarbage()
        {
            _garbageCollector.OnTimer();
        }

        private void RaiseDisplayMessageInteractionRequest(string title, string message)
        {
            _displayMessageInteractionRequest.Raise(new Notification { Title = title, Content = message });
        }

      
        
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

        public IInteractionRequest OpenFileInteractionRequest
        {
            get
            {
                return _openFileInteractionRequest;
            }
        }


        public InteractionRequest<EditMediaItemConfirmation> EditMediaItemInteractionRequest
        {
            get
            {
                return _editMediaItemInteractionRequest;
            }
        }

        public InteractionRequest<EditDurationConfirmation> EditDurationInteractionRequest
        {
            get
            {
                return _editDurationInteractionRequest;
            }
        }







        //public int? MediaFileChannelce {

        //      get
        //      {


        //          return BindableMediaFileItem.MediaFileChannelce;
        //      }

        //  }
        #endregion

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
            get
            {
                return _displayMessageInteractionRequest;
            }
        }

        public IInteractionRequest SaveFileInteractionRequest
        {
            get
            {
                return _saveFileInteractionRequest;
            }
        }

        public ITimer Timer
        {
            get
            {
                return _timer;
            }
        }

        public ICommand CollectGarbageCommand
        {
            get
            {
                return _collectGarbageCommand;
            }
        }

        public InteractionRequest<EditDateTimeConfirmation> EditDateTimeInteractionRequest
        {
            get
            {
                return _editDateTimeInteractionRequest;
            }
        }

        public InteractionRequest<EditCGItemsConfirmation> EditCGItemsInteractionRequest
        {
            get
            {
                return _editCGItemsInteractionRequest;
            }
        }

        public InteractionRequest<PlayableItemPreviewNotification> PreviewInteractionRequest
        {
            get { return _previewInteractionRequest; }
        }

        public ICommand PreviewCommand
        {
            get { return _previewCommand; }
        }

        internal PlayControl2 Player { get; set; }

        private bool CanPreview(object parameter)
        {
            return (parameter is IPlayableItem) && this.Player!=null;
        }

        private void Preview(object parameter)
        {
            if (CanPreview(parameter))
            {
                this.Player.PlayableItem = (parameter as IPlayableItem);

                //var notification = this.GetPreviewNotification(parameter as IPlayableItem);
                //if (notification != null)
                //{
                //    this.PreviewInteractionRequest.Raise(notification);
                //}
            }
        }

        public PlayableItemPreviewNotification GetPreviewNotification(IPlayableItem playableItem)
        {
            if (playableItem == null)
            {
                return null;
            }

            if (_previewNotification == null)
            {
                _previewNotification = new PlayableItemPreviewNotification();
            }

            if (_previewNotification.PlayableItem != playableItem)
            {
                _previewNotification.PlayableItem = playableItem;
            }
            return _previewNotification;
        }
    }
}