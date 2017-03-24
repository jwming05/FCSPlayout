using System;
using System.Collections.Generic;

namespace FCSPlayout.Domain
{
    public class PlaylistEditor2:PlaylistEditor,IPlaylistEditor2
    {
        private IPlaylist2 _playlist;
        public PlaylistEditor2(IPlaylist2 playlist)
            :base(playlist)
        {
            _playlist = playlist;
        }

        public bool ForcePlay(IPlayItem playItem,DateTime startTime)
        {
            if (!_playlist.CanDelete(playItem)) return false;

            var currentPlayItem = _playlist.CurrentItem;
            //var startTime = DefaultDateTimeService.Instance.GetLocalNow().AddSeconds(1.0);
            var remainDuration = currentPlayItem.CalculatedStopTime.Subtract(startTime);

            

            var adusted = TimeSpan.FromSeconds(0.2);
            if (currentPlayItem.CalculatedPlayDuration - remainDuration >= adusted)
            {
                remainDuration = remainDuration + adusted;
            }
            else
            {
                remainDuration = currentPlayItem.CalculatedPlayDuration;
            }

            if (remainDuration < PlayoutConfiguration.Current.MinPlayDuration) return false;

            DateTime stopTime;
            var endIndex = _playlist.FindFirstIndex(i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing);
            if (endIndex == -1)
            {
                endIndex = _playlist.Count - 1;
                stopTime = DateTime.MaxValue;
            }
            else
            {
                stopTime = _playlist[endIndex].StartTime;
                endIndex = endIndex - 1;
            }

            if (stopTime.Subtract(startTime) <PlayoutConfiguration.Current.MinPlayDuration) return false;

            var deleteIndex = _playlist.FindFirstIndex(i=>i==playItem);

            bool deleted = false;
            if(deleteIndex > endIndex)
            {
                this.Delete(playItem);
                deleted = true;
            }

            var beginIndex = 0;
            IList<IPlayItem> playItems = _playlist.GetPlayItems(beginIndex, endIndex);
            if (!deleted)
            {
                playItems.Remove(playItem);
            }

            //this.Delete(playItem);

            IPlaySource playSource = ((PlaybillItem)currentPlayItem.PlaybillItem).PlaySource.Clone();

            var temp = new PlayRange(currentPlayItem.PlayRange.StartPosition+currentPlayItem.CalculatedPlayDuration - remainDuration, remainDuration);

            var newAutoItem = new AutoPlayItem(PlaybillItem.Auto(playSource));

            PlaylistBuildData data = new PlaylistBuildData(this.Id);
            data.StartTime = startTime;
            data.StopTime = stopTime;
            data.AddAuto((AutoPlayItem)playItem);
            data.AddAuto(newAutoItem);

            for (int i = 0; i < playItems.Count; i++)
            {
                var item = playItems[i];
                if (item.PlaybillItem.ScheduleMode == PlayScheduleMode.Auto)
                {
                    data.AddAuto((AutoPlayItem)item);
                }
                else
                {
                    data.InsertTiming((TimingPlaybillItem)item);
                }
            }

            IList<IPlayItem> newPlayItems = Builder.Build(data);

            _playlist.Update(beginIndex, endIndex - beginIndex + 1, newPlayItems);

            return true;
        }
    }
}
