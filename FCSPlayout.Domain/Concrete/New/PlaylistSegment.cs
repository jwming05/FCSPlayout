﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlaylistSegment
    {
        private List<IPlayItem> _playItems;
        private PlaylistSegment _next;
        //private List<IPlayItem> _newTimingBreakItems;
        private SortedList<DateTime, IPlayItem> _newTimingBreakItems;
        private PlaylistSegment _innerSegment;

        internal PlaylistSegment(List<IPlayItem> items)
        {
            this._playItems = new List<IPlayItem>(items);
            this.StartTime= _playItems[0].StartTime;
        }

        internal PlaylistSegment(IPlayItem item)
        {
            _playItems = new List<IPlayItem>();
            _playItems.Add(item);
            this.StartTime = _playItems[0].StartTime;
        }

        internal bool IsDirty { get; set; }


        //public DateTime StartTime { get { return _playItems[0].StartTime; } }

        public DateTime? StartTime { get; private set; }
        private DateTime? StopTime
        {
            get
            {
                if (this.Next != null)
                {
                    return this.Next.StartTime;
                }

                return null;
            }            
        }

        internal void Add(IPlayItem playItem)
        {
            if (playItem.ScheduleMode == PlayScheduleMode.Timing)
            {
                // 只能添加到末尾。
                if (playItem.StartTime < this.StartTime.Value)
                {
                    throw new InvalidOperationException("新加的定时播必须在现有定时播后面。");
                }

                if (this.HasTimingConflict(playItem.StartTime, playItem.CalculatedStopTime))
                {
                    throw new InvalidOperationException("新加的定时播与现有定时播或定时插播之间有时间冲突。");
                }

                List<IPlayItem> newSegmentItems = new List<IPlayItem>();
                newSegmentItems.Add(playItem);

                for(int i = _playItems.Count; i >= 0; i--)
                {
                    var temp = _playItems[i];
                    if (temp.ScheduleMode == PlayScheduleMode.TimingBreak)
                    {
                        if (temp.StartTime < playItem.StartTime)
                        {
                            break;
                        }

                        _playItems.RemoveAt(i);
                        newSegmentItems.Add(temp);
                    }
                }

                _innerSegment = new PlaylistSegment(newSegmentItems) { IsDirty = true };
                this.IsDirty = true;
            }
        }

        public PlaylistSegment Next
        {
            get { return _next; }
            internal set
            {
                if (_next != value)
                {
                    if (_next != null)
                    {
                        if (_next.Previous == this)
                        {
                            _next.Previous = null;
                        }
                    }

                    _next = value;
                    if (_next != null)
                    {
                        _next.Previous = this;
                    }
                    this.IsDirty = true;
                }
            }
        }
        public PlaylistSegment Previous { get; internal set; }

        internal bool HasTimingConflict(DateTime startTime, DateTime stopTime,IPlayItem excludeItem=null)
        {
            foreach (var item in _playItems)
            {
                if (item!=excludeItem && item.ScheduleMode != PlayScheduleMode.Auto)
                {
                    var startTime2 = item.StartTime;
                    var stopTime2 = item.CalculatedStopTime;

                    // stopTime2<=startTime || startTime2>=stopTime
                    if ((startTime==startTime2) || (stopTime==stopTime2) || 
                        (stopTime > startTime2 && stopTime2 > startTime))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private PlaylistSegmentBuildData CreatePlaylistSegmentBuildData(DateTime stopTime)
        {
            var data = new PlaylistSegmentBuildData();

            data.StartTime = this.StartTime.Value; // this.StartTime;
            data.StopTime = this.StopTime ?? stopTime;

            for (int i = 0; i < _playItems.Count; i++)
            {
                ScheduleItem item = new ScheduleItem(_playItems[i]);
                if (item.ScheduleMode == PlayScheduleMode.Auto)
                {
                    data.AddAuto(item);
                }
                else
                {
                    data.InsertTiming(item);
                }
            }

            if (_newTimingBreakItems != null)
            {
                var newTimingBreakList = _newTimingBreakItems.Values;

                for (int i = 0; i < newTimingBreakList.Count; i++)
                {
                    ScheduleItem item = new ScheduleItem(newTimingBreakList[i]);
                    data.InsertTiming(item);
                }
            }
            
            return data;
        }

        internal bool ShouldMerge()
        {
            return this.StartTime == null;
        }

        internal bool Remove(IPlayItem playItem)
        {
            if (playItem.ScheduleMode == PlayScheduleMode.Timing)
            {
                if (playItem.StartTime == this.StartTime)
                {
                    if (this.Previous == null)
                    {
                        // Note: 为了简单起见直接这样处理。
                        throw new InvalidOperationException("不能删除第一个定时播");
                    }
                }
            }

            if (_playItems.Remove(playItem))
            {
                this.IsDirty = true;
                return true;
            }
            return false;
        }

        internal void MergeFrom(PlaylistSegment segment)
        {
            foreach(var item in segment._playItems)
            {
                if (item.ScheduleMode == PlayScheduleMode.Timing)
                {
                    throw new InvalidOperationException();
                }

                this._playItems.Add(item);
                this.IsDirty = true;
            }
        }

        internal IPlaylistSegmentBuilder CreateBuilder(DateTime maxStopTime)
        {
            if (this.IsDirty)
            {
                PlaylistSegmentBuildData buildData = CreatePlaylistSegmentBuildData(maxStopTime);
                return new PlaylistSegmentBuilder(buildData);
            }
            else
            {
                return new NullPlaylistSegmentBuilder(_playItems);
            }
        }

        internal void AddTimingBreakItem(IPlayItem playItem)
        {
            AddNewTimingBreakItem(playItem);
            this.IsDirty = true;
        }

        private void AddNewTimingBreakItem(IPlayItem playItem)
        {
            if (_newTimingBreakItems == null)
            {
                _newTimingBreakItems = new SortedList<DateTime, IPlayItem>();
            }
            _newTimingBreakItems.Add(playItem.StartTime, playItem);
        }

        internal void ChangePlayRange(IPlayItem playItem, PlayRange newRange)
        {
            int index = _playItems.IndexOf(playItem);
            if (index < 0) throw new ArgumentException();

            IPlayItem newPlayItem = null;
            if (playItem.ScheduleMode != PlayScheduleMode.Auto)
            {
                if (newRange.Duration > playItem.PlayRange.Duration)
                {
                    var newStopTime = playItem.StartTime.Add(newRange.Duration);
                    if (this.HasTimingConflict(playItem.StartTime, newStopTime, playItem))
                    {
                        throw new InvalidOperationException();
                    }
                }                
                newPlayItem = (IPlayItem)((TimingPlaybillItem)playItem).Clone(newRange);
            }
            else
            {
                newPlayItem = new AutoPlayItem(((AutoPlaybillItem)playItem.PlaybillItem).Clone(newRange));
            }

            _playItems[index] = newPlayItem;
            this.IsDirty = true;
        }

        internal void ChangeSource(IPlayItem playItem, IMediaSource newSource, PlayRange? newRange)
        {
            int index = _playItems.IndexOf(playItem);
            if (index < 0) throw new ArgumentException();

            IPlayItem newPlayItem = null;
            if (newRange != null && playItem.ScheduleMode != PlayScheduleMode.Auto)
            {
                if (newRange.Value.Duration > playItem.PlayRange.Duration)
                {
                    var newStopTime = playItem.StartTime.Add(newRange.Value.Duration);
                    if (this.HasTimingConflict(playItem.StartTime, newStopTime, playItem))
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
            else
            {
            }

            newRange = newRange == null ? new PlayRange(TimeSpan.Zero, playItem.CalculatedPlayDuration) : newRange.Value;
            var newPlaySource = new PlaySource(newSource, newRange.Value, playItem.PlaybillItem.PlaySource.CGItems);

            switch (playItem.ScheduleMode)
            {
                case PlayScheduleMode.Auto:
                    newPlayItem = new AutoPlayItem(AutoPlaybillItem.Auto(newPlaySource));
                    break;
                case PlayScheduleMode.Timing:
                    newPlayItem = (IPlayItem)TimingPlaybillItem.Timing(newPlaySource, playItem.StartTime);
                    break;
                case PlayScheduleMode.TimingBreak:
                    newPlayItem = (IPlayItem)TimingPlaybillItem.TimingBreak(newPlaySource, playItem.StartTime);
                    break;
            }

            _playItems[index] = newPlayItem;
            this.IsDirty = true;
        }

        internal bool Contains(IPlayItem playItem)
        {
            if (this.StartTime > playItem.StartTime) return false;

            if (_playItems.Contains(playItem))
            {
                return true;
            }

            //if (_newTimingBreakItems != null && _newTimingBreakItems.Contains(playItem))
            //{
            //    return true;
            //}

            return false;
        }

        internal void InsertAuto(IPlayItem playItem, IPlayItem prevItem)
        {
            int index = _playItems.Count - 1;
            if (prevItem != null)
            {
                index = _playItems.IndexOf(prevItem);
                if (index < 0) throw new InvalidOperationException();
            }

            _playItems.Insert(index + 1, playItem);
            this.IsDirty = true;
        }

        internal void ChangeTimingToAuto(IPlayItem playItem)
        {
            var index = _playItems.IndexOf(playItem);
            if (index < 0) throw new InvalidOperationException();
            //Debug.Assert();
            var newPlayItem = new AutoPlayItem(AutoPlaybillItem.Auto(playItem.PlaybillItem.PlaySource));
            _playItems[index] = newPlayItem;
            this.IsDirty = true;
        }

        //internal void ChangeTimingToAuto(IPlayItem playItem)
        //{
        //    //var index = _playItems.IndexOf(playItem);
        //    if (_playItems[0]!=playItem) throw new InvalidOperationException();

        //    var newPlayItem = new AutoPlayItem(AutoPlaybillItem.Auto(playItem.PlaybillItem.PlaySource));
        //    _playItems[0] = newPlayItem;

        //    this.IsDirty = true;
        //}

        internal List<IPlayItem> SplitFrom(IPlayItem playItem)
        {
            var index = _playItems.IndexOf(playItem);
            if (index < 0) throw new ArgumentException();


            

            List<IPlayItem> newSegmentItems = new List<IPlayItem>();
            for(int i = _playItems.Count - 1; i >= index; i--)
            {
                newSegmentItems.Insert(0, _playItems[i]);
                _playItems.RemoveAt(i);
            }

            return newSegmentItems;
            //newSegmentItems.Insert(0, newItem);
            //_playItems.Remove(playItem);
        }

        internal void ChangeStartTime(DateTime newStartTime)
        {
            if (_playItems[0].ScheduleMode != PlayScheduleMode.Timing)
            {
                throw new InvalidOperationException();
            }

            var prevSegment = this.Previous;
            if (newStartTime < this.StartTime)
            {
                if (prevSegment != null)
                {
                    for(int i = prevSegment._playItems.Count - 1; i >= 0; i--)
                    {
                        var item = prevSegment._playItems[i];
                        if (item.ScheduleMode == PlayScheduleMode.TimingBreak)
                        {
                            if (item.StartTime >= newStartTime)
                            {
                                AddNewTimingBreakItem(item);
                                prevSegment._playItems.Remove(item);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else //newStartTime > this.StartTime
            {
                for (int i = this._playItems.Count - 1; i >= 0; i--)
                {
                    var item = this._playItems[i];
                    if (item.ScheduleMode == PlayScheduleMode.TimingBreak)
                    {
                        if (item.StartTime < newStartTime)
                        {
                            if (prevSegment == null)
                            {
                                throw new InvalidOperationException();
                            }

                            prevSegment.AddNewTimingBreakItem(item);
                            this._playItems.Remove(item);
                        }
                    }
                }
            }

            var playSource = _playItems[0].PlaybillItem.PlaySource;
            _playItems[0] = (TimingPlaybillItem)TimingPlaybillItem.Timing(playSource, newStartTime);

            if (prevSegment != null) prevSegment.IsDirty = true;
            this.IsDirty = true;
        }

        internal int IndexOf(IPlayItem playItem)
        {
            return _playItems.IndexOf(playItem);
        }

        internal int Count
        {
            get { return _playItems.Count; }
        }

        internal IPlayItem this[int index]
        {
            get { return _playItems[index]; }
        }

        class NullPlaylistSegmentBuilder : IPlaylistSegmentBuilder
        {
            private List<IPlayItem> _playItems;

            public NullPlaylistSegmentBuilder(List<IPlayItem> _playItems)
            {
                this._playItems = _playItems;
            }

            public IEnumerable<IPlayItem> Build()
            {
                foreach (var item in _playItems)
                {
                    yield return item;
                }
            }
        }
    }
}
