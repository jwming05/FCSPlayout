using FCSPlayout.Domain;
using FCSPlayout.WPFApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;

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
    }
}
