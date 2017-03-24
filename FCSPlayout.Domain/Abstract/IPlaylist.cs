using System;
using System.Collections.Generic;
namespace FCSPlayout.Domain
{
    public interface IPlaylist
    {
        bool Contains(IPlayItem playItem);
        IPlayItem this[int index] { get; }
        IPlaylistEditor Edit();

        int FindLastIndex(Func<IPlayItem, bool> predicate);
        int FindFirstIndex(Func<IPlayItem, bool> predicate);

        int FindLastIndex(int lastStartIndex, Func<IPlayItem, bool> predicate);
        bool IsLocked(IPlayItem playItem);
        int FindFirstIndex(int startIndex, Func<IPlayItem, bool> predicate);

        void ValidateTimeRange(DateTime startTime, TimeSpan duration);
        void ValidateTimeRange(DateTime startTime, TimeSpan duration,IPlayItem excludeItem);

        int Count { get; }

        void Update(int index, int length, IList<IPlayItem> newItems);
        IList<IPlayItem> GetPlayItems(int beginIndex, int endIndex);
        bool CanClear();

        event EventHandler<TimeValidationEventArgs> ValidateStartTime;

        void Clear();
        //void Append(IList<IPlayItem> playItems);

        bool CanDelete(IPlayItem playItem);

        bool EditLocked(IPlayItem playItem);
        DateTime? GetStartTime();
    }
}
