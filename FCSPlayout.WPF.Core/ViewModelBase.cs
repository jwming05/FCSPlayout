using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace FCSPlayout.WPF.Core
{
    public abstract class ViewModelBase : BindableBase
    {
        
        public ViewModelBase()
        {
            
        }

        public InteractionRequest<EditMediaItemConfirmation> EditMediaItemInteractionRequest { get; protected internal set; }

        //public InteractionRequest<EditDurationConfirmation> EditDurationInteractionRequest { get; protected internal set; }

        public InteractionRequest<OpenFileDialogConfirmation> OpenFileInteractionRequest { get; protected internal set; }
        public InteractionRequest<SaveFileDialogConfirmation> SaveFileInteractionRequest { get; protected internal set; }
    }
}
