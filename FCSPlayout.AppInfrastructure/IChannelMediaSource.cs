using FCSPlayout.Domain;
using FCSPlayout.Entities;

namespace FCSPlayout.AppInfrastructure
{
    public interface IChannelMediaSource : IMediaSource
    {
        ChannelInfo Channel { get; }
    }
}