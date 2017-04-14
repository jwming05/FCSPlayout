using System;
using System.Collections.Generic;

namespace FCSPlayout.Domain
{
    public interface IPlaylistEditor : IDisposable
    {
        void AddAuto(IPlayItem playItem);
        void AddTiming(IPlayItem playItem);
        void AddAuto(IPlayItem prevItem, IPlayItem newItem);

        //void AddAutoAfter(IPlayItem prevItem, AutoPlaybillItem newItem);
        //void InsertTiming(IPlayItem playItem);
        void ClearAll();
        //void Append(IList<IPlayItem> playItems);
        void Delete(IPlayItem playItem);

        event EventHandler Disposed;

        void ChangeMediaSource(IPlayItem playItem, IMediaSource mediaSource, PlayRange playRange);
        void ChangeMediaSource(IPlayItem playItem, IMediaSource mediaSource);
        void MoveUp(IPlayItem playItem);
        void MoveDown(IPlayItem playItem);
        void ChangeStartTime(IPlayItem playItem, DateTime startTime);
        void ChangePlayRange(IPlayItem playItem, PlayRange newRange);
        void ChangeSchedule(IPlayItem playItem, PlayScheduleMode scheduleMode, DateTime? startTime);
        void Reorder(IPlayItem newPrevItem, IPlayItem reorderItem);
    }
}
