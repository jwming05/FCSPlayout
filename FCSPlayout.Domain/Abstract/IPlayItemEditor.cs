using System;

namespace FCSPlayout.Domain
{
    public interface IPlayItemEditor
    {
        PlaylistEditResult ChangeSchedule(PlayScheduleMode scheduleMode, DateTime startTime);

        PlaylistEditResult ChangePlayRange(PlayRange newRange);

        PlaylistEditResult ChangeStartTime(DateTime startTime);

        /// <summary>
        /// 等长替换媒体源。
        /// </summary>
        /// <param name="mediaSource">指定用于播放项的新媒体源。</param>
        /// <returns></returns>

        PlaylistEditResult ChangeMediaSource(IMediaSource mediaSource);

        /// <summary>
        /// 变长替换媒体源。
        /// </summary>
        /// <param name="mediaItem"></param>
        /// <returns></returns>
        PlaylistEditResult ChangeMediaItem(MediaItem mediaItem);

        PlaylistEditResult MoveDown();

        PlaylistEditResult MoveUp();
    }
}