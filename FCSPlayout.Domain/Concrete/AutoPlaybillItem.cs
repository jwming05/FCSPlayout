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

        //protected override PlaybillItem Clone()
        //{
        //    var result = new AutoPlaybillItem(this.PlaySource.Clone());
        //    result.IsAutoPadding = this.IsAutoPadding; // ?
        //    result.Id = this.Id;
        //    return result;
        //}
    }
}
