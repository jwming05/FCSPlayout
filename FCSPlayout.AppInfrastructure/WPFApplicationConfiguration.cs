using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.AppInfrastructure
{
    public class WPFApplicationConfiguration
    {
        public bool RequireGarbageCollection { get; set; }
        public TimeSpan? GarbageCollectionInterval { get; set; }
    }
}
