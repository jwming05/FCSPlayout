using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCSPlayout.CG;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class AutoPlayItem : IPlayItem
    {
        #region Static Members
        internal static AutoPlayItem CreateAutoPadding(DateTime startTime, TimeSpan duration)
        {
            AutoPlaybillItem billItem = AutoPlaybillItem.CreateAutoPadding(duration);
            AutoPlayItem playItem = new AutoPlayItem(billItem);
            playItem.StartTime = startTime;
            return playItem;
        }

        internal static bool CanMerge(AutoPlayItem item1, AutoPlayItem item2)
        {
            if (item1.PlaybillItem != item2.PlaybillItem ||
                item1.PlayRange == item1.PlaybillItem.PlayRange ||
                item2.PlayRange == item2.PlaybillItem.PlayRange)
            {
                return false;
            }

            return FCSPlayout.Domain.PlayRange.CanMerge(item1.PlayRange, item2.PlayRange);
        }

        internal static AutoPlayItem Merge(AutoPlayItem item1, AutoPlayItem item2)
        {
            if (!CanMerge(item1, item2)) throw new InvalidOperationException();

            var range = FCSPlayout.Domain.PlayRange.Merge(item1.PlayRange, item2.PlayRange);

            if (range == item1.PlaybillItem.PlayRange)
            {
                return new AutoPlayItem(item1.PlaybillItem);
            }
            else
            {
                return new AutoPlayItem(item1.PlaybillItem, range);
            }
        }
        #endregion Static Members


        private TimeSpan? _playDuration;
        private PlayRange? _playRange;
        public AutoPlayItem(IPlaybillItem billItem)
        {
            this.Id = Guid.NewGuid();
            this.PlaybillItem = billItem;
        }

        // 用于创建顺播片断。
        private AutoPlayItem(IPlaybillItem billItem, PlayRange playRange):this(billItem)
        {
            this.PlayRange = playRange;
        }

        public IPlaybillItem PlaybillItem
        {
            get;private set;
        }

        public Guid Id
        {
            get; set;
        }
        /// <summary>
        /// 获取相对于<paramref name="PlaybillItem"/>的播放范围。
        /// </summary>
        public PlayRange PlayRange
        {
            get
            {
                if (_playRange == null)
                {
                    return this.PlaybillItem.PlayRange;
                }
                else
                {
                    return _playRange.Value;
                }
                
            }
            set { _playRange = value; }
        }

        public TimeSpan CalculatedPlayDuration
        {
            get
            {
                if (_playDuration == null)
                {
                    return this.PlayRange.Duration;
                }
                else
                {
                    return _playDuration.Value;
                }
            }

            set
            {
                _playDuration = value;
            }
        }

        public PlayRange CalculatedPlayRange
        {
            get
            {
                if (_playDuration == null)
                {
                    return this.PlayRange;
                }
                else
                {
                    return new PlayRange(this.PlayRange.StartPosition, this.CalculatedPlayDuration);
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

        public DateTime StartTime
        {
            get; set;
        }
        public bool IsAutoPadding { get{ return ((AutoPlaybillItem)this.PlaybillItem).IsAutoPadding; } }

        [NonSerialized]
        private long? _editId;
        public long? EditId
        {
            get { return _editId; }
            set { _editId = value; }
        }

        public PlayScheduleMode ScheduleMode
        {
            get
            {
                return this.PlaybillItem.ScheduleMode;
            }
        }

        public IMediaSource MediaSource
        {
            get
            {
                return this.PlaybillItem.MediaSource;
            }
        }

        public string Title
        {
            get
            {
                return this.PlaybillItem.Title;
            }
        }

        public CGItemCollection CGItems
        {
            get
            {
                return this.PlaybillItem.CGItems;
            }
        }

        internal void Split(TimeSpan duration,out AutoPlayItem first,out AutoPlayItem second)
        {
            PlayRange firstRange, secondRange;

            FCSPlayout.Domain.PlayRange.Split(this.PlayRange, duration, out firstRange, out secondRange);

            first = new AutoPlayItem(this.PlaybillItem, firstRange);
            second = new AutoPlayItem(this.PlaybillItem, secondRange);
        }

        //public IPlayItem Clone()
        //{
        //    var result = new AutoPlayItem(this.PlaybillItem.Clone());
        //    result._playDuration = this._playDuration;
        //    result._playRange = this._playRange;
        //    result.Id = this.Id;
        //    result.StartTime = this.StartTime;

        //    return result;
        //}
    }
}