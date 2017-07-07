using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public struct TimeRange
    {
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
    }

    public interface IPlaylistBuildOption
    {
        /// <summary>
        /// 指示第一个播放项是否必须是定时播。
        /// </summary>
        bool FirstItemMustBeTiming { get; }

        //DateTime MinStartTime { get; }
        //DateTime MaxStopTime { get; }

        TimeRange PlayTimeRange { get; }
        
    }

    public class PlaylistBuilder
    {
        private PlaylistSegmentCollection _segments;
        private IPlaylistBuildOption _buildOption;

        public PlaylistBuilder(IList<IPlayItem> playItems)
        {
            _segments = new PlaylistSegmentCollection(playItems);
        }

        // 添加一个定时播。
        public void AddTimingItem(IPlayItem playItem)
        {
            Debug.Assert(playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing);

            ValidateTimeRange(playItem);

            //_buildOption.ValidateStartTime(playItem.StartTime);

            if (_segments.IsEmpty)
            {
                // TODO: 验证第一个定时播的开始时间是否有效。       
                

                var segment = _segments.CreateSegment(playItem);
                _segments.AddLast(segment);
            }
            else
            {
                var lastSegment = _segments.LastSegment;

                lastSegment.Add(playItem);

                // 只能添加到末尾。
                //if (playItem.StartTime < lastSegment.StartTime)
                //{
                //    throw new InvalidOperationException("新加的定时播必须在现有定时播后面。");
                //}

                //if (lastSegment.HasTimingConflict(playItem.StartTime, playItem.CalculatedStopTime))
                //{
                //    throw new InvalidOperationException("新加的定时播与现有定时播或定时插播之间有时间冲突。");
                //}
            }
        }

        private void ValidateTimeRange(IPlayItem playItem)
        {
            if(playItem.StartTime<_buildOption.PlayTimeRange.Start || playItem.CalculatedStopTime > _buildOption.PlayTimeRange.Stop)
            {
                throw new ArgumentException($"定时时间范围无效，开始时间不能小于{_buildOption.PlayTimeRange.Start}，结束时间不能大于{_buildOption.PlayTimeRange.Stop}。");
            }
        }

        public void AddTimingBreakItem(IPlayItem playItem)
        {
            Debug.Assert(playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak);

            if (_segments.IsEmpty)
            {
                if (_buildOption.FirstItemMustBeTiming)
                {
                    throw new InvalidOperationException("第一个播放项必须是定时播。");
                }

                var newSegment = _segments.CreateSegment(playItem);
                _segments.AddLast(newSegment);
            }
            else
            {

            }

            var segment = _segments.FindLastSegment((s) => s.StartTime <= playItem.StartTime);
            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            if (segment.HasTimingConflict(playItem.StartTime, playItem.CalculatedStopTime))
            {
                throw new InvalidOperationException();
            }

            if (segment.Next != null &&
                segment.Next.HasTimingConflict(playItem.StartTime, playItem.CalculatedStopTime))
            {
                throw new InvalidOperationException();
            }

            segment.AddTimingBreakItem(playItem);
        }

        public void AddAutoItem(IPlayItem playItem, IPlayItem prevItem)
        {
            Debug.Assert(playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.Auto);

            PlaylistSegment segment = null;
            if (prevItem != null)
            {
                segment = _segments.FindLastSegment((s) => s.Contains(prevItem));
            }
            else
            {
                segment = _segments.FindLastSegment((s) => true);
            }

            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            
            segment.InsertAuto(playItem, prevItem);
        }

        public void RemoveItem(IPlayItem playItem)
        {
            // TODO: 验证操作合法性。

            var segment = _segments.FindLastSegment(s => s.Contains(playItem));
            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            if (playItem.ScheduleMode == PlayScheduleMode.Timing)
            {
                if (segment.Previous == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    if (segment.Remove(playItem))
                    {
                        segment.Previous.MergeFrom(segment);
                        _segments.Remove(segment);
                        return;
                    }
                }
            }
            else
            {
                if (!segment.Remove(playItem))
                {
                    throw new InvalidOperationException();
                }
            }
        }
        
        public void ChangeStartTime(IPlayItem playItem,DateTime newStartTime)
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

        private void ChangeTimingStartTime(IPlayItem playItem, DateTime newStartTime)
        {
            var segment = _segments.FindLastSegment((s) => s.Contains(playItem));
            if (segment == null)
            {
                throw new ArgumentException();
            }

            var newStopTime = newStartTime.Add(playItem.PlaybillItem.PlayRange.Duration);

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
