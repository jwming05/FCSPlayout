using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public abstract class ApplicationService
    {
        public static ApplicationService Current { get; set; }

        public abstract void RegisterApplicationExitAware(IApplicationExitAware aware);
    }
}
