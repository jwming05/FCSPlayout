using FCSPlayout.CG;
using System;

namespace FCSPlayout.Domain
{
    public interface IPlaybillItem
    {
        Guid Id { get; set; }
        /// <summary>
        /// 获取编单项的开始时间，顺播返回<c>null</c>，表示没有指定开始时间。
        /// </summary>
        DateTime? StartTime { get; }

        /// <summary>
        /// 获取播放项的类型，有效类型为顺播、定时播或定时插播。
        /// </summary>
        PlayScheduleMode ScheduleMode { get; }

        /// <summary>
        /// 获取播放源。
        /// </summary>
        //IPlaySource PlaySource { get; }

        /// <summary>
        /// 获取媒体源。
        /// </summary>
        IMediaSource MediaSource { get; }

        /// <summary>
        /// 获取应用于媒体源的入出点。
        /// </summary>
        PlayRange PlayRange { get; }

        //IPlayParameters Parameters { get; }

        string Title { get; }

        CGItemCollection CGItems { get; set; }

        IPlaybillItem Clone(PlayRange newRange);
        bool CanMerge(IPlaybillItem playbillItem);
        IPlaybillItem Merge(IPlaybillItem playbillItem);

        IPlaySource PlaySource { get; }
        //IPlaybillItem Clone();
    }

    public static class PlaybillItemExtensions
    {
        //public static DateTime GetStopTime(this IPlaybillItem playbillItem)
        //{
        //    return playbillItem.StartTime.Value.Add(playbillItem.PlaySource.GetDuration());
        //}

        //public static PlayRange GetPlayRange(this IPlaybillItem playbillItem)
        //{
        //    return playbillItem.PlaySource.PlayRange;
        //}
    }
}
