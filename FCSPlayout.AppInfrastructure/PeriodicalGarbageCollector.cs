using FCSPlayout.Domain;
using System;

namespace FCSPlayout.AppInfrastructure
{
    public class PeriodicalGarbageCollector : ITimerAware
    {
        private DateTime _lastCollectTime;

        public PeriodicalGarbageCollector(TimeSpan duration)
        {
            this.CollectPeroid = duration;
            _lastCollectTime = DateTime.Now;
        }

        public PeriodicalGarbageCollector()
            : this(TimeSpan.FromMinutes(10))
        {

        }
        public TimeSpan CollectPeroid { get; private set; }

        public void OnTimer()
        {
            if (ElapsedTime() >= this.CollectPeroid)
            {
                Collect();
            }
        }

        private void Collect()
        {
            GC.Collect();
            _lastCollectTime = DateTime.Now;
        }

        private TimeSpan ElapsedTime()
        {
            return DateTime.Now.Subtract(_lastCollectTime);
        }
    }
}
