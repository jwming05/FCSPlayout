using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class AutoPlaybillItem: PlaybillItem
    {
        public bool IsAutoPadding { get; private set; }

        internal static AutoPlaybillItem CreateAutoPadding(TimeSpan duration)
        {
            IPlaySource playSource = FCSPlayout.Domain.PlaySource.CreateAutoPadding(duration);
            return new AutoPlaybillItem(playSource) { IsAutoPadding = true };
        }

        public AutoPlaybillItem(IPlaySource playSource)
            :base(playSource,PlaybillItemCategory.Auto)
        {
        }
    }
}
