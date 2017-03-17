using FCSPlayout.CG;
using FCSPlayout.Domain;
using System;

namespace FCSPlayout.PlayEngine
{
    public interface IPlayerItem
    {
        CGItemCollection CGItems { get; }
        IMediaSource MediaSource { get; }
        //IPlayParameters PlayParameters { get; }
        PlayRange PlayRange { get; }

        IPlayItem PlayItem { get; }

        TimeSpan Position { get; }

        DateTime ExpectedPlayTime { get; }
        DateTime LoadTime { get; set; }
        IPlayerToken PlayerToken { get; set; }
        DateTime StartTime { get; set; }
        DateTime StopTime { get; set; }
        PlayRange LoadRange { get; }
    }
}
