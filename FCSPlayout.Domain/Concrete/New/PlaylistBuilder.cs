using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlaylistBuilder
    {
        private PlaylistSegmentCollection _segments;

        public PlaylistBuilder(IList<IPlayItem> playItems)
        {
            _segments = new PlaylistSegmentCollection(playItems);
        }

        // 添加一个定时播。
        public void AddTimingItem(IPlayItem playItem)
        {
            Debug.Assert(playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing);

            if (_segments.IsEmpty)
            {
                // TODO: 验证第一个定时播的开始时间是否有效。       
            }
            else
            {
                var lastSegment = _segments.LastSegment;

                // 只能添加到末尾。
                if (playItem.StartTime < lastSegment.StartTime)
                {
                    throw new InvalidOperationException();
                }

                if (lastSegment.HasTimingConflict(playItem.StartTime, playItem.CalculatedStopTime))
                {
                    throw new InvalidOperationException();
                }
            }

            var segment = _segments.CreateSegment(playItem);
            _segments.AddLast(segment);
        }

        public void AddTimingBreakItem(IPlayItem playItem)
        {
            Debug.Assert(playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak);

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
                        segment.Previous.Merge(segment);
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
                this.RemoveItem(playItem);
                this.AddTimingBreakItem(newBreakItem);     
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
                if (segment.Previous == null)
                {
                    // TODO: 验证第一个定时播的开始时间。
                    ValidatePlaylistStartTime(newStartTime);
                }
                else
                {
                    // 前移
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
                segment.ChangeTimingBreakToAuto(playItem);
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
                    prevSegment.Merge(segment);

                    _segments.Remove(segment);
                }
            }
            else
            {
            }
        }

        public void ChangeToTiming(IPlayItem playItem)
        {

        }

        public void ChangeToTimingBreak(IPlayItem playItem)
        {
            // TODO:操作有效性验证。
            var segment = _segments.FindLastSegment(s => s.Contains(playItem));
            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            var startTime = playItem.StartTime;
            var temp = (IPlayItem)TimingPlaybillItem.TimingBreak(playItem.PlaybillItem.PlaySource, startTime);
            segment.Remove(playItem);
            segment.AddTimingBreakItem(temp);

            
        }
    }
}
