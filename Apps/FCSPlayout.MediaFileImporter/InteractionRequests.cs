using FCSPlayout.WPF.Core;
using Prism.Interactivity.InteractionRequest;

namespace FCSPlayout.MediaFileImporter
{
    public class InteractionRequests
    {
        private readonly InteractionRequest<OpenFileDialogConfirmation>  _openFileInteractionRequest = new InteractionRequest<OpenFileDialogConfirmation>();
        private readonly InteractionRequest<LoginConfirmation> _loginInteractionRequest=new InteractionRequest<LoginConfirmation>();
        private readonly InteractionRequest<Notification> _displayMessageInteractionRequest=new InteractionRequest<Notification>();

        //private readonly InteractionRequest<SaveFileDialogConfirmation> _saveFileInteractionRequest=new InteractionRequest<SaveFileDialogConfirmation>();
        public InteractionRequests()
        {
        }

        public InteractionRequest<OpenFileDialogConfirmation> OpenFileInteractionRequest
        {
            get
            {
                return _openFileInteractionRequest;
            }
        }

        public InteractionRequest<LoginConfirmation> LoginInteractionRequest
        {
            get
            {
                return _loginInteractionRequest;
            }
        }

        public InteractionRequest<Notification> DisplayMessageInteractionRequest
        {
            get
            {
                return _displayMessageInteractionRequest;
            }
        }
    }
}
