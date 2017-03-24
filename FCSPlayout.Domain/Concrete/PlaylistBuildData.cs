﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlaylistBuildData
    {
        private List<AutoPlayItem> _autoBillItems = new List<AutoPlayItem>();
        private SortedList<DateTime, TimingPlaybillItem> _timingBillItems = new SortedList<DateTime, TimingPlaybillItem>();

        public PlaylistBuildData(long editId)
        {
            this.EditId = editId;
            this.Result = new List<IPlayItem>();
        }

        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public void AddAuto(AutoPlayItem autoItem)
        {
            if (!autoItem.IsAutoPadding)
            {
                if (_autoBillItems.Count > 0)
                {
                    var prev = _autoBillItems[_autoBillItems.Count - 1];
                    if (AutoPlayItem.CanMerge(prev, autoItem))
                    {
                        _autoBillItems[_autoBillItems.Count - 1] = AutoPlayItem.Merge(prev, autoItem);
                        return;
                    }
                }

                _autoBillItems.Add(autoItem);
            }
        }

        public void InsertTiming(TimingPlaybillItem timingItem)
        {
            _timingBillItems.Add(timingItem.StartTime.Value, timingItem);
        }

        internal AutoPlayItem TakeAuto()
        {
            AutoPlayItem result = _autoBillItems[0];
            _autoBillItems.RemoveAt(0);
            return result;
        }

        internal TimingPlaybillItem TakeTiming()
        {
            TimingPlaybillItem result = _timingBillItems.First().Value;
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

        public long EditId { get; private set; }

        internal void AddResult(IPlayItem playItem)
        {
            var autoItem = playItem as AutoPlayItem;
            if (autoItem != null)
            {
                autoItem.EditId = this.EditId;
            }
            else
            {
                if (playItem.EditId == null)
                {
                    playItem.EditId = this.EditId;
                }
            }
            this.Result.Add(playItem);
        }

        public IList<IPlayItem> Result { get; private set; }
    }
}