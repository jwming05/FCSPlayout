using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FCSPlayout.Domain
{
    public interface IItemWrapper<TItem> where TItem : class
    {
        TItem WrappedItem { get; }
    }

    public class WrappedItemListAdapter<TWrapper, TItem> : IList<TItem>
        where TWrapper : IItemWrapper<TItem>
        where TItem : class
    {
        private IList<TWrapper> _wrapperList;
        private Func<TItem, TWrapper> _creator;

        public WrappedItemListAdapter(IList<TWrapper> wrapperList,Func<TItem,TWrapper> creator)
        {
            _wrapperList = wrapperList;
            _creator = creator;
        }
        public TItem this[int index]
        {
            get
            {
                return _wrapperList[index].WrappedItem;
            }

            set
            {
                _wrapperList[index] = _creator(value);
            }
        }

        public int Count
        {
            get
            {
                return _wrapperList.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _wrapperList.IsReadOnly;
            }
        }

        public void Add(TItem item)
        {
            _wrapperList.Add(_creator(item));
        }

        public void Clear()
        {
            _wrapperList.Clear();
        }

        public bool Contains(TItem item)
        {
            return _wrapperList.Any(i => this.Equals(i.WrappedItem, item));
        }

        
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            foreach(var item in _wrapperList)
            {
                array[arrayIndex++] = item.WrappedItem;
            }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            foreach(var item in _wrapperList)
            {
                yield return item.WrappedItem;
            }
        }

        public int IndexOf(TItem item)
        {
            for(int i = 0; i < _wrapperList.Count; i++)
            {
                if (this.Equals(_wrapperList[i].WrappedItem, item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, TItem item)
        {
            _wrapperList.Insert(index, _creator(item));
        }

        public bool Remove(TItem item)
        {
            var index = this.IndexOf(item);
            if (index >= 0)
            {
                this.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            _wrapperList.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private bool Equals(TItem item1, TItem item2)
        {
            return object.Equals(item1, item2);
        }
    }
}
