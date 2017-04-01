using Prism.Interactivity.InteractionRequest;
using System;

namespace FCSPlayout.WPF.Core
{
    public class LoginConfirmation:Confirmation
    {
        public Func<string, string, bool> LoginAction
        {
            get;
            set;
        }
    }
}
