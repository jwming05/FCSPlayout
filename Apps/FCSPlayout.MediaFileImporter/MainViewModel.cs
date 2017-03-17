using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

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

        //private readonly InteractionRequest<EditDateTimeConfirmation> _editDateTimeInteractionRequest;
        //private readonly InteractionRequest<EditCGItemsConfirmation> _editCGItemsInteractionRequest;

        //private readonly DelegateCommand _collectGarbageCommand;
        //private PeriodicalGarbageCollector _garbageCollector = new PeriodicalGarbageCollector();
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

            //_collectGarbageCommand = new DelegateCommand(ExecuteCollectGarbage);

            _editMediaItemInteractionRequest = new InteractionRequest<EditMediaItemConfirmation>();
            _editDurationInteractionRequest = new InteractionRequest<EditDurationConfirmation>();

            //_editDateTimeInteractionRequest = new InteractionRequest<EditDateTimeConfirmation>();
            //_editCGItemsInteractionRequest = new InteractionRequest<EditCGItemsConfirmation>();
        }

        //private void ExecuteCollectGarbage()
        //{
        //    _garbageCollector.OnTimer();
        //}

        private void RaiseDisplayMessageInteractionRequest(string title, string message)
        {
            _displayMessageInteractionRequest.Raise(new Notification { Title = title, Content = message });
        }


        #region

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
        #endregion

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

        //public ICommand CollectGarbageCommand
        //{
        //    get
        //    {
        //        return _collectGarbageCommand;
        //    }
        //}

        //public InteractionRequest<EditDateTimeConfirmation> EditDateTimeInteractionRequest
        //{
        //    get
        //    {
        //        return _editDateTimeInteractionRequest;
        //    }
        //}

        //public InteractionRequest<EditCGItemsConfirmation> EditCGItemsInteractionRequest
        //{
        //    get
        //    {
        //        return _editCGItemsInteractionRequest;
        //    }
        //}
    }
}