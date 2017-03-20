using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public interface IPlayItemCollection2 : IPlayItemCollection
    {
        IPlayItem GetNextPlayItem(DateTime minStartTime, DateTime maxStartTime);
        void Start();
        void Stop();
        void OnTimer();

        IPlayItem NextPlayItem { get; set; }
        IPlayItem CurrentPlayItem { get; set; }

        bool CanForcePlay(IPlayItem playItem);
        //void ForcePlay(IPlayItem playItem);

        bool CanDelete(IPlayItem playItem);

        void SendPlaylistSyncRequest();
    }
}
