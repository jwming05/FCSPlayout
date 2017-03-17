using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlaylistEditor : IPlaylistEditor
    {
        private IPlaylist _playlist;
        private PlaylistBuilder _builder=new PlaylistBuilder();

        public long Id { get; private set; }

        public PlaylistEditor(IPlaylist playlist)
        {
            this.Id = DateTime.Now.Ticks;
            _playlist = playlist;
        }

        public void AddAutoAfter(IPlayItem prevItem,AutoPlaybillItem newItem)
        {
            if (!_playlist.Contains(prevItem))
            {
                throw new ArgumentException();
            }

            // 不能在定时插播后插入一条顺播。
            if (prevItem.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak)
            {
                throw new ArgumentException();
            }

            int prevIndex;
            if (prevItem.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing)
            {
                prevIndex = _playlist.FindLastIndex(i => i==prevItem);
            }
            else // (prevItem.PlaybillItem.Category == PlaybillItemCategory.Auto)
            {
                prevIndex = _playlist.FindLastIndex(i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && i.StartTime < prevItem.StartTime);
            }

            DateTime startTime, stopTime;
            int beginIndex, endIndex;

            if (prevIndex == -1)
            {
                startTime = _playlist[0].StartTime;
                beginIndex = 0;
            }
            else
            {
                startTime = _playlist[prevIndex].GetStopTime();
                beginIndex = prevIndex + 1;
            }

            int nextIndex = _playlist.FindFirstIndex(beginIndex, i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing);

            if (nextIndex == -1)
            {
                stopTime = DateTime.MaxValue;
                endIndex = _playlist.Count - 1;
            }
            else
            {
                stopTime = _playlist[nextIndex].StartTime;
                endIndex = nextIndex - 1;
            }

            IList<IPlayItem> playItems = _playlist.GetPlayItems(beginIndex, endIndex);

            PlaylistBuildData data = new PlaylistBuildData(this.Id);
            data.StartTime = startTime;
            data.StopTime = stopTime;

            var newAutoPlayItem = new AutoPlayItem(newItem);

            if (prevItem.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing)
            {
                data.AddAuto(newAutoPlayItem);
            }

            for (int i = 0; i < playItems.Count; i++)
            {
                var item = playItems[i];
                if (item.PlaybillItem.ScheduleMode == PlayScheduleMode.Auto)
                {
                    data.AddAuto((AutoPlayItem)item);
                    if (item == prevItem)
                    {
                        data.AddAuto(newAutoPlayItem);
                    }
                }
                else
                {
                    data.InsertTiming((TimingPlaybillItem)item);
                }
            }

            IList<IPlayItem> newPlayItems = _builder.Build(data);
            _playlist.Update(beginIndex, endIndex - beginIndex + 1, newPlayItems);
        }

        public void InsertTiming(PlaybillItem playbillItem)
        {
            // 验证时间范围（包含开始时间验证）。
            _playlist.ValidateTimeRange(playbillItem.StartTime.Value, playbillItem.PlaySource.GetDuration());

            int prevIndex = _playlist.FindLastIndex(i =>i.PlaybillItem.ScheduleMode==PlayScheduleMode.Timing && i.StartTime < playbillItem.StartTime.Value);
            DateTime startTime, stopTime;
            int beginIndex, endIndex;

            if (prevIndex == -1)
            {
                startTime = playbillItem.StartTime.Value;
                if (_playlist.Count > 0 && _playlist[0].StartTime<startTime)
                {
                    startTime = _playlist[0].StartTime;
                }
                beginIndex = 0;
            }
            else
            {
                startTime = _playlist[prevIndex].GetStopTime();
                beginIndex = prevIndex + 1;
            }

            int nextIndex = _playlist.FindFirstIndex(beginIndex, i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing);

            if (nextIndex == -1)
            {
                stopTime = DateTime.MaxValue;
                endIndex = _playlist.Count - 1;
            }
            else
            {
                stopTime = _playlist[nextIndex].StartTime;
                endIndex = nextIndex - 1;
            }

            IList<IPlayItem> playItems = _playlist.GetPlayItems(beginIndex, endIndex);

            PlaylistBuildData data = new PlaylistBuildData(this.Id);
            data.StartTime = startTime;
            data.StopTime = stopTime;
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
            ((TimingPlaybillItem)playbillItem).EditId = this.Id;

            data.InsertTiming((TimingPlaybillItem)playbillItem);

            IList<IPlayItem> newPlayItems = _builder.Build(data);

            _playlist.Update(beginIndex, endIndex - beginIndex + 1, newPlayItems);
        }

        public void Dispose()
        {
            if (this.Disposed != null)
            {
                this.Disposed(this, EventArgs.Empty);
            }
        }

        public void ClearAll()
        {
            _playlist.Clear();
        }

        public void Append(IList<IPlayItem> playItems)
        {
            _playlist.Append(playItems);
        }

        public event EventHandler Disposed;
    }
}
