using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCSPlayout.WPF.Core;
using FCSPlayout.AppInfrastructure;

namespace FCSPlayout.WPFApp
{
    public class PreviewRequestConfirmation : Confirmation
    {
        public PreviewRequestConfirmation(IPlayableItem playableItem)
        {
            this.PlayableItem = playableItem;
        }

        public IPlayableItem PlayableItem { get; private set; }

        public IPlayableItemEditorFactory PlayItemEditorFactory { get; set; }
    }
}
