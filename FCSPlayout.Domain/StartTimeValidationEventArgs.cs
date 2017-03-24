using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class TimeValidationEventArgs
    {
        public TimeValidationEventArgs(DateTime time)
        {
            this.Time = time;
            this.IsValid = true;
        }

        public DateTime Time { get; private set; }
        public bool IsValid
        {
            get;
            set;
        }
    }
}