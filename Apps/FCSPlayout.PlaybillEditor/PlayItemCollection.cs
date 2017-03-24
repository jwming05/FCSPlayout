using FCSPlayout.Domain;
using FCSPlayout.WPFApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FCSPlayout.PlaybillEditor
{
    public class PlayItemCollection : ObservableCollection<BindablePlayItem>, IPlayItemCollection
    {
        IPlayItem IPlayItemCollection.this[int index]
        {
            get
            {
                return this[index].PlayItem;
            }
        }

        int IPlayItemCollection.Count
        {
            get
            {
                return this.Count;
            }
        }

        public void Append(IList<IPlayItem> playItems)
        {
            for(int i = 0; i < playItems.Count; i++)
            {
                this.Add(new BindablePlayItem(playItems[i]));
            }
        }

        bool IPlayItemCollection.Contains(IPlayItem playItem)
        {
            return this.Any(i => i.PlayItem == playItem);
        }

        void IPlayItemCollection.Insert(int index, IPlayItem playItem)
        {
            this.Insert(index, new BindablePlayItem(playItem));
        }

        void IPlayItemCollection.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            this.IsDirty = true;
        }

        public void ValidateTimeRange(DateTime startTime, TimeSpan duration)
        {
            ValidateStartTime(startTime);
            //var e = new TimeValidationEventArgs(startTime);
            //OnValidateStartTime(e);
            //if (!e.IsValid)
            //{
            //    throw new InvalidTimeException(startTime);
            //}

            ValidateTimeRange(startTime, startTime.Add(duration));
        }

        private void ValidateStartTime(DateTime startTime)
        {
            //throw new NotImplementedException();
        }

        private void ValidateTimeRange(DateTime startTime, DateTime stopTime)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var item = this[i].PlayItem;
                if (item.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing || item.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak)
                {
                    var startTime2 = item.StartTime;
                    var stopTime2 = item.CalculatedStopTime;

                    if (stopTime > startTime2 && stopTime2 > startTime)
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        public void ValidateTimeRange(DateTime startTime, TimeSpan duration, IPlayItem excludeItem)
        {
            ValidateStartTime(startTime);
            ValidateTimeRange(startTime, startTime.Add(duration),excludeItem);
        }

        private void ValidateTimeRange(DateTime startTime, DateTime stopTime, IPlayItem excludeItem)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var item = this[i].PlayItem;
                if (item!=excludeItem && 
                    item.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing || item.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak)
                {
                    var startTime2 = item.StartTime;
                    var stopTime2 = item.CalculatedStopTime;

                    if (stopTime > startTime2 && stopTime2 > startTime)
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        public bool CanClear()
        {
            return true;
        }

        public DateTime? GetStartTime()
        {
            return null;
        }

        public bool IsDirty
        {
            get;set;
        }
    }
}
