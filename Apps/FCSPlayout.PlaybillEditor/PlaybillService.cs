using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCSPlayout.Domain;

namespace FCSPlayout.PlaybillEditor
{
    class PlaybillService
    {
        private List<IPlayItem> _playItems;
        private DateTime? maxStopTime = null;

        public bool CanDelete(IPlayItem playItem)
        {
            throw new NotImplementedException();
        }

        public void Delete(IPlayItem playItem)
        {
            BeforeDelete(playItem);

            var builder = new PlaylistBuilder(_playItems);
            builder.RemoveItem(playItem);
            var items = builder.Build(maxStopTime);
            _playItems.Clear();
            foreach (var item in items)
            {
                _playItems.Add(item);
            }

            //OnCommitted();
        }

        private void Rebuild(IPlayItem addItem, IPlayItem removeItem)
        {
        }

        private void BeforeDelete(IPlayItem playItem)
        {
        }
    }
}
