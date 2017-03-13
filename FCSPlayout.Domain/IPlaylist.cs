using System;
using System.Collections.Generic;
namespace FCSPlayout.Domain
{
    public interface IPlaylist
    {
        bool Contains(IPlayItem playItem);

        IPlaylistEditor Edit();


        PlaylistSegment FindLastSegment(Func<PlaylistSegment, bool> predicate);
        PlaylistSegment FindFirstSegment(Func<PlaylistSegment, bool> predicate);

        PlaylistSegment GetNextSegment(PlaylistSegment segment);

        void ValidateTimeRange(DateTime startTime, TimeSpan duration);

        int Count { get; }

        void Update(int index, int length, IList<IPlayItem> newItems);
        IList<IPlayItem> GetPlayItems(int beginIndex, int endIndex);

        event EventHandler<TimeValidationEventArgs> ValidateStartTime;
    }
}
