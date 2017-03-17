using FCSPlayout.Domain;
using System.Windows;
using System;

namespace FCSPlayout.AppInfrastructure
{
    public abstract class WPFApplicationBase: Application,ITimerAware
    {
        protected WPFApplicationBase()
        {
        }
        protected WPFApplicationBase(WPFApplicationConfiguration configuration)
        {
            if (configuration.RequireGarbageCollection)
            {
                if (configuration.GarbageCollectionInterval != null)
                {
                    this.GarbageCollector = new PeriodicalGarbageCollector(configuration.GarbageCollectionInterval.Value);
                }
                else
                {
                    this.GarbageCollector = new PeriodicalGarbageCollector();
                }
            }
        }

        public PeriodicalGarbageCollector GarbageCollector { get; private set; }

        public void RegisterApplicationExitAware(IApplicationExitAware aware)
        {
        }

        public void OnTimer()
        {
            if (this.GarbageCollector != null)
            {
                this.GarbageCollector.OnTimer();
            }
        }
    }
}
