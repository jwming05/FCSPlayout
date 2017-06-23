using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlaylistSegmentBuildData
    {
        private List<ScheduleItem> _autoBillItems = new List<ScheduleItem>();
        private SortedList<DateTime, ScheduleItem> _timingBillItems = new SortedList<DateTime, ScheduleItem>();

        public PlaylistSegmentBuildData()
        {
            this.Result = new List<ScheduleItem>();
        }

        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public void AddAuto(ScheduleItem autoItem)
        {
            Debug.Assert(autoItem.ScheduleMode == PlayScheduleMode.Auto);

            if (!autoItem.IsAutoPadding)
            {
                if (_autoBillItems.Count > 0)
                {
                    var prev = _autoBillItems[_autoBillItems.Count - 1];
                    if (prev.CanMerge(autoItem))
                    {
                        ScheduleItem mergedItem = prev.Merge(autoItem);
                        _autoBillItems[_autoBillItems.Count - 1] = mergedItem;
                        return;
                    }
                }

                _autoBillItems.Add(autoItem);
            }
        }

        public void InsertTiming(ScheduleItem timingItem)
        {
            Debug.Assert(timingItem.ScheduleMode != PlayScheduleMode.Auto);
            _timingBillItems.Add(timingItem.StartTime, timingItem);
        }

        internal ScheduleItem TakeTiming()
        {
            if (_timingBillItems.Count > 0)
            {
                var first = _timingBillItems.First().Value;
                if (first.ScheduleMode == PlayScheduleMode.Timing)
                {
                    _timingBillItems.RemoveAt(0);
                    return first;
                }
            }
            return null;
        }

        

        internal ScheduleItem TakeAuto()
        {
            if (_autoBillItems.Count > 0)
            {
                ScheduleItem result = _autoBillItems[0];
                _autoBillItems.RemoveAt(0);
                return result;
            }
            return null;
        }

        internal ScheduleItem TakeTimingBreak()
        {
            if (_timingBillItems.Count > 0)
            {
                ScheduleItem first = _timingBillItems.First().Value;
                if (first.ScheduleMode == PlayScheduleMode.TimingBreak)
                {
                    _timingBillItems.RemoveAt(0);
                    return first;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            return null;
        }

        internal void AddResult(ScheduleItem playItem)
        {
            this.Result.Add(playItem);
        }

        public IList<ScheduleItem> Result { get; private set; }
    }
}