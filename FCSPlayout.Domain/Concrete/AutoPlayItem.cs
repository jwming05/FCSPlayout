using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class AutoPlayItem : IPlayItem
    {
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
                item1.PlayRange == null || item2.PlayRange == null)
            {
                return false;
            }

            return FCSPlayout.Domain.PlayRange.CanMerge(item1.PlayRange.Value, item2.PlayRange.Value);
        }

        internal static AutoPlayItem Merge(AutoPlayItem item1, AutoPlayItem item2)
        {
            if (!CanMerge(item1, item2)) throw new InvalidOperationException();

            var range = FCSPlayout.Domain.PlayRange.Merge(item1.PlayRange.Value, item2.PlayRange.Value);

            if (range == item1.PlaybillItem.GetPlayRange())
            {
                return new AutoPlayItem(item1.PlaybillItem);
            }
            else
            {
                return new AutoPlayItem(item1.PlaybillItem, range);
            }
        }

        private TimeSpan? _playDuration;
        public AutoPlayItem(IPlaybillItem billItem)
        {
            this.PlaybillItem = billItem;
        }

        // 用于创建顺播片断。
        private AutoPlayItem(IPlaybillItem billItem, PlayRange playRange)
        {
            this.PlaybillItem = billItem;
            this.PlayRange = playRange;
        }

        public IPlaybillItem PlaybillItem
        {
            get;private set;
        }

        /// <summary>
        /// 获取相对于<paramref name="PlaybillItem"/>的播放范围。
        /// </summary>
        internal PlayRange? PlayRange
        {
            get;private set;
        }

        public TimeSpan PlayDuration
        {
            get
            {
                if (_playDuration == null)
                {
                    return this.PlayRange == null ? 
                        this.PlaybillItem.PlaySource.GetDuration() : this.PlayRange.Value.Duration;
                }
                else
                {
                    return _playDuration.Value;
                }
            }

            internal set
            {
                _playDuration = value;
            }
        }

        public DateTime StartTime
        {
            get;internal set;
        }
        public bool IsAutoPadding { get{ return ((AutoPlaybillItem)this.PlaybillItem).IsAutoPadding; } }

        internal void Split(TimeSpan duration,out AutoPlayItem first,out AutoPlayItem second)
        {
            PlayRange range= this.PlayRange == null ? this.PlaybillItem.GetPlayRange() : this.PlayRange.Value;
            PlayRange firstRange, secondRange;

            FCSPlayout.Domain.PlayRange.Split(range, duration, out firstRange, out secondRange);

            first = new AutoPlayItem(this.PlaybillItem, firstRange);
            second = new AutoPlayItem(this.PlaybillItem, secondRange);
        }
    }

    //public static class AutoPlayItemExtensions
    //{
    //    public static PlayRange GetPlayRange(this AutoPlayItem playItem)
    //    {
    //        return playItem.PlayRange == null ? playItem.PlaybillItem.GetPlayRange() : playItem.PlayRange.Value;
    //    }
    //}
}
