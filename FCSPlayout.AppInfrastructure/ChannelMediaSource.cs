using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.AppInfrastructure
{
    [Serializable]
    public class ChannelMediaSource : MediaSourceBase, IChannelMediaSource
    {
        
        public ChannelMediaSource(ChannelInfo channel)
            : base(MediaSourceCategory.External, channel)
        {
            //this.Id = channel.Id;
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

        //public override IMediaSource Clone()
        //{
        //    var result = new ChannelMediaSource(this.Channel);
        //    result.Id = this.Id;
        //    return result;
        //}

        public override bool Equals(IMediaSource other)
        {
            ChannelMediaSource temp = other as ChannelMediaSource;

            return temp != null && temp.Id == this.Id;
        }
    }
}
