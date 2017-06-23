using System;

namespace FCSPlayout.Domain
{
    public class PlayItemAddContext
    {
        public static PlayItemAddContext CreateTimingItemContext(IPlayItem playItem, DateTime startTime, bool isBreak)
        {
            var context = CreateContext(playItem, isBreak ? PlayScheduleMode.TimingBreak : PlayScheduleMode.Timing);
            context.StartTime = startTime;
            return context;
        }

        public static PlayItemAddContext CreateAutoItemContext(IPlayItem playItem, IPlayItem previousPlaylistItem = null)
        {
            var context = CreateContext(playItem, PlayScheduleMode.Auto);
            context.PreviousPlayItem = previousPlaylistItem;
            return context;
        }

        private static PlayItemAddContext CreateContext(IPlayItem playItem, PlayScheduleMode scheduleMode)
        {
            return new PlayItemAddContext
            {
                PlayItem = playItem,
                ScheduleMode = scheduleMode
            };
        }

        private PlayItemAddContext()
        {
        }
        public IPlayItem PlayItem { get; private set; }
        public PlayScheduleMode ScheduleMode { get; private set; }
        public DateTime? StartTime { get; private set; }
        public IPlayItem PreviousPlayItem { get; private set; }
    }
}
