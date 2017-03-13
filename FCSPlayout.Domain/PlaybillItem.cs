using System;

namespace FCSPlayout.Domain
{
    public abstract class PlaybillItem : IPlaybillItem
    {
        protected PlaybillItem(IPlaySource playSource, PlaybillItemCategory category)
        {
            this.PlaySource = playSource;
            this.Category = category;
        }

        public PlaybillItemCategory Category
        {
            get;private set;
        }

        public IPlaySource PlaySource
        {
            get;private set;
        }

        public DateTime? StartTime
        {
            get;protected set;
        }
    }
}
