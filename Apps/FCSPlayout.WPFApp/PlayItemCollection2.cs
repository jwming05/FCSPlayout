using FCSPlayout.Domain;
using FCSPlayout.WPFApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPFApp
{
    public class PlayItemCollection2 : ObservableCollection<BindablePlayItem>, IPlayItemCollection2
    {
        private ListView _listView;

        IPlayItem IPlayItemCollection.this[int index]
        {
            get
            {
                if (_listView != null)
                {
                    return _listView[index].PlayItem;
                }
                else
                {
                    return this[index].PlayItem;
                }
                
            }
        }

        int IPlayItemCollection.Count
        {
            get
            {
                if (_listView != null)
                {
                    return _listView.Count;
                }
                else
                {
                    return this.Count;
                }
                
            }
        }

        void IPlayItemCollection.Append(IList<IPlayItem> playItems)
        {
            if (_listView != null)
            {
                for (int i = 0; i < playItems.Count; i++)
                {
                    _listView.Add(new BindablePlayItem(playItems[i]));
                }

                //_listView.Append(playItems);
            }
            else
            {
                for(int i = 0; i < playItems.Count; i++)
                {
                    this.Add(new BindablePlayItem(playItems[i]));
                }
            }
        }

        void IPlayItemCollection.Clear()
        {
            if (_listView != null)
            {
                _listView.Clear();
            }
            else
            {
                this.Clear();
            }
        }

        bool IPlayItemCollection.Contains(IPlayItem playItem)
        {
            if (_listView != null)
            {
                return _listView.Contains(playItem);
            }
            else
            {
                return this.Any(i=>i.PlayItem==playItem);
            }
        }

        IPlayItem IPlayItemCollection2.GetNextPlayItem(DateTime minStartTime, DateTime maxStartTime)
        {
            if (_listView != null)
            {
                return _listView.GetNextPlayItem(minStartTime, maxStartTime);
            }
            else
            {
                return null; // this.Count;
            }
        }

        void IPlayItemCollection.Insert(int index, IPlayItem playItem)
        {
            if (_listView != null)
            {
                _listView.Insert(index, new BindablePlayItem(playItem));
            }
            else
            {
                this.Insert(index, new BindablePlayItem(playItem));
            }
        }

        void IPlayItemCollection2.OnTimer()
        {
            if (_listView != null)
            {
                _listView.OnTimer();
            }
            
        }

        void IPlayItemCollection.RemoveAt(int index)
        {
            if (_listView != null)
            {
                _listView.RemoveAt(index);
            }
            else
            {
                this.RemoveAt(index);
            }
        }

        void IPlayItemCollection2.Start()
        {
            _listView = new ListView(this);
        }

        void IPlayItemCollection2.Stop()
        {
            _listView = null;
        }

        class ListView
        {
            private IList<BindablePlayItem> _playItemList;
            private int _offset;

            public ListView(IList<BindablePlayItem> playItemList)
            {
                _offset = 0;
                _playItemList = playItemList;
            }

            private int CalcIndex(int index)
            {
                return _offset + index;
            }
            public BindablePlayItem this[int index]
            {
                get
                {
                    return _playItemList[CalcIndex(index)];
                }
            }

            public int Count
            {
                get { return _playItemList.Count - _offset; }
            }

            public void RemoveAt(int index)
            {
                _playItemList.RemoveAt(CalcIndex(index));
            }

            public void OnTimer()
            {
            }

            public void Insert(int index, BindablePlayItem bindablePlayItem)
            {
                _playItemList.Insert(CalcIndex(index),bindablePlayItem);
            }

            public void Clear()
            {
                for(int i = _playItemList.Count - 1; i >= _offset; i--)
                {
                    _playItemList.RemoveAt(i);
                }
            }

            

            public bool Contains(IPlayItem playItem)
            {
                for(int i = _offset; i < _playItemList.Count; i++)
                {
                    if (_playItemList[i].PlayItem == playItem)
                    {
                        return true;
                    }
                }
                return false;
            }

            public void Add(BindablePlayItem bindablePlayItem)
            {
                _playItemList.Add(bindablePlayItem);
            }

            public IPlayItem GetNextPlayItem(DateTime minStartTime, DateTime maxStartTime)
            {
                IPlayItem result = null;
                while (this.Count>0)
                {
                    IPlayItem playItem = this.Peek();
                    if (playItem.StartTime > maxStartTime)
                    {
                        break;
                    }

                    TimeSpan expiredDuration=TimeSpan.Zero;
                    if (playItem.StartTime < minStartTime)
                    {
                        expiredDuration = minStartTime.Subtract(playItem.StartTime);
                    }

                    if (playItem.PlayDuration- expiredDuration < PlayoutConfiguration.Current.MinPlayDuration)
                    {
                        this.Take();
                        continue;
                    }
                    else
                    {
                        result = playItem;
                        break;
                    }       
                }
                return result;
            }

            private void Take()
            {
                _offset++;
            }

            private IPlayItem Peek()
            {
                return this[0].PlayItem;
            }
        }
    }
}
