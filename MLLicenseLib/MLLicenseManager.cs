using System;

namespace MLLicenseLib
{
    public class MLLicenseManager
    {
        private static readonly MLLicenseManager _instance = new MLLicenseManager();
        public static MLLicenseManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private readonly MPlatformLicense _license;
        private ITimer _timer;

        private MLLicenseManager()
        {
            _license = new MPlatformLicense();
        }

        public void Timer()
        {
            _license.Timer();
        }

        public void SetTimer(ITimer timer)
        {
            if (_timer != timer)
            {
                if (_timer != null)
                {
                    _timer.Tick -= OnTimer_Tick;
                }
                _timer = timer;
                if (_timer != null)
                {
                    _timer.Tick += OnTimer_Tick;
                }
            }
            
        }

        private void OnTimer_Tick(object sender, EventArgs e)
        {
            _license.Timer();
        }

        public interface ITimer
        {
            event EventHandler Tick;
        }
    }
}
