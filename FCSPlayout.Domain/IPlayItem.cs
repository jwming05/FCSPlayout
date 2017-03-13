using System;

namespace FCSPlayout.Domain
{
    public interface IPlayItem
    {
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
    }

    public static class PlayItemExtensions
    {
        public static DateTime GetStopTime(this IPlayItem playItem)
        {
            return playItem.StartTime.Add(playItem.PlayDuration);
        }
    }
}