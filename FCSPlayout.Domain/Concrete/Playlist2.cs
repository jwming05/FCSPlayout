﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class Playlist2 : Playlist, IPlaylist2
    {
        private IPlayItemCollection2 _playItemCollection;

        public Playlist2(IPlayItemCollection2 playItemCollection)
            :base(playItemCollection)
        {
            _playItemCollection = playItemCollection;
        }
        public TimeSpan Duration
        {
            get
            {
                if (this.Count > 0)
                {
                    return this[this.Count - 1].CalculatedStopTime.Subtract(this.StartTime.Value);
                }

                return TimeSpan.Zero;
            }
        }

        public DateTime? StartTime
        {
            get
            {
                if (this.Count > 0)
                {
                    return this[0].StartTime;
                }

                return null; //???
            }
        }

        public bool CanEnterLoop(IPlayItem playItem)
        {
            return false; // throw new NotImplementedException();
        }

        public bool CanForcePlay(IPlayItem playItem)
        {
            return _playItemCollection.CanForcePlay(playItem);
        }

        //public void ForcePlay(IPlayItem playItem)
        //{
        //    if (CanForcePlay(playItem))
        //    {
        //        _playItemCollection.ForcePlay(playItem);
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException();
        //    }
        //}

        protected override PlaylistEditor CreateEditor()
        {
            return new PlaylistEditor2(this);
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
            _playItemCollection.Start();
        }

        public void Stop()
        {
            _playItemCollection.Stop();
        }

        public IPlayItem NextItem
        {
            get { return _playItemCollection.NextPlayItem; }
            set
            {
                _playItemCollection.NextPlayItem = value;
            }
        }
        public IPlayItem CurrentItem { get; set; }

        public override bool CanDelete(IPlayItem playItem)
        {
            return _playItemCollection.CanDelete(playItem);
        }

        protected override void OnEditorDisposed(long editId)
        {
            _playItemCollection.SendPlaylistSyncRequest();
            //base.OnEditorDisposed(editId);
        }
    }
}