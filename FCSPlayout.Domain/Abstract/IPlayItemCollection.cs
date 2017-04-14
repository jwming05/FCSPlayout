using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public interface IPlayItemCollection
    {
        int Count { get; }
        IPlayItem this[int index] { get; }
        void RemoveAt(int index);
        void Insert(int index, IPlayItem playItem);
        bool Contains(IPlayItem playItem);
        void Clear();
        void Append(IList<IPlayItem> playItems);

        bool IsDirty { get; set; }

        void ValidateTimeRange(DateTime startTime, TimeSpan duration);
        void ValidateTimeRange(DateTime startTime, TimeSpan duration, IPlayItem excludeItem);
        bool CanClear();
        DateTime? GetStartTime();
        DateTime? GetStopTime();
    }
}
