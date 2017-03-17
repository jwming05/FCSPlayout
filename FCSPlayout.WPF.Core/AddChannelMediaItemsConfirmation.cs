//using FCSPlayout.Domain.Entities;
using FCSPlayout.Entities;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPF.Core
{
    public class AddChannelMediaItemsConfirmation : Confirmation
    {
        public AddChannelMediaItemsConfirmation()
        {
            this.Channels =new List<ChannelInfo>(PlayoutRepository.GetChannelInfos(true));
        }

        public IEnumerable<ChannelInfo> Channels { get; private set; }

        public ChannelInfo SelectedChannel
        { get; set; }
    }
}
