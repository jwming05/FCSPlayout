using FCSPlayout.Domain;
using System;

namespace FCSPlayout.Domain
{
    public interface IPlaylist2:IPlaylist
    {
        TimeSpan Duration { get; }
        DateTime? StartTime { get; }
        IPlayItem NextItem { get; set; }
        IPlayItem CurrentItem { get; set; }

        IPlayItem GetNextPlayItem(DateTime minStartTime, DateTime maxStartTime);
        bool CanEnterLoop(IPlayItem playItem);
        void OnTimer();
        bool CanForcePlay(IPlayItem playItem);
        void ForcePlay(IPlayItem playItem);
        void Start();
        void Stop();
    }

    // 播放列表：
    // 未启动的情况下：（起始时间，结束时间，总时长）
    // 已启动的情况下：未开播（起始时间，结束时间，总时长）开播倒计时
    //                 已开播（开播时间，当前位置，剩余时间，）
}