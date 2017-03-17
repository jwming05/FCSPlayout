using FCSPlayout.Domain;
using FCSPlayout.PlayEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FCSPlayout.WPFApp.Models
{
    public class ObservablePlayItemCollection : ObservableCollection<IPlayItemAdapter>, IPlaylist //, IPlayItemCollection
    {
        private Guid _propertyCacheUpdateToken=Guid.Empty;
        private Guid _collectionChangeToken= Guid.NewGuid();

        private TimeSpan _durationCache=TimeSpan.Zero;
        private DateTime _startTimeCache= DateTime.MaxValue;
        private DateTime _stopTimeCache = DateTime.MaxValue;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            _collectionChangeToken = Guid.NewGuid();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            _collectionChangeToken = Guid.NewGuid();
        }

        public TimeSpan Duration
        {
            get
            {
                if (this.Count > 0)
                {
                    TryUpdatePropertyCache();
                    return _durationCache;
                }
                return TimeSpan.Zero;
            }
        }

        public DateTime StartTime
        {
            get
            {
                if (this.Count > 0)
                {
                    TryUpdatePropertyCache();
                    return _startTimeCache;
                }
                else
                {
                    return DateTime.MaxValue;
                }
            }
        }

        public DateTime StopTime
        {
            get
            {
                if (this.Count > 0)
                {
                    TryUpdatePropertyCache();
                    return _stopTimeCache;
                }
                else
                {
                    return DateTime.MaxValue;
                }
            }
        }

        

        

        

        IPlayItem IPlaylist.this[int index]
        {
            get
            {
                return base[index].Adaptee;
            }
        }

        //public double Position
        //{
        //    get;
        //    private set;
        //}

        private void TryUpdatePropertyCache()
        {
            if (_propertyCacheUpdateToken != _collectionChangeToken)
            {
                _startTimeCache = ((IEnumerable<IPlayItem>)this).Min(i => i.StartTime);
                _stopTimeCache = ((IEnumerable<IPlayItem>)this).Max(i => i.StopTime);
                _durationCache = _stopTimeCache.Subtract(_startTimeCache);

                _propertyCacheUpdateToken = _collectionChangeToken;
            }
        }

        protected override void InsertItem(int index, IPlayItemAdapter item)
        {
            //if (item != null && !(item is BindablePlayItem))
            //{
            //    item = new BindablePlayItem(item);
            //}
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, IPlayItemAdapter item)
        {
            //if (item != null && !(item is BindablePlayItem))
            //{
            //    item = new BindablePlayItem(item);
            //}
            base.SetItem(index, item);
        }

        

        IEnumerator<IPlayItem> IEnumerable<IPlayItem>.GetEnumerator()
        {
            for(int i = 0; i < this.Count; i++)
            {
                yield return base[i].Adaptee;
            }
        }

        public IPlayItem FindPlayItem(DateTime currentStopTime)
        {
            throw new NotImplementedException();
        }

        //public new void Add(IPlayItem item)
        //{
        //    if (this.Count > 0)
        //    {
        //        var lastItem = this[this.Count - 1];
        //        TimeSpan duration = item.ActualStartTime.Subtract(lastItem.ActualStopTime);
        //        if (duration > TimeSpan.FromMilliseconds(200))
        //        {
        //            base.Add(new BindablePlayItem(PlayItem.CreateNull(lastItem.ActualStopTime, duration)));
        //        }
        //    }
        //    base.Add(new BindablePlayItem(item));
        //}
    }
}
