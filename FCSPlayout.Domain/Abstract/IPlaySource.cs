using FCSPlayout.CG;
using System;
namespace FCSPlayout.Domain
{
    public interface IPlaySource
    {
        /// <summary>
        /// 获取媒体源。
        /// </summary>
        IMediaSource MediaSource { get; }

        /// <summary>
        /// 获取应用于媒体源的入出点。
        /// </summary>
        PlayRange PlayRange { get; set; }

        //IPlayParameters Parameters { get; }

        string Title { get; }

        CGItemCollection CGItems { get; set; }

        IPlaySource Clone();
    }

    public static class PlaySourceExtensions
    {
        /// <summary>
        /// 获取播放源的时长。
        /// </summary>
        /// <param name="playSource"></param>
        /// <returns></returns>
        public static TimeSpan GetDuration(this IPlaySource playSource)
        {
            return playSource.PlayRange == null ?
                playSource.MediaSource.Duration.Value : playSource.PlayRange.Duration;
        }
    }
}