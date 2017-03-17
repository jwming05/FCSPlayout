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
    }
}
