using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class AutoPlaybillItem: PlaybillItem
    {
        public bool IsAutoPadding { get; private set; }

        internal static AutoPlaybillItem CreateAutoPadding(TimeSpan duration)
        {
            IPlaySource playSource = FCSPlayout.Domain.PlaySource.CreateAutoPadding(duration);
            return new AutoPlaybillItem(playSource) { IsAutoPadding = true };
        }

        public AutoPlaybillItem(IPlaySource playSource)
            :base(playSource,PlayScheduleMode.Auto)
        {
        }

        public override IPlaybillItem Clone(PlayRange newRange)
        {
            var result = new AutoPlaybillItem(this.PlaySource.Clone(newRange));
            result.IsAutoPadding = this.IsAutoPadding; // ?
            result.Id = Guid.NewGuid();
            return result;
        }

        public override bool CanMerge(IPlaybillItem playbillItem)
        {
            return this.PlaySource.CanMerge(playbillItem.PlaySource);
        }

        public override IPlaybillItem Merge(IPlaybillItem playbillItem)
        {
            if (!CanMerge(playbillItem))
            {
                throw new InvalidOperationException();
            }

            return new AutoPlaybillItem(this.PlaySource.Merge(playbillItem.PlaySource));
        }

        //protected override PlaybillItem Clone()
        //{
        //    var result = new AutoPlaybillItem(this.PlaySource.Clone());
        //    result.IsAutoPadding = this.IsAutoPadding; // ?
        //    result.Id = this.Id;
        //    return result;
        //}
    }
}
