using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlaylistBuildData
    {
        //private List<AutoPlayItem> _autoBillItems = new List<AutoPlayItem>();
        private List<IPlayItem> _autoBillItems = new List<IPlayItem>();
        //private SortedList<DateTime, TimingPlaybillItem> _timingBillItems = new SortedList<DateTime, TimingPlaybillItem>();
        private SortedList<DateTime, IPlayItem> _timingBillItems = new SortedList<DateTime, IPlayItem>();
        private IPlaylistEditor _editor;

        //public PlaylistBuildData(long editId)
        //{
        //    this.EditId = editId;
        //    this.Result = new List<IPlayItem>();
        //}

        public PlaylistBuildData(IPlaylistEditor editor)
        {
            //this.EditId = editId;
            _editor = editor;
            this.Result = new List<IPlayItem>();
        }

        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public void AddAuto(/*Auto*/IPlayItem autoItem)
        {
            Debug.Assert(autoItem.ScheduleMode == PlayScheduleMode.Auto);

            autoItem.Editor = _editor;

            if (!autoItem.IsAutoPadding())
            {
                if (_autoBillItems.Count > 0)
                {
                    var prev = _autoBillItems[_autoBillItems.Count - 1];
                    if (AutoPlayItem.CanMerge(prev, autoItem))
                    {
                        var mergedItem = AutoPlayItem.Merge(prev, autoItem);
                        mergedItem.Editor = _editor;
                        _autoBillItems[_autoBillItems.Count - 1] = mergedItem; // AutoPlayItem.Merge(prev, autoItem);
                        return;
                    }
                }

                _autoBillItems.Add(autoItem);
            }
        }

        public void InsertTiming(/*TimingPlaybillItem*/IPlayItem timingItem)
        {
            Debug.Assert(timingItem.ScheduleMode != PlayScheduleMode.Auto);
            timingItem.Editor = _editor;
            //_timingBillItems.Add(timingItem.StartTime.Value, timingItem);
            _timingBillItems.Add(timingItem.StartTime, timingItem);
        }

        internal /*Auto*/IPlayItem TakeAuto()
        {
            /*Auto*/IPlayItem result = _autoBillItems[0];
            _autoBillItems.RemoveAt(0);
            return result;
        }

        internal IPlayItem/*TimingPlaybillItem*/ TakeTiming()
        {
            IPlayItem/*TimingPlaybillItem*/ result = _timingBillItems.First().Value;
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

        //public long EditId { get; private set; }

        internal void AddResult(IPlayItem playItem)
        {
            var autoItem = playItem as AutoPlayItem;
            //if (autoItem != null)
            //{
            //    autoItem.EditId = this.EditId;
            //}
            //else
            //{
            //    if (playItem.EditId == null)
            //    {
            //        playItem.EditId = this.EditId;
            //    }
            //}
            this.Result.Add(playItem);
        }

        public IList<IPlayItem> Result { get; private set; }
    }
}