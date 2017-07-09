using System;

namespace FCSPlayout.Domain
{
    public interface IPlaylistBuildOption
    {
        /// <summary>
        /// 指示第一个播放项是否必须是定时播。
        /// </summary>
        bool RequireFirstTimingItem { get; }

        //DateTime MinStartTime { get; }
        //DateTime MaxStopTime { get; }

        TimeRange PlayTimeRange { get; }

        TimeSpan MinPlayDuration { get; }
    }
}
