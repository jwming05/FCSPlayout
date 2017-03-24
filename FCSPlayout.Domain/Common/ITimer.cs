using System;

namespace FCSPlayout.Domain
{
    public interface ITimer
    {
        event EventHandler Tick;

        void Start();
        void Stop();
    }
}
