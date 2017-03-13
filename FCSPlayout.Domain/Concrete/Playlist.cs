using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class Playlist:IPlaylist
    {
        private IList<IPlayItem> _playItems;

        public event EventHandler<TimeValidationEventArgs> ValidateStartTime;

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

        

        public IPlaylistEditor Edit()
        {
            throw new NotImplementedException();
        }

        public PlaylistSegment FindFirstSegment(Func<PlaylistSegment,bool> predicate)
        {
            PlaylistSegment segment=GetFirstSegment();
            if(segment.IsValid && predicate(segment))
            {
                return segment;
            }

            for (int i = 1; i < _playItems.Count; i++)
            {
                var item = _playItems[i];
                if (item.PlaybillItem.Category == PlaybillItemCategory.Timing)
                {
                    segment = PlaylistSegment.CreateValid(i, item);

                    if (predicate(segment))
                    {
                        return segment;
                    }
                }
            }

            return PlaylistSegment.Invalid;
        }

        public PlaylistSegment FindLastSegment(Func<PlaylistSegment, bool> predicate)
        {
            PlaylistSegment segment;
            for (int i = _playItems.Count - 1; i > 0; i--)
            {
                var item = _playItems[i];
                if (item.PlaybillItem.Category == PlaybillItemCategory.Timing)
                {
                    segment = PlaylistSegment.CreateValid(i, item);

                    if (predicate(segment))
                    {
                        return segment;
                    }
                }
            }

            segment = GetFirstSegment();
            if (segment.IsValid && predicate(segment))
            {
                return segment;
            }

            return PlaylistSegment.Invalid;
        }

        public PlaylistSegment GetNextSegment(PlaylistSegment segment)
        {
            if (!segment.IsValid) throw new InvalidOperationException();

            for (int i = segment.HeadIndex+1; i < _playItems.Count; i++)
            {
                var item = _playItems[i];
                if (item.PlaybillItem.Category == PlaybillItemCategory.Timing)
                {
                    return PlaylistSegment.CreateValid(i, item);
                }
            }

            return PlaylistSegment.Invalid;
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
            var e = new TimeValidationEventArgs(startTime);
            OnValidateStartTime(e);
            if (!e.IsValid)
            {
                throw new InvalidTimeException(startTime);
            }

            ValidateTimeRange(startTime, startTime.Add(duration));
        }

        private void ValidateTimeRange(DateTime startTime, DateTime stopTime)
        {
            for(int i = 0; i < _playItems.Count; i++)
            {
                var item = _playItems[i];
                if (item.PlaybillItem.Category == PlaybillItemCategory.Timing || item.PlaybillItem.Category==PlaybillItemCategory.TimingBreak)
                {
                    var startTime2 = item.StartTime;
                    var stopTime2 = item.GetStopTime();

                    if(stopTime>startTime2 && stopTime2 > startTime)
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        private PlaylistSegment GetFirstSegment()
        {
            PlaylistSegment segment = PlaylistSegment.Invalid;
            if (_playItems.Count > 0)
            {
                var first = _playItems[0];

                if (first.PlaybillItem.Category != PlaybillItemCategory.Timing)
                {
                    segment = PlaylistSegment.CreateValid(-1, null);
                    segment.StartTime = first.StartTime;
                }
                else
                {
                    segment = PlaylistSegment.CreateValid(0, first);
                }
            }

            return segment;
        }

        public bool Contains(IPlayItem playItem)
        {
            return _playItems.Contains(playItem);
        }
    }

    
}
