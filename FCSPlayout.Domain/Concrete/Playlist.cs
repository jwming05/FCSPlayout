using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class Playlist:IPlaylist
    {
        //private List<IPlayItem> _playItems=new List<IPlayItem>();
        private PlaylistEditor _editor;
        private IPlayItemCollection _playItems;

        public event EventHandler<TimeValidationEventArgs> ValidateStartTime;

        public Playlist(IPlayItemCollection playItems)
        {
            _playItems = playItems;
        }

        private void OnValidateStartTime(TimeValidationEventArgs e)
        {
            if (ValidateStartTime != null)
            {
                ValidateStartTime(this, e);
            }
        }

        public int Count
        {
            get
            {
                return _playItems.Count;
            }
        }

        public IPlayItem this[int index]
        {
            get
            {
                return _playItems[index];
            }
        }

        public IPlaylistEditor Edit()
        {
            if (_editor != null)
            {
                throw new InvalidOperationException();
            }

            _editor = CreateEditor();
            _editor.Disposed += Editor_Disposed;
            return _editor;
        }

        protected virtual PlaylistEditor CreateEditor()
        {
            return new PlaylistEditor(this);
        }
        private void Editor_Disposed(object sender, EventArgs e)
        {
            long editId = _editor.Id;
            if (_playItems.IsDirty)
            {
                OnEditorDisposed(editId);
                _playItems.IsDirty = false;
            }
            _editor = null;
        }

        protected virtual void OnEditorDisposed(long editId)
        {

        }

        public IList<IPlayItem> GetPlayItems(int beginIndex, int endIndex)
        {
            List<IPlayItem> playItems = new List<IPlayItem>();
            for(int i = beginIndex; i <= endIndex; i++)
            {
                playItems.Add(_playItems[i]);
            }
            return playItems;
        }

        public void Update(int index, int length, IList<IPlayItem> newItems)
        {
            for(int i = 0; i < length; i++)
            {
                _playItems.RemoveAt(index);
            }

            for(int i = newItems.Count - 1; i >= 0; i--)
            {
                _playItems.Insert(index, newItems[i]);
            }
        }

        public void ValidateTimeRange(DateTime startTime, TimeSpan duration)
        {
            _playItems.ValidateTimeRange(startTime,duration);

            //var e = new TimeValidationEventArgs(startTime);
            //OnValidateStartTime(e);
            //if (!e.IsValid)
            //{
            //    throw new InvalidTimeException(startTime);
            //}

            //ValidateTimeRange(startTime, startTime.Add(duration));
        }

        public void ValidateTimeRange(DateTime startTime, TimeSpan duration, IPlayItem excludeItem)
        {
            _playItems.ValidateTimeRange(startTime, duration, excludeItem);
        }

        //private void ValidateTimeRange(DateTime startTime, DateTime stopTime, IPlayItem excludeItem)
        //{

        //}

        public bool Contains(IPlayItem playItem)
        {
            return _playItems.Contains(playItem);
        }

        public int FindLastIndex(Func<IPlayItem, bool> predicate)
        {
            return FindLastIndex(this.Count - 1, predicate);
        }

        public int FindFirstIndex(Func<IPlayItem, bool> predicate)
        {
            return FindFirstIndex(0, predicate);
        }

        public int FindLastIndex(int lastStartIndex, Func<IPlayItem, bool> predicate)
        {
            for(int i = lastStartIndex; i >= 0; i--)
            {
                if (predicate(this[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public int FindFirstIndex(int startIndex, Func<IPlayItem, bool> predicate)
        {
            for(int i = startIndex; i < this.Count; i++)
            {
                if (predicate(this[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Clear()
        {
            _playItems.Clear();
        }

        public void Append(IList<IPlayItem> playItems)
        {
            _playItems.Append(playItems);
        }

        public virtual bool CanDelete(IPlayItem playItem)
        {
            return this.Contains(playItem);
        }

        public bool IsLocked(IPlayItem playItem)
        {
            throw new NotImplementedException();
        }

        public bool EditLocked(IPlayItem playItem)
        {
            throw new NotImplementedException();
        }

        public virtual bool CanClear()
        {
            return _playItems.CanClear();
            //throw new NotImplementedException();
        }

        public virtual DateTime? GetStartTime()
        {
            return _playItems.GetStartTime();
        }
    }
}
