using FCSPlayout.WPF.Core;
using Prism.Interactivity.InteractionRequest;

namespace FCSPlayout.PlaybillEditor
{
    public class InteractionRequests
    {
        private readonly InteractionRequest<Notification> _displayMessageInteractionRequest=new InteractionRequest<Notification>();
        private readonly InteractionRequest<Confirmation> _confirmationInteractionRequest=new InteractionRequest<Confirmation>();
        private readonly InteractionRequest<EditDurationConfirmation> _editDurationInteractionRequest = new InteractionRequest<EditDurationConfirmation>();
        private readonly InteractionRequest<EditDateTimeConfirmation> _editDateTimeInteractionRequest=new InteractionRequest<EditDateTimeConfirmation>();
        private readonly InteractionRequest<EditCGItemsConfirmation> _editCGItemsInteractionRequest=new InteractionRequest<EditCGItemsConfirmation>();
        private readonly InteractionRequest<OpenFileDialogConfirmation> _openFileInteractionRequest=new InteractionRequest<OpenFileDialogConfirmation>();
        private readonly InteractionRequest<SaveFileDialogConfirmation> _saveFileInteractionRequest=new InteractionRequest<SaveFileDialogConfirmation>();
        private readonly InteractionRequest<LoadPlaybillConfirmation> _loadPlaybillInteractionRequest = new InteractionRequest<LoadPlaybillConfirmation>();
        
        public InteractionRequests()
        {
        }

        public InteractionRequest<EditDurationConfirmation> EditDurationInteractionRequest
        {
            get { return _editDurationInteractionRequest; }
        }

        public InteractionRequest<Notification> DisplayMessageInteractionRequest
        {
            get
            {
                return _displayMessageInteractionRequest;
            }
        }

        public InteractionRequest<Confirmation> ConfirmationInteractionRequest
        {
            get { return _confirmationInteractionRequest; }
        }

        public InteractionRequest<EditDateTimeConfirmation> EditDateTimeInteractionRequest
        { get { return _editDateTimeInteractionRequest; } }

        public InteractionRequest<EditCGItemsConfirmation> EditCGItemsInteractionRequest
        { get { return _editCGItemsInteractionRequest; } }

        public InteractionRequest<OpenFileDialogConfirmation> OpenFileInteractionRequest
        {
            get
            {
                return _openFileInteractionRequest;
            }
        }

        public InteractionRequest<SaveFileDialogConfirmation> SaveFileInteractionRequest
        {
            get
            {
                return _saveFileInteractionRequest;
            }
        }

        public InteractionRequest<LoadPlaybillConfirmation> LoadPlaybillInteractionRequest
        {
            get
            {
                return _loadPlaybillInteractionRequest;
            }
        }
    }
}
