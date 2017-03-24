using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class TimeConflictException:PlaylistEditException
    {
        public TimeConflictException()
        {
        }
    }
}
