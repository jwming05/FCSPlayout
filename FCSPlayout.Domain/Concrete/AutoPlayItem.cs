using FCSPlayout.CG;
using System;

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

        internal static bool CanMerge(/*Auto*/IPlayItem item1, /*Auto*/IPlayItem item2)
        {
            if(item1.ScheduleMode != PlayScheduleMode.Auto || item2.ScheduleMode != PlayScheduleMode.Auto)
            {
                return false;
            }

            //if (item1.PlaybillItem != item2.PlaybillItem ||
            //    item1.PlayRange == item1.PlaybillItem.PlayRange ||
            //    item2.PlayRange == item2.PlaybillItem.PlayRange)
            //{
            //    return false;
            //}

            //return FCSPlayout.Domain.PlayRange.CanMerge(item1.PlayRange, item2.PlayRange);

            return item1.PlaybillItem.CanMerge(item2.PlaybillItem);
        }

        internal static AutoPlayItem Merge(/*Auto*/IPlayItem item1, /*Auto*/IPlayItem item2)
        {
            if (!CanMerge(item1, item2)) throw new InvalidOperationException();

            return new AutoPlayItem(item1.PlaybillItem.Merge(item2.PlaybillItem));

            //var range = FCSPlayout.Domain.PlayRange.Merge(item1.PlayRange, item2.PlayRange);

            //if (range == item1.PlaybillItem.PlayRange)
            //{
            //    return new AutoPlayItem(item1.PlaybillItem);
            //}
            //else
            //{
            //    return new AutoPlayItem(item1.PlaybillItem, range);
            //}
        }
        #endregion Static Members


        private TimeSpan? _calculatedPlayDuration;
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
                if (_calculatedPlayDuration == null)
                {
                    return this.PlayRange.Duration;
                }
                else
                {
                    return _calculatedPlayDuration.Value;
                }
            }

            set
            {
                _calculatedPlayDuration = value;
            }
        }

        public PlayRange CalculatedPlayRange
        {
            get
            {
                if (_calculatedPlayDuration == null)
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
        internal bool IsAutoPadding { get{ return ((AutoPlaybillItem)this.PlaybillItem).IsAutoPadding; } }

        //[NonSerialized]
        //private long? _editId;
        //public long? EditId
        //{
        //    get { return _editId; }
        //    set { _editId = value; }
        //}

        [NonSerialized]
        private IPlaylistEditor _editor;
        public IPlaylistEditor Editor
        {
            get { return _editor; }
            set { _editor = value; }
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

            set
            {
                this.PlaybillItem.CGItems = value;
            }
        }

        public void Split(TimeSpan duration,out IPlayItem first,out IPlayItem second)
        {
            PlayRange firstRange, secondRange;

            FCSPlayout.Domain.PlayRange.Split(this.PlayRange, duration, out firstRange, out secondRange);

            first = new AutoPlayItem(this.PlaybillItem.Clone(firstRange), firstRange);
            second = new AutoPlayItem(this.PlaybillItem.Clone(secondRange), secondRange);
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