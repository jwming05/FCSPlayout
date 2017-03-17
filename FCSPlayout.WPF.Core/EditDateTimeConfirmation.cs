using System;

namespace FCSPlayout.WPF.Core
{
    public class EditDateTimeConfirmation:Prism.Interactivity.InteractionRequest.Confirmation
    {
        public DateTime Time { get; set; }
    }
}
