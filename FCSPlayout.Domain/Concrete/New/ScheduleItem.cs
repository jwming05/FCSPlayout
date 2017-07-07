using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class ScheduleItem
    {
        private readonly IPlayItem _playItem;
        private DateTime _startTime;
        private TimeSpan _calculatedPlayDuration;
        private bool _isDirty;

        public ScheduleItem(IPlayItem playItem)
        {
            _isDirty = false;
            _playItem = playItem;
            _startTime = _playItem.StartTime;
            _calculatedPlayDuration = _playItem.PlaybillItem.PlayRange.Duration; //  _playItem.CalculatedPlayDuration;
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    _isDirty = true;
                }
            }
        }

        public DateTime CalculatedStopTime
        {
            get
            {
                return this.StartTime.Add(this.CalculatedPlayDuration);
            }
        }

        public TimeSpan CalculatedPlayDuration
        {
            get { return _calculatedPlayDuration; }
            set
            {
                if (_calculatedPlayDuration != value)
                {
                    _calculatedPlayDuration = value;
                    _isDirty = true;
                }
            }
        }

        public PlayScheduleMode ScheduleMode
        {
            get { return _playItem.ScheduleMode; }
        }



        public bool IsAutoPadding
        {
            get { return _playItem.IsAutoPadding(); }
        }

        internal IPlayItem PlayItem
        {
            get
            {
                return _playItem;
            }
        }

        internal bool CanMerge(ScheduleItem autoItem)
        {
            return AutoPlayItem.CanMerge(_playItem, autoItem._playItem);
        }

        internal ScheduleItem Merge(ScheduleItem autoItem)
        {
            return new ScheduleItem(AutoPlayItem.Merge(_playItem, autoItem._playItem));
        }

        internal void Split(TimeSpan duration, out ScheduleItem first, out ScheduleItem second)
        {
            IPlayItem firstItem = null, secondItem = null;
            _playItem.Split(duration, out firstItem, out secondItem);
            first = new ScheduleItem(firstItem);
            second = new ScheduleItem(secondItem);
        }

        internal IPlayItem CommitChange()
        {
            if (_isDirty)
            {
                _playItem.StartTime = this.StartTime;
                _playItem.CalculatedPlayDuration = this.CalculatedPlayDuration;
                _isDirty = false;
            }
            return _playItem;
        }
    }
}
