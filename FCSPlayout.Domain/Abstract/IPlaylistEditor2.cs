using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public interface IPlaylistEditor2:IPlaylistEditor
    {
        bool ForcePlay(IPlayItem playItem, DateTime startTime);
    }
}
