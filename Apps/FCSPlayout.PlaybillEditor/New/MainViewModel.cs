using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System.Windows.Input;

namespace FCSPlayout.PlaybillEditor
{
    public class MainViewModel : ShellModelBase // BindableBase
    {
        private readonly InteractionRequest<EditMediaItemConfirmation> _editMediaItemInteractionRequest;

        //private readonly InteractionRequest<OpenFileDialogConfirmation> _openFileInteractionRequest;
        //private readonly InteractionRequest<SaveFileDialogConfirmation> _saveFileInteractionRequest;

        private readonly DelegateCommand _collectGarbageCommand;
        private PeriodicalGarbageCollector _garbageCollector = new PeriodicalGarbageCollector();

        private readonly InteractionRequest<LoadPlaybillConfirmation> _loadPlaybillInteractionRequest;
        public MainViewModel(IUserService userService, InteractionRequests interactionRequests) 
            :base(userService)
        {
            this.OpenFileInteractionRequest = interactionRequests.OpenFileInteractionRequest;
            this.DisplayMessageInteractionRequest = interactionRequests.DisplayMessageInteractionRequest;
            this.ConfirmationInteractionRequest = interactionRequests.ConfirmationInteractionRequest;
            this.EditDurationInteractionRequest = interactionRequests.EditDurationInteractionRequest;
            this.EditDateTimeInteractionRequest = interactionRequests.EditDateTimeInteractionRequest;
            this.EditCGItemsInteractionRequest = interactionRequests.EditCGItemsInteractionRequest;
            this.SaveFileInteractionRequest = interactionRequests.SaveFileInteractionRequest; 

            //_timer = timer;

            //_playItemCollection = new ObservableCollection<IPlayItem>();
            //_playItemsView = new CollectionView(_playItemCollection);



            //_playbill = new Playbill();
            //_playbill.PlayItemsChanged += Playbill_PlayItemsChanged;

            _collectGarbageCommand = new DelegateCommand(ExecuteCollectGarbage);

            _editMediaItemInteractionRequest = new InteractionRequest<EditMediaItemConfirmation>();
            _loadPlaybillInteractionRequest = new InteractionRequest<LoadPlaybillConfirmation>();
        }

        private void ExecuteCollectGarbage()
        {
            _garbageCollector.OnTimer();
        }

        //private void RaiseDisplayMessageInteractionRequest(string title, string message)
        //{
        //    _displayMessageInteractionRequest.Raise(new Notification { Title = title, Content = message });
        //}

        public IInteractionRequest OpenFileInteractionRequest
        {
            get;private set;
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
            get;private set;
        }
        
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
            get
            {
                return _loadPlaybillInteractionRequest;
            }
        }

        public InteractionRequest<Confirmation> ConfirmationInteractionRequest { get; private set; }
    }
}