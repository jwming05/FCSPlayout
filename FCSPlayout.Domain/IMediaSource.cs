using System;

namespace FCSPlayout.Domain
{
    public interface IMediaSource
    {
        /// <summary>
        /// 获取媒体源的时间。
        /// <c>null</c>表示该媒体源没有固定时长。
        /// </summary>
        TimeSpan? Duration { get; }
    }

    public static class MediaSourceExtensions
    {
        public static PlayRange GetPlayRange(this IMediaSource mediaSource)
        {
            return new PlayRange(TimeSpan.Zero, mediaSource.Duration.Value);
        }
    }
}