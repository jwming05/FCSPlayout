using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{

    public partial class PlaylistBuilder
    {
        public void AddTimingItem(IPlayItem playItem)
        {
            //LogAction();
            Debug.Assert(playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing);

            ValidateTimeRange(playItem);

            if (_segments.IsEmpty)
            {
                if (_buildOption.RequireFirstTimingItem && playItem.StartTime != _buildOption.PlayTimeRange.StartTime)
                {
                    throw new ArgumentException($"第一个定时播开始时间必须是{_buildOption.PlayTimeRange.StartTime}");
                }

                if (playItem.StartTime != _buildOption.PlayTimeRange.StartTime)
                {
                    TimeSpan duration = _buildOption.PlayTimeRange.StartTime.Subtract(playItem.StartTime);
                    if (!ValidatePlayDuration(duration))
                    {
                        throw new InvalidOperationException("操作无效，该操作将产生播放时长小于最小播放时长的自动垫片");
                    }
                    _segments.AddLast(_segments.CreateSegment(_buildOption.PlayTimeRange.StartTime));
                }

                var segment = _segments.CreateSegment(playItem);
                _segments.AddLast(segment);
            }
            else
            {
                _segments.LastSegment.Add(playItem);
            }
        }

        public void AddTimingBreakItem(IPlayItem playItem)
        {
            Debug.Assert(playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak);

            ValidateTimeRange(playItem);

            if (_segments.IsEmpty)
            {
                if (_buildOption.RequireFirstTimingItem)
                {
                    throw new InvalidOperationException("第一个播放项必须是定时播。");
                }

                var newSegment = _segments.CreateSegment(_buildOption.PlayTimeRange.StartTime, playItem);
                _segments.AddLast(newSegment);
            }
            else
            {
                var segment = _segments.FindLastSegment((s) => s.StartTime <= playItem.StartTime);
                if (segment == null)
                {
                    throw new ArgumentException();
                }

                segment.Add(playItem);
            }


        }

        public void AddAutoItem(IPlayItem playItem, IPlayItem prevItem)
        {
            Debug.Assert(playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.Auto);

            if (_segments.IsEmpty)
            {
                if (_buildOption.RequireFirstTimingItem)
                {
                    throw new InvalidOperationException("第一个播放项必须是定时播。");
                }

                var newSegment = _segments.CreateSegment(_buildOption.PlayTimeRange.StartTime, playItem);
                _segments.AddLast(newSegment);
            }
            else
            {
                PlaylistSegment segment = prevItem != null ? 
                    _segments.FindLastSegment((s) => s.Contains(prevItem)) : _segments.LastSegment;

                if (segment == null)
                {
                    throw new ArgumentException();
                }

                if (prevItem == null)
                {
                    segment.Add(playItem);
                }
                else
                {
                    segment.InsertAuto(playItem, prevItem);
                }
                
            }
            
        }

        public void RemoveItem(IPlayItem playItem)
        {
            var segment = _segments.FindLastSegment(s => s.Contains(playItem));
            if (segment == null)
            {
                throw new ArgumentException();
            }

            if (playItem.ScheduleMode == PlayScheduleMode.Timing)
            {
                if (segment.Previous == null && _buildOption.RequireFirstTimingItem)
                {
                    throw new InvalidOperationException("不能删除第一个定时播");
                }
            }

            if (!segment.Remove(playItem))
            {
                throw new InvalidOperationException();
            }
        }

        public void ChangeStartTime(IPlayItem playItem, DateTime newStartTime)
        {
            // TODO: 检查操作合法性。
            if (newStartTime == playItem.StartTime) return;


            if (playItem.ScheduleMode == PlayScheduleMode.Timing)
            {
                ChangeTimingStartTime(playItem, newStartTime);
            }
            else if (playItem.ScheduleMode == PlayScheduleMode.TimingBreak)
            {
                var newBreakItem = (TimingPlaybillItem)TimingPlaybillItem.TimingBreak(playItem.PlaybillItem.PlaySource, newStartTime);
                this.RemoveItem(playItem);  // remove a break item
                this.AddTimingBreakItem(newBreakItem);     // add a new breakItem
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void ChangeTimingStartTime(IPlayItem playItem, DateTime newStartTime)
        {
            var segment = _segments.FindLastSegment((s) => s.Contains(playItem));
            if (segment == null)
            {
                throw new ArgumentException();
            }

            if(segment.Previous==null && _buildOption.RequireFirstTimingItem)
            {
                throw new InvalidOperationException("不能修改第一个定时播开始时间");
            }
            var newStopTime = newStartTime.Add(playItem.PlaybillItem.PlayRange.Duration);
            ValidateTimeRange(newStartTime, newStopTime);

            if (segment.Previous == null)
            {
                Debug.Assert(newStartTime > playItem.StartTime);
                Debug.Assert(_buildOption.PlayTimeRange.StartTime == playItem.StartTime);

                TimeSpan duration = newStartTime.Subtract(playItem.StartTime);

                if (!ValidatePlayDuration(duration))
                {
                    throw new InvalidOperationException("操作无效，该操作将产生播放时长小于最小播放时长的自动垫片");
                }
                _segments.AddFirst(_segments.CreateSegment(_buildOption.PlayTimeRange.StartTime));
            }

            segment.ChangeStartTime(newStartTime);

            // 编辑第一个定时播的开始时间。

            if (newStartTime < playItem.StartTime)
            {
                // 前移
                if (segment.Previous == null)
                {
                    // TODO: 验证第一个定时播的开始时间。
                    ValidatePlaylistStartTime(newStartTime);
                }
                else
                {

                    if (newStartTime < segment.Previous.StartTime)
                    {
                        throw new ArgumentException();
                    }

                    if (segment.Previous.HasTimingConflict(newStartTime, newStopTime))
                    {
                        throw new InvalidOperationException();
                    }

                    // 可能需要把前面片断中的定时插播移到当前片断中。
                }
            }
            else
            {
                // 后移

                if (segment.HasTimingConflict(newStartTime, newStopTime, playItem))
                {
                    throw new InvalidOperationException();
                }

                if (segment.Next != null && newStopTime > segment.Next.StartTime)
                {
                    throw new InvalidOperationException();
                }

                if (segment.Previous == null)
                {
                    // TODO: 验证第一个定时播的开始时间。
                    ValidatePlaylistStartTime(newStartTime);

                    // 检查当前片断中是否有定时插播并且开始时间小于newStartTime，
                }
                // 可能需要把当前片断中的定时插播移到前面片断中。
            }

            segment.ChangeStartTime(newStartTime);
        }
    }



    public partial class PlaylistBuilder
    {
        private PlaylistSegmentCollection _segments;
        private IPlaylistBuildOption _buildOption;

        public PlaylistBuilder(IList<IPlayItem> playItems, IPlaylistBuildOption buildOption)
        {
            _buildOption = buildOption;
            _segments = new PlaylistSegmentCollection(playItems, buildOption);
        }

        // 添加一个定时播。
        

        private bool ValidatePlayDuration(TimeSpan duration)
        {
            return duration >= _buildOption.MinPlayDuration;
        }

        private void ValidateTimeRange(IPlayItem playItem)
        {
            if(playItem.StartTime<_buildOption.PlayTimeRange.StartTime || playItem.CalculatedStopTime > _buildOption.PlayTimeRange.StopTime)
            {
                throw new ArgumentException($"定时时间范围无效，开始时间不能小于{_buildOption.PlayTimeRange.StartTime}，结束时间不能大于{_buildOption.PlayTimeRange.StopTime}。");
            }
        }

        private void ValidateTimeRange(DateTime startTime,DateTime stopTime)
        {
            if (startTime < _buildOption.PlayTimeRange.StartTime || stopTime > _buildOption.PlayTimeRange.StopTime)
            {
                throw new ArgumentException($"定时时间范围无效，开始时间不能小于{_buildOption.PlayTimeRange.StartTime}，结束时间不能大于{_buildOption.PlayTimeRange.StopTime}。");
            }
        }







        public void ChangePlayRange(IPlayItem playItem, PlayRange newRange)
        {
            // TODO:操作有效性验证。

            playItem.PlaybillItem.PlaySource.MediaSource.ValidatePlayRange(newRange);

            var segment = _segments.FindLastSegment(s => s.Contains(playItem));
            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            segment.ChangePlayRange(playItem, newRange);
        }

        

        // 验证播放列表中第一个定时播的开始时间。
        private void ValidatePlaylistStartTime(DateTime startTime)
        {
            //throw new NotImplementedException();
        }

        public IEnumerable<IPlayItem> Build(DateTime? maxStopTime)
        {
            return _segments.Build(maxStopTime ?? DateTime.MaxValue);
        }

        public void ChangeSource(IPlayItem playItem,IMediaSource newSource,PlayRange? newRange)
        {
            // TODO:操作有效性验证。

            if (newRange != null)
            {
                newSource.ValidatePlayRange(newRange.Value);
            }

            var segment = _segments.FindLastSegment(s => s.Contains(playItem));
            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            segment.ChangeSource(playItem, newSource, newRange);
        }

        public void ChangeToAuto(IPlayItem playItem)
        {
            // TODO:操作有效性验证。
            var segment = _segments.FindLastSegment(s => s.Contains(playItem));
            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            if (playItem.ScheduleMode == PlayScheduleMode.TimingBreak)
            {
                segment.ChangeTimingToAuto(playItem);
            }
            else if (playItem.ScheduleMode == PlayScheduleMode.Timing)
            {
                if (segment.Previous == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    segment.ChangeTimingToAuto(playItem);
                    var prevSegment = segment.Previous;
                    prevSegment.MergeFrom(segment);

                    _segments.Remove(segment);
                }
            }
            else
            {
            }
        }

        public void ChangeToTiming(IPlayItem playItem)
        {
            // 顺播被截短的情况？？

            // 考虑使得就地编辑方法，直接替换列表中的项，然后重新构建Segment（需要识别出是新建项，从而知道该Segment是脏的）。
            // 当然需要考虑是否允许编辑第一个定时播。

            // TODO:操作有效性验证。
            var segment = _segments.FindLastSegment(s => s.Contains(playItem));
            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            var newSegmentItems=segment.SplitFrom(playItem);

            var startTime = playItem.StartTime;
            var newItem = (IPlayItem)TimingPlaybillItem.Timing(playItem.PlaybillItem.PlaySource, startTime);

            newSegmentItems[0] = newItem;

            var newSegment = new PlaylistSegment(newSegmentItems) { IsDirty = true };

            _segments.InsertSegment(segment, newSegment);
        }

        public void ChangeToTimingBreak(IPlayItem playItem)
        {
            // 顺播被截短的情况？？

            // 考虑使用就地编辑方法，直接替换列表中的项，然后重新构建Segment（需要识别出是新建项，从而知道该Segment是脏的）。
            // 当然需要考虑是否允许编辑第一个定时播。

            // TODO:操作有效性验证。
            var segment = _segments.FindLastSegment(s => s.Contains(playItem));
            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            var startTime = playItem.StartTime;
            var newItem = (IPlayItem)TimingPlaybillItem.TimingBreak(playItem.PlaybillItem.PlaySource, startTime);

            this.RemoveItem(playItem);
            this.AddTimingBreakItem(newItem);
        }

        public void MoveUp(IPlayItem playItem)
        {
            // Ensure it is auto playItem

            // TODO:操作有效性验证。
            var segment = _segments.FindLastSegment(s => s.Contains(playItem));
            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            // get previous of previous and then reorder
            int index=segment.IndexOf(playItem);
            if (index < 0)
            {
                throw new ArgumentException();
            }

            IPlayItem prevItem = null;
            if (index > 0)
            {
                index = index - 2;
                if (index >= 0)
                {
                    prevItem = segment[index];
                }
                else
                {
                    if (segment.Previous != null)
                    {
                        prevItem = segment.Previous[segment.Count-1];  // last
                    }
                    else
                    {

                    }
                }
            }

            if (prevItem != null)
            {
                ReorderAutoPlayItem(prevItem, playItem);
            }
        }

        public void MoveDown(IPlayItem playItem)
        {
            // Ensure it is auto playItem

            // TODO:操作有效性验证。
            var segment = _segments.FindLastSegment(s => s.Contains(playItem));
            if (segment == null)
            {
                throw new InvalidOperationException();
            }


            // get next and then reorder
            int index = segment.IndexOf(playItem);
            if (index < 0)
            {
                throw new ArgumentException();
            }

            IPlayItem prevItem = null;

            index = index + 1;
            if (index < segment.Count)
            {
                prevItem = segment[index];
            }
            else
            {
                if (segment.Next != null)
                {
                    prevItem = segment.Next[0]; 
                }
                else
                {

                }
            }

            if (prevItem != null)
            {
                ReorderAutoPlayItem(prevItem, playItem);
            }
        }

        public void ReorderAutoPlayItem(IPlayItem newPrevItem,IPlayItem playItem)
        {
            Debug.Assert(playItem.ScheduleMode == PlayScheduleMode.Auto);
            this.RemoveItem(playItem);
            this.AddAutoItem(newPrevItem, playItem);
        }
    }

    interface IPlayItemBehavior
    {
        bool CanRemove(IPlayItem playItem);
        bool CanEdit(IPlayItem playItem,EditOption option);
        bool CanMove(IPlayItem playItem, MoveOption option);
        void Add(IPlayItem playItem);
        void Remove(IPlayItem playItem);
        void Edit(IPlayItem playItem, EditOption option);
        void Move(IPlayItem playItem, MoveOption option);
    }

    enum MoveOption
    {
        Up,
        Down
    }
    enum EditOption
    {
        StartTime,
        PlaySource,
        ScheduleMode
    }
}

/*
 * 入口点（起点）
 * 
 * 
 * 
 * 节目单同步机制
 * 
 * 触发命令
 * 延时命令
 * 加载节目单命令
 * 更新节目单命令
 */
