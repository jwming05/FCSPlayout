﻿using FCSPlayout.CG;
using System;

namespace FCSPlayout.Domain
{
    public interface IPlayItem
    {
        Guid Id { get; }

        /// <summary>
        /// 获取播放项的开始时间。
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// 获取播放项的播放时长。
        /// </summary>
        TimeSpan CalculatedPlayDuration { get; set; }

        /// <summary>
        /// 获取播放项对应的编单项。
        /// </summary>
        IPlaybillItem PlaybillItem { get; }

        /// <summary>
        /// 获取播放项的播放范围。
        /// </summary>
        /// <remarks>
        /// 对于顺播在被后面的定时播截短的情况下，
        /// 该播放范围包含的时长大于<seealso cref="CalculatedPlayDuration"/>。
        /// </remarks>
        PlayRange PlayRange { get; }

        PlayRange CalculatedPlayRange { get; }

        DateTime CalculatedStopTime { get; }

        /// <summary>
        /// 获取播放项的类型，有效类型为顺播、定时播或定时插播。
        /// </summary>
        PlayScheduleMode ScheduleMode { get; }

        /// <summary>
        /// 获取媒体源。
        /// </summary>
        IMediaSource MediaSource { get; }

        string Title { get; }

        CGItemCollection CGItems { get; set; }
        IPlaylistEditor Editor { get; set; }

        void Split(TimeSpan duration, out IPlayItem first, out IPlayItem second);
    }

    public static class PlayItemExtensions
    {
        public static bool IsAutoPadding(this IPlayItem playItem)
        {
            var autoItem = playItem as AutoPlayItem;
            if (autoItem != null) return autoItem.IsAutoPadding;
            return false;
        }
    }
}