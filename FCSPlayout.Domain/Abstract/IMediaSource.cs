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

        PlayRange? Adjust(PlayRange playRange);

        MediaSourceCategory Category { get; }
        string Title { get; }
        Guid Id { get; }

        //IMediaSource Clone();

        bool Equals(IMediaSource other);
    }

    

    public static class MediaSourceExtensions
    {
        internal static void ValidatePlayRange(this IMediaSource mediaSource,PlayRange playRange)
        {
            if (mediaSource.Duration != null)
            {
                if(playRange.StartPosition>mediaSource.Duration.Value || playRange.StopPosition > mediaSource.Duration.Value)
                {
                    throw new InvalidPlayRangeException(playRange);
                }
            }
        }
    }
}