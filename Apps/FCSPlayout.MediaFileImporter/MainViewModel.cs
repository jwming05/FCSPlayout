using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Interactivity.InteractionRequest;

namespace FCSPlayout.MediaFileImporter
{
    public class MainViewModel :ShellModelBase
    {
        public MainViewModel(IUserService userService, InteractionRequests requests)
            :base(userService)
        {
            this.OpenFileInteractionRequest = requests.OpenFileInteractionRequest;
            this.DisplayMessageInteractionRequest = requests.DisplayMessageInteractionRequest;
            this.SaveFileInteractionRequest=requests.SaveFileInteractionRequest;
        }

        public IInteractionRequest OpenFileInteractionRequest
        {
            get;private set;
        }

        public IInteractionRequest DisplayMessageInteractionRequest
        {
            get;private set;
        }

        public IInteractionRequest SaveFileInteractionRequest
        {
            get; private set;
        }
    }
}