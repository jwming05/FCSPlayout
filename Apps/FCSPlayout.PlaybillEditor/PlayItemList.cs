using FCSPlayout.WPFApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FCSPlayout.PlaybillEditor
{
    //class PlayItemList:IList<PlayItem>
    //{
    //    private ObservableCollection<BindablePlayItem> _bindableItems;

    //    public PlayItemList(ObservableCollection<BindablePlayItem> bindableItems)
    //    {
    //        _bindableItems = bindableItems;
    //    }

    //    public PlayItem this[int index]
    //    {
    //        get
    //        {
    //            return _bindableItems[index].PlayItem;
    //        }

    //        set
    //        {
    //            _bindableItems[index] = value==null ? null : new BindablePlayItem(value);
    //        }
    //    }

    //    public int Count
    //    {
    //        get
    //        {
    //            return _bindableItems.Count;
    //        }
    //    }

    //    public bool IsReadOnly
    //    {
    //        get
    //        {
    //            return false;
    //        }
    //    }

    //    public void Add(PlayItem item)
    //    {
    //        if (item == null)
    //        {
    //            _bindableItems.Add(null);
    //        }
    //        else
    //        {
    //            _bindableItems.Add(new BindablePlayItem(item));
    //        }
    //    }

    //    public void Clear()
    //    {
    //        _bindableItems.Clear();
    //    }

    //    public bool Contains(PlayItem item)
    //    {
    //        return _bindableItems.Any(i=>i.PlayItem==item);
    //    }

    //    public void CopyTo(PlayItem[] array, int arrayIndex)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IEnumerator<PlayItem> GetEnumerator()
    //    {
    //        foreach(var item in _bindableItems)
    //        {
    //            yield return item.PlayItem;
    //        }
    //    }

    //    public int IndexOf(PlayItem item)
    //    {
    //        for(int i = 0; i < _bindableItems.Count; i++)
    //        {
    //            if (_bindableItems[i].PlayItem == item) return i;
    //        }

    //        return -1;
    //    }

    //    public void Insert(int index, PlayItem item)
    //    {
    //        if (item != null)
    //        {
    //            _bindableItems.Insert(index, new BindablePlayItem(item));
    //        }
    //        else
    //        {
    //            _bindableItems.Insert(index, null);
    //        }
            
    //    }

    //    public bool Remove(PlayItem item)
    //    {
    //        var idx = IndexOf(item);
    //        if (idx >= 0)
    //        {
    //            this.RemoveAt(idx);
    //            return true;
    //        }
    //        return false;
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        _bindableItems.RemoveAt(index);
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.GetEnumerator();
    //    }
    //}
}
