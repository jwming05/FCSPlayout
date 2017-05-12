using FCSPlayout.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.PlayEngine
{
    public interface IPlaylist3
    {
        IPlayItem NextItem { get; set; }
        IPlayItem CurrentItem { get; set; }
        void Start();
        void Stop();

        void OnTimer();

        IPlayItem GetNextPlayItem(DateTime minStartTime, DateTime maxStartTime);

        TimeSpan Duration { get; }
        DateTime? StartTime { get; }

        event EventHandler Changed;
    }
}
