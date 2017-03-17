using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.AppInfrastructure
{
    public class ChannelMediaSource : MediaSourceBase, IChannelMediaSource
    {
        
        public ChannelMediaSource(ChannelInfo channel)
            : base(MediaSourceCategory.External, channel)
        {
            this.Id = channel.Id;
            this.Channel = channel;
        }

        public ChannelInfo Channel
        {
            get; private set;
        }


        //public Guid Id
        //{
        //    get; set;
        //}

        public override PlayRange? Adjust(PlayRange playRange)
        {
            return new PlayRange(TimeSpan.Zero, playRange.Duration);
        }
    }
}
