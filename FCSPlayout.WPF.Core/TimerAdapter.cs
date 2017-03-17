using FCSPlayout.Domain;
using System;
using System.Windows.Threading;

namespace FCSPlayout.WPF.Core
{
    public class TimerAdapter : ITimer
    {
        private DispatcherTimer _timer;

        public TimerAdapter(DispatcherTimer timer)
        {
            _timer = timer;
        }

        public event EventHandler Tick
        {
            add { _timer.Tick += value; }
            remove { _timer.Tick -= value; }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public TimeSpan Interval
        {
            get { return _timer.Interval; }
            set
            {
                _timer.Interval = value;
            }
        }
    }
}
