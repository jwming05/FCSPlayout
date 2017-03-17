using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPF.Core
{
    public class AddNullMediaItemConfirmation:Confirmation
    {
        public string Caption
        {
            get;set;
        }

        public TimeSpan Duration
        {
            get;set;
        }
    }
}
