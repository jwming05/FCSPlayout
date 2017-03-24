using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public partial class PlaylistEditor : IPlaylistEditor
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
        
        public void Dispose()
        {
            if (this.Disposed != null)
            {
                this.Disposed(this, EventArgs.Empty);
            }
        }

        public void ClearAll()
        {
            if (_playlist.CanClear() && _playlist.Count > 0)
            {
                _playlist.Clear();
            }
        }

        public void Append(IList<IPlayItem> playItems)
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
                    var playlistStartTime = _playlist.GetStartTime();
                    if (playlistStartTime != null)
                    {
                        TimeSpan duration = itemStartTime.Subtract(playlistStartTime.Value);
                        if (duration > TimeSpan.Zero)
                        {
                            var autoPadding = AutoPlayItem.CreateAutoPadding(playlistStartTime.Value, duration);
                            var newItems = new List<IPlayItem>();
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
                        var autoPadding = AutoPlayItem.CreateAutoPadding(lastStopTime, duration);
                        var newItems = new List<IPlayItem>();
                        newItems.Add(autoPadding);
                        newItems.AddRange(playItems);

                        playItems = newItems;
                    }
                    else
                    {

                    }
                }
            }

            _playlist.Update(beginIndex, _playlist.Count - beginIndex, playItems);
        }

        

        public event EventHandler Disposed;

        #region
        private PlaylistSegment FindSegment(IPlayItem playItem)
        {
            if (_playlist.Count == 0)
            {
                throw new InvalidOperationException();
            }

            int currentIndex = _playlist.FindFirstIndex(i => i == playItem);
            if (currentIndex == -1)
            {
                throw new InvalidOperationException();
            }

            int prevIndex = _playlist.FindLastIndex(currentIndex, i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && i.StartTime <= playItem.StartTime);

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

            int nextIndex = _playlist.FindFirstIndex(currentIndex + 1, i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && i.StartTime > playItem.StartTime);

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

        private Tuple<IPlayItem, int> FindFirstTiming(Func<IPlayItem, bool> predicate)
        {
            int index = _playlist.FindFirstIndex(i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && predicate(i));
            return new Tuple<IPlayItem, int>(index == -1 ? null : _playlist[index], index);
        }

        private Tuple<IPlayItem, int> FindFirstTiming(int startIndex, Func<IPlayItem, bool> predicate)
        {
            int index = _playlist.FindFirstIndex(startIndex, i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && predicate(i));
            return new Tuple<IPlayItem, int>(index == -1 ? null : _playlist[index], index);
        }

        private Tuple<IPlayItem, int> FindLastTiming(Func<IPlayItem, bool> predicate)
        {
            int index = _playlist.FindLastIndex(i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && predicate(i));
            return new Tuple<IPlayItem, int>(index == -1 ? null : _playlist[index], index);
        }

        private Tuple<IPlayItem, int> FindLastTiming(int startLastIndex, Func<IPlayItem, bool> predicate)
        {
            int index = _playlist.FindLastIndex(startLastIndex, i => i.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing && predicate(i));
            return new Tuple<IPlayItem, int>(index == -1 ? null : _playlist[index], index);
        }
        #endregion

        class PlaylistSegment
        {
            public PlaylistSegment(IPlayItem head)
            {
                this.Head = head;
                this.StartTime = head.StartTime;
            }

            public PlaylistSegment(DateTime startTime)
            {
                this.StartTime = startTime;
            }

            public IPlayItem Head
            {
                get;private set;
            }

            public DateTime StartTime { get; private set; }

            public int BeginIndex { get; set; }
            public int EndIndex { get; set; }

            

            public DateTime StopTime { get; set; }
        }
    }
}