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

        protected PlaylistBuilder Builder { get { return _builder; } }

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

        public void Delete(IPlayItem playItem)
        {
            if (_playlist.CanDelete(playItem))
            {
                int prevIndex = _playlist.FindLastIndex(i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && i.StartTime < playItem.StartTime);
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

                int nextIndex = _playlist.FindFirstIndex(beginIndex, i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && i.StartTime > playItem.StartTime);

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
                playItems.Remove(playItem);
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

                IList<IPlayItem> newPlayItems = _builder.Build(data);
                _playlist.Update(beginIndex, endIndex - beginIndex + 1, newPlayItems);
            }
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
            if (_playlist.Count > 0)
            {
                _playlist.Clear();
            }
        }

        public void Append(IList<IPlayItem> playItems)
        {
            if (playItems.Count > 0)
            {
                _playlist.Append(playItems);
            }
        }

        /// <summary>
        /// 定时播改为定时插播；
        /// 或者，定时插播改为定时播；
        /// 或者，只更改定时播或定时插播的开始时间。
        /// </summary>
        /// <param name="playItem"></param>
        /// <param name="newScheduleMode"></param>
        /// <param name="newStartTime"></param>
        public void ChangeTimingItem(IPlayItem playItem,PlayScheduleMode newScheduleMode,DateTime newStartTime)
        {
            TimingPlaybillItem current = (TimingPlaybillItem)playItem;

            if (newScheduleMode==playItem.PlaybillItem.ScheduleMode && newStartTime == playItem.StartTime)
            {
                return;
            }

            if (_playlist.CanDelete(playItem))
            {
                // 验证时间范围（包含开始时间验证）。
                _playlist.ValidateTimeRange(newStartTime, playItem.PlayDuration, playItem);

                var newItem = newScheduleMode == PlayScheduleMode.Timing ?
                    PlaybillItem.Timing(playItem.PlaybillItem.PlaySource, newStartTime) :
                    PlaybillItem.TimingBreak(playItem.PlaybillItem.PlaySource, newStartTime);

                this.Delete(playItem);
                this.InsertTiming(newItem);
            }
        }

        public void ChangeSource(IPlayItem playItem,IMediaSource mediaSource,PlayRange? newPlayRange)
        {
            if (mediaSource == null && newPlayRange == null) return;
            if (mediaSource == null && newPlayRange.Value == playItem.PlayRange) return;

            if (_playlist.CanDelete(playItem))
            {
                PlayRange playRange=newPlayRange ?? new PlayRange(TimeSpan.Zero,playItem.PlayDuration);

                if(playItem.PlaybillItem.ScheduleMode== PlayScheduleMode.Timing || playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak)
                {
                    _playlist.ValidateTimeRange(playItem.StartTime, playRange.Duration, playItem);
                }


                if (mediaSource == null)
                {
                    mediaSource = playItem.PlaybillItem.PlaySource.MediaSource;
                }

                var playsource = new PlaySource(new MediaItem(mediaSource, playRange));
                playsource.CGItems = playItem.PlaybillItem.PlaySource.CGItems;

                IPlayItem newPlayItem = null;
                switch (playItem.PlaybillItem.ScheduleMode)
                {
                    case PlayScheduleMode.Timing:
                        this.Delete(playItem);
                        this.InsertTiming(PlaybillItem.Timing(playsource, playItem.StartTime));
                        break;
                    case PlayScheduleMode.TimingBreak:
                        this.Delete(playItem);
                        this.InsertTiming(PlaybillItem.TimingBreak(playsource, playItem.StartTime));
                        break;
                    case PlayScheduleMode.Auto:
                        newPlayItem =new AutoPlayItem(PlaybillItem.Auto(playsource));
                        break;
                }


                
                //// 验证时间范围（包含开始时间验证）。
                //_playlist.ValidateTimeRange(newStartTime, playItem.PlayDuration, playItem);

                //var newItem = newScheduleMode == PlayScheduleMode.Timing ?
                //    PlaybillItem.Timing(playItem.PlaybillItem.PlaySource, newStartTime) :
                //    PlaybillItem.TimingBreak(playItem.PlaybillItem.PlaySource, newStartTime);

                //this.Delete(playItem);
                //this.InsertTiming(newItem);
            }
        }

        public void ChangeTimingToAuto(IPlayItem playItem)
        {
            TimingPlaybillItem current = (TimingPlaybillItem)playItem;

            Debug.Assert(current.ScheduleMode == PlayScheduleMode.Timing);

            if (_playlist.CanDelete(current))
            {
                int prevIndex = _playlist.FindLastIndex(i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && i.StartTime < playItem.StartTime);
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

                int currentIndex= _playlist.FindFirstIndex(beginIndex,i => i==playItem);
                IPlayItem prevItem = null;
                if (currentIndex > 0)
                {
                    prevItem = _playlist[currentIndex - 1];
                }

                int nextIndex = _playlist.FindFirstIndex(beginIndex, i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && i.StartTime>playItem.StartTime);

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
                playItems.Remove(playItem);

                var newAutoItem =new AutoPlayItem(PlaybillItem.Auto(playItem.PlaybillItem.PlaySource));
                
                PlaylistBuildData data = new PlaylistBuildData(this.Id);
                data.StartTime = startTime;
                data.StopTime = stopTime;

                if (prevItem == null)
                {
                    data.AddAuto(newAutoItem);
                }

                for (int i = 0; i < playItems.Count; i++)
                {
                    var item = playItems[i];
                    if (item.PlaybillItem.ScheduleMode == PlayScheduleMode.Auto)
                    {
                        data.AddAuto((AutoPlayItem)item);
                        if (prevItem!=null && item == prevItem)
                        {
                            data.AddAuto(newAutoItem);
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
        }

        public event EventHandler Disposed;
    }
}
