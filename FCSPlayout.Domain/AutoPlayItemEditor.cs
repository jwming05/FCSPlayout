using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    //class AutoPlayItemEditor : IPlayItemEditor
    //{
    //    private AutoPlayItem _autoPlayItem;
    //    //private IPlaylistEditor _playlistEditor;
    //    private IPlaylist _playlist;

    //    public PlaylistEditResult ChangeMediaItem(MediaItem mediaItem)
    //    {
    //        if (!_playlist.IsLocked(_autoPlayItem))
    //        {
    //            using(var editor = _playlist.Edit())
    //            {
    //                editor.ChangeMediaItem(_autoPlayItem, mediaItem);
    //            }
    //        }
    //        throw new NotImplementedException();
    //    }

    //    public PlaylistEditResult ChangeMediaSource(IMediaSource mediaSource)
    //    {
    //        using (var editor = _playlist.Edit())
    //        {
    //            editor.ChangeMediaSource(_autoPlayItem, mediaSource);
    //        }
    //        throw new NotImplementedException();
    //    }

    //    public PlaylistEditResult ChangePlayRange(PlayRange newRange)
    //    {
    //        using (var editor = _playlist.Edit())
    //        {
    //            editor.ChangePlayRange(_autoPlayItem, newRange);
    //        }
    //        throw new NotImplementedException();
    //    }

    //    public PlaylistEditResult ChangeSchedule(PlayScheduleMode scheduleMode, DateTime startTime)
    //    {
    //        using (var editor = _playlist.Edit())
    //        {
    //            editor.ChangeSchedule(_autoPlayItem, scheduleMode, startTime);
    //        }
    //        throw new NotImplementedException();
    //    }

    //    //public PlaylistEditResult ChangeSchedule(PlayScheduleMode scheduleMode, DateTime startTime)
    //    //{
    //    //    using (var editor = _playlist.Edit())
    //    //    {
    //    //        editor.ChangeSchedule(_autoPlayItem, scheduleMode, startTime);
    //    //    }

    //    //}

    //    public void ChangeCG(CG.CGItemCollection cgItems)
    //    {
    //        //using (var editor = _playlist.Edit())
    //        //{
    //        //    editor.ChangeCG(_autoPlayItem, cgItems);
    //        //}
    //    }

    //    //public void ChangePlayParameters(IPlayParameters playParameters)
    //    //{
    //    //    //using (var editor = _playlist.Edit())
    //    //    //{
    //    //    //    editor.ChangeCG(_autoPlayItem, cgItems);
    //    //    //}
    //    //}

    //    public PlaylistEditResult ChangeStartTime(DateTime startTime)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public PlaylistEditResult MoveDown()
    //    {
    //        using (var editor = _playlist.Edit())
    //        {
    //            editor.MoveDown(_autoPlayItem);
    //        }
    //        throw new NotImplementedException();
    //    }

    //    public PlaylistEditResult MoveUp()
    //    {
    //        using (var editor = _playlist.Edit())
    //        {
    //            editor.MoveUp(_autoPlayItem);
    //        }
    //        throw new NotImplementedException();
    //    }
    //}
}
