using System;

namespace FCSPlayout.Domain
{
    public interface IPlayItem
    {
        Guid Id { get; }
        /// <summary>
        /// 获取播放项的开始时间。
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// 获取播放项的播放时长。
        /// </summary>
        TimeSpan PlayDuration { get; }

        /// <summary>
        /// 获取播放项对应的编单项。
        /// </summary>
        IPlaybillItem PlaybillItem { get; }

        long EditId { get; set; }

        /// <summary>
        /// 获取播放项的播放范围。
        /// </summary>
        /// <remarks>
        /// 对于顺播在被后面的定时播截短的情况下，
        /// 该播放范围包含的时长大于<seealso cref="PlayDuration"/>。
        /// </remarks>
        PlayRange PlayRange { get; }
    }

    public static class PlayItemExtensions
    {
        public static DateTime GetStopTime(this IPlayItem playItem)
        {
            return playItem.StartTime.Add(playItem.PlayDuration);
        }
    }
}