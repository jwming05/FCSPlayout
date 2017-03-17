using FCSPlayout.CG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPF.Core
{
    public class EditCGItemsConfirmation : Prism.Interactivity.InteractionRequest.Confirmation
    {
        private CGItemCollection _items = new CGItemCollection();
        public CGItemCollection Items
        {
            get { return _items; }
            set
            {
                _items.Reset(value);
            }
        }
    }
}
