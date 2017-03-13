using System;

namespace FCSPlayout.Domain
{
    public interface IPlaybillItem
    {
        /// <summary>
        /// 获取编单项的开始时间，顺播返回<c>null</c>，表示没有指定开始时间。
        /// </summary>
        DateTime? StartTime { get; }

        /// <summary>
        /// 获取播放项的类型，有效类型为顺播、定时播或定时插播。
        /// </summary>
        PlaybillItemCategory Category { get; }

        /// <summary>
        /// 获取播放源。
        /// </summary>
        IPlaySource PlaySource { get; }
    }

    public static class PlaybillItemExtensions
    {
        public static DateTime GetStopTime(this IPlaybillItem playbillItem)
        {
            return playbillItem.StartTime.Value.Add(playbillItem.PlaySource.GetDuration());
        }

        public static PlayRange GetPlayRange(this IPlaybillItem playbillItem)
        {
            return playbillItem.PlaySource.GetPlayRange();
        }
    }
}
