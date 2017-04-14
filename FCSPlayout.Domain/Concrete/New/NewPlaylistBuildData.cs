using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class NewPlaylistBuildData
    {
        private List<ScheduleItem> _autoBillItems = new List<ScheduleItem>();
        private SortedList<DateTime, ScheduleItem> _timingBillItems = new SortedList<DateTime, ScheduleItem>();

        public NewPlaylistBuildData()
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

        internal ScheduleItem TakeAuto()
        {
            ScheduleItem result = _autoBillItems[0];
            _autoBillItems.RemoveAt(0);
            return result;
        }

        internal ScheduleItem TakeTiming()
        {
            ScheduleItem result = _timingBillItems.First().Value;
            _timingBillItems.RemoveAt(0);
            return result;
        }

        internal bool HasAutoPlaybillItem
        {
            get { return _autoBillItems.Count > 0; }
        }
        internal bool HasTimingPlaybillItem
        {
            get { return _timingBillItems.Count > 0; }
        }


        internal void AddResult(ScheduleItem playItem)
        {
            this.Result.Add(playItem);
        }

        public IList<ScheduleItem> Result { get; private set; }
    }
}