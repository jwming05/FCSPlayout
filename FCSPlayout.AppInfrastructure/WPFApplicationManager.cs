using FCSPlayout.Domain;
using System.Windows;

namespace FCSPlayout.AppInfrastructure
{
    public class WPFApplicationManager:ITimerAware
    {
        private static readonly WPFApplicationManager _current = new WPFApplicationManager();

        public static WPFApplicationManager Current
        {
            get
            {
                return _current;
            }
        }

        public void OnTimer()
        {
            this.CurrentApplication.OnTimer();
        }

        public WPFApplicationBase CurrentApplication
        {
            get { return (WPFApplicationBase)Application.Current; }
        }

        public void RegisterApplicationExitAware(IApplicationExitAware aware)
        {
            this.CurrentApplication.RegisterApplicationExitAware(aware);
        }
    }
}
