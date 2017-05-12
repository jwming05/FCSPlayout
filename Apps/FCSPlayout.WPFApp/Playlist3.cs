using System;
using FCSPlayout.Domain;
using FCSPlayout.PlayEngine;

namespace FCSPlayout.WPFApp
{
    internal class Playlist3 : IPlaylist3
    {
        private PlayItemCollection _playItemCollection;
        public event EventHandler Changed;

        public Playlist3(PlayItemCollection playItemCollection)
        {
            _playItemCollection = playItemCollection;
            _playItemCollection.Committed += PlayItemCollection_Committed;
        }

        private void PlayItemCollection_Committed(object sender, EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }

        public IPlayItem CurrentItem
        {
            get { return _playItemCollection.CurrentItem; }
            set
            {
                _playItemCollection.CurrentItem = value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                var start = _playItemCollection.GetStartTime();
                var stop = _playItemCollection.GetStopTime();
                if(stop!=null && start != null)
                {
                    return stop.Value.Subtract(start.Value);
                }
                return TimeSpan.Zero;
            }
        }

        public IPlayItem NextItem
        {
            get { return _playItemCollection.NextItem; }
            set
            {
                _playItemCollection.NextItem=value;
            }
        }

        public DateTime? StartTime
        {
            get
            {
                return _playItemCollection.GetStartTime();
            }
        }

        public IPlayItem GetNextPlayItem(DateTime minStartTime, DateTime maxStartTime)
        {
            return _playItemCollection.GetNextPlayItem(minStartTime, maxStartTime);
        }

        public void OnTimer()
        {
            _playItemCollection.OnTimer();
        }

        public void Start()
        {
            _playItemCollection.OnStart();
        }

        public void Stop()
        {
            _playItemCollection.OnStop();
        }
    }
}