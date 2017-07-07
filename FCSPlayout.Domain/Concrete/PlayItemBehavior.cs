using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain.Concrete
{
    public interface DeletePlayItemActionValidator
    {
        void Validate(IPlayItem playItem);
    }

    public abstract class PlayItemBehavior
    {
        internal PlaylistSegmentCollection Segments { get; private set; }
        public virtual void Delete(IPlayItem playItem)
        {
            var segment = FindSegment(playItem); // Note: 这里可以优化。
            if (segment == null)
            {
                throw new InvalidOperationException();
            }

            if (!segment.Remove(playItem))
            {
                throw new InvalidOperationException();
            }
        }

        protected PlaylistSegment FindSegment(IPlayItem playItem)
        {
            return this.Segments.FindLastSegment(s => s.Contains(playItem));
        }
    }

    public class AutoPlayItemBehavior : PlayItemBehavior
    {
        public override void Delete(IPlayItem playItem)
        {
            throw new NotImplementedException();
        }
    }

    public class TimingPlayItemBehavior : PlayItemBehavior
    {
        public override void Delete(IPlayItem playItem)
        {
            throw new NotImplementedException();
        }
    }

    public class TimingBreakPlayItemBehavior : PlayItemBehavior
    {
        public override void Delete(IPlayItem playItem)
        {
            throw new NotImplementedException();
        }
    }
}
