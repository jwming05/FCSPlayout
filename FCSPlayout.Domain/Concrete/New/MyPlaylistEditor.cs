using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public partial class MyPlaylistEditor : IPlaylistEditor
    {
        private List<ScheduleItem> _playlist;
        private NewPlaylistBuilder _builder=new NewPlaylistBuilder();
        private IList<IPlayItem> _originalPlayItems;
        private bool _cancelCommit = false;
        private Action _onCommitted;

        protected NewPlaylistBuilder Builder { get { return _builder; } }

        public long Id { get; private set; }

        public MyPlaylistEditor(IList<IPlayItem> playItems,Action onCommitted)
        {
            this.Id = DateTime.Now.Ticks;
            _originalPlayItems = playItems;

            
            _playlist = _originalPlayItems.Select(i=>new ScheduleItem(i)).ToList();

            _onCommitted = onCommitted;
        }
        
        public void Dispose()
        {
            this.Commit();
            if (this.Disposed != null)
            {
                this.Disposed(this, EventArgs.Empty);
            }
        }

        public void ClearAll()
        {
            throw new NotImplementedException();
            //if (_playlist.CanClear() && _playlist.Count > 0)
            //{
            //    _playlist.Clear();
            //}
        }

        public void Append(IList<ScheduleItem> playItems)
        {
            if (playItems.Count == 0) return;

            var itemStartTime = playItems[0].StartTime;

            var prevTuple = this.FindLastTiming(i => i.StartTime <= itemStartTime);

            int beginIndex;

            if (prevTuple.Item2 == -1)
            {
                beginIndex = 0;
            }
            else
            {
                beginIndex = prevTuple.Item2;
            }

            while (beginIndex < _playlist.Count)
            {
                if (_playlist[beginIndex].StartTime >= itemStartTime)
                {
                    break;
                }
                beginIndex++;
            }


            if (beginIndex == _playlist.Count)
            {
                if (_playlist.Count == 0)
                {
                    var playlistStartTime = GetPlaylistStartTime(); //  _playlist.GetStartTime();
                    if (playlistStartTime != null)
                    {
                        TimeSpan duration = itemStartTime.Subtract(playlistStartTime.Value);
                        if (duration > TimeSpan.Zero)
                        {
                            var autoPadding =new ScheduleItem(AutoPlayItem.CreateAutoPadding(playlistStartTime.Value, duration));
                            var newItems = new List<ScheduleItem>();
                            newItems.Add(autoPadding);
                            newItems.AddRange(playItems);

                            playItems = newItems;
                        }
                        else
                        {

                        }
                    }
                    else //playlistStartTime == null
                    {

                    }
                }
                else
                {
                    var lastStopTime = _playlist[_playlist.Count - 1].CalculatedStopTime;
                    TimeSpan duration = itemStartTime.Subtract(lastStopTime);

                    if (duration > TimeSpan.Zero)
                    {
                        var autoPadding =new ScheduleItem(AutoPlayItem.CreateAutoPadding(lastStopTime, duration));
                        var newItems = new List<ScheduleItem>();
                        newItems.Add(autoPadding);
                        newItems.AddRange(playItems);

                        playItems = newItems;
                    }
                    else
                    {

                    }
                }
            }

            //_playlist.Update(beginIndex, _playlist.Count - beginIndex, playItems);
            UpdatePlaylist(beginIndex, _playlist.Count - beginIndex, playItems);
        }

        public event EventHandler Disposed;

        #region
        private PlaylistSegment FindSegment(IPlayItem playItem)
        {
            if (_playlist.Count == 0)
            {
                throw new InvalidOperationException();
            }

            //int currentIndex = _playlist.FindFirstIndex(i => i == playItem);
            int currentIndex = _playlist.FindIndex(i => i.PlayItem == playItem);
            if (currentIndex == -1)
            {
                throw new InvalidOperationException();
            }

            int prevIndex = _playlist.FindLastIndex(currentIndex, i => i.ScheduleMode == PlayScheduleMode.Timing && i.StartTime <= playItem.StartTime);

            PlaylistSegment segment = null;
            if (prevIndex == -1)
            {
                segment = new PlaylistSegment(_playlist[0].StartTime)
                {
                    BeginIndex = 0
                };
            }
            else
            {
                segment = new PlaylistSegment(_playlist[prevIndex])
                {
                    BeginIndex = prevIndex
                };
            }

            //int nextIndex = _playlist.FindFirstIndex(currentIndex + 1, i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && i.StartTime > playItem.StartTime);
            int nextIndex = _playlist.FindIndex(currentIndex + 1, i => i.ScheduleMode == PlayScheduleMode.Timing && i.StartTime > playItem.StartTime);
            if (nextIndex == -1)
            {
                segment.EndIndex = _playlist.Count - 1;
                segment.StopTime = DateTime.MaxValue;
            }
            else
            {
                segment.EndIndex = nextIndex - 1;
                segment.StopTime = _playlist[nextIndex].StartTime;
            }

            return segment;
        }

        private Tuple<ScheduleItem, int> FindFirstTiming(Func<ScheduleItem, bool> predicate)
        {
            //int index = _playlist.FindFirstIndex(i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && predicate(i));
            int index = _playlist.FindIndex(i => i.ScheduleMode == PlayScheduleMode.Timing && predicate(i));
            return new Tuple<ScheduleItem, int>(index == -1 ? null : _playlist[index], index);
        }

        private Tuple<ScheduleItem, int> FindFirstTiming(int startIndex, Func<ScheduleItem, bool> predicate)
        {
            //int index = _playlist.FindFirstIndex(startIndex, i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && predicate(i));
            int index = _playlist.FindIndex(startIndex, i => i.ScheduleMode == PlayScheduleMode.Timing && predicate(i));
            return new Tuple<ScheduleItem, int>(index == -1 ? null : _playlist[index], index);
        }

        private Tuple<ScheduleItem, int> FindLastTiming(Func<ScheduleItem, bool> predicate)
        {
            int index = _playlist.FindLastIndex(i => i.ScheduleMode == PlayScheduleMode.Timing && predicate(i));
            return new Tuple<ScheduleItem, int>(index == -1 ? null : _playlist[index], index);
        }

        private Tuple<ScheduleItem, int> FindLastTiming(int startLastIndex, Func<ScheduleItem, bool> predicate)
        {
            int index = _playlist.FindLastIndex(startLastIndex, i => i.ScheduleMode == PlayScheduleMode.Timing && predicate(i));
            return new Tuple<ScheduleItem, int>(index == -1 ? null : _playlist[index], index);
        }
        #endregion

        class PlaylistSegment
        {
            public PlaylistSegment(ScheduleItem head)
            {
                this.Head = head;
                this.StartTime = head.StartTime;
            }

            public PlaylistSegment(DateTime startTime)
            {
                this.StartTime = startTime;
            }

            public ScheduleItem Head
            {
                get;private set;
            }

            public DateTime StartTime { get; private set; }

            public int BeginIndex { get; set; }
            public int EndIndex { get; set; }

            

            public DateTime StopTime { get; set; }
        }

        private DateTime? GetPlaylistStartTime()
        {
            ScheduleItem first = _playlist.FirstOrDefault();
            return first == null ? (DateTime?)null : first.StartTime;
            //return _playItems.GetStartTime();
        }

        private void UpdatePlaylist(int index, int length, IList<ScheduleItem> newItems)
        {
            for (int i = 0; i < length; i++)
            {
                _playlist.RemoveAt(index);
            }

            for (int i = newItems.Count - 1; i >= 0; i--)
            {
                _playlist.Insert(index, newItems[i]);
            }
        }

        private void ValidateTimeRange(DateTime startTime, TimeSpan duration)
        {
            ValidateStartTime(startTime);
            //var e = new TimeValidationEventArgs(startTime);
            //OnValidateStartTime(e);
            //if (!e.IsValid)
            //{
            //    throw new InvalidTimeException(startTime);
            //}

            ValidateTimeRange(startTime, startTime.Add(duration));
        }

        private void ValidateTimeRange(DateTime startTime, TimeSpan duration, IPlayItem excludeItem)
        {
            ValidateStartTime(startTime);
            ValidateTimeRange(startTime, startTime.Add(duration), excludeItem);
        }

        private void ValidateTimeRange(DateTime startTime, DateTime stopTime, IPlayItem excludeItem)
        {
            for (int i = 0; i < _playlist.Count; i++)
            {
                var item = _playlist[i];
                if (item != excludeItem &&
                    item.ScheduleMode == PlayScheduleMode.Timing || item.ScheduleMode == PlayScheduleMode.TimingBreak)
                {
                    var startTime2 = item.StartTime;
                    var stopTime2 = item.CalculatedStopTime;

                    if (stopTime > startTime2 && stopTime2 > startTime)
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        private void ValidateStartTime(DateTime startTime)
        {
        }

        private void ValidateTimeRange(DateTime startTime, DateTime stopTime)
        {
            for (int i = 0; i < _playlist.Count; i++)
            {
                var item = _playlist[i];
                if (item.ScheduleMode == PlayScheduleMode.Timing || item.ScheduleMode == PlayScheduleMode.TimingBreak)
                {
                    var startTime2 = item.StartTime;
                    var stopTime2 = item.CalculatedStopTime;

                    if (stopTime > startTime2 && stopTime2 > startTime)
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        private List<ScheduleItem> GetPlaylistItems(int beginIndex, int endIndex)
        {
            List<ScheduleItem> playItems = new List<ScheduleItem>();
            for (int i = beginIndex; i <= endIndex; i++)
            {
                playItems.Add(_playlist[i]);
            }
            return playItems;
        }

        

        public void Rollback()
        {
            _cancelCommit = true;
        }

        private void Commit()
        {
            if (!_cancelCommit)
            {
                _originalPlayItems.Clear();
                for (int i = 0; i < _playlist.Count; i++)
                {
                    var item = _playlist[i];
                    
                    _originalPlayItems.Add(item.CommitChange());
                }

                if (_onCommitted != null)
                {
                    _onCommitted();
                }
            }
        }

        public void ForcePlay(IPlayItem currentItem, IPlayItem forcedItem)
        {
            this.Delete(forcedItem);

            DateTime stopTime;
            int beginIndex;

            var startTime = DateTime.Now.AddSeconds(1.0);
            var currentIndex = _playlist.FindIndex(i => i.PlayItem == currentItem);
            beginIndex = currentIndex + 1;

            Tuple<ScheduleItem, int> temp = FindFirstTiming(beginIndex, (i)=>true);
            int endIndex;
            if (temp.Item1 != null)
            {
                stopTime = temp.Item1.StartTime;
                endIndex = temp.Item2 - 1;
            }
            else
            {
                stopTime = DateTime.MaxValue;
                endIndex = _playlist.Count - 1;
            }

            // 调整当前项的播放时长（入出点）


            // 复制当前项，并调整时长。
            var startOffset = startTime.Subtract(currentItem.StartTime);

            var newRange = new PlayRange(currentItem.CalculatedPlayRange.StartPosition + startOffset,
                currentItem.CalculatedPlayRange.Duration-startOffset);
            var copyItem = new AutoPlayItem(new AutoPlaybillItem(currentItem.PlaybillItem.PlaySource.Clone(newRange)));

            

            //if (segment.Head == null)
            //{
            //    startTime = segment.StartTime;
            //    beginIndex = segment.BeginIndex;
            //}
            //else
            //{
            //    startTime = segment.Head.CalculatedStopTime;
            //    beginIndex = segment.BeginIndex + 1;
            //}

            //stopTime = segment.StopTime;
            //endIndex = segment.EndIndex;

            this.Rebuild(startTime, stopTime, beginIndex, endIndex, (items) =>
            {
                // 插入复制项和forcedItem。
                items.Insert(0, new ScheduleItem(copyItem));
                items.Insert(0, new ScheduleItem(forcedItem));

            });
        }
    }


}