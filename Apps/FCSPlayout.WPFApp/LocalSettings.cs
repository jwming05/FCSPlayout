using FCSPlayout.AppInfrastructure;
using FCSPlayout.Entities;

namespace FCSPlayout.WPFApp
{
    internal class LocalSettings: LocalSettingsBase
    {
        private static LocalSettings _instance;

        internal static LocalSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalSettings();
                }
                return _instance;
            }
        }

        private LocalSettings()
        {

        }

        public void Initialize()
        {
            PlayoutRepository.Register(this.MachineName, this.ApplicationName);
        }
    }
}
