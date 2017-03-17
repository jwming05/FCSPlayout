using FCSPlayout.Domain;
using System;

namespace FCSPlayout.PlayEngine
{
    public interface IPlayerToken:IDisposable
    {
        TimeSpan Position { get; }
        TimeSpan RemainTime { get; }
        void OnTimer();
        PlayRange LoadRange { get; }

        event EventHandler Stopped;
    }
}