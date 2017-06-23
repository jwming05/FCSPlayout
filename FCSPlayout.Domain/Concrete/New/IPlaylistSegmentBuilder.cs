using System.Collections.Generic;

namespace FCSPlayout.Domain
{
    public interface IPlaylistSegmentBuilder
    {
        IEnumerable<IPlayItem> Build();
    }
}
