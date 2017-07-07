using FCSPlayout.Domain;
using System.Configuration;

namespace FCSPlayout.AppInfrastructure
{
    public class FakeAppSettings : IAppSettings
    {
        public int MediaFileThumbnailWidth
        {
            get
            {
                return 200;
            }
        }

        public string PrimaryMediaFileStoragePath
        {
            get
            {
                return ConfigurationManager.AppSettings["primaryStorage"];
            }
        }

        public string SecondaryMediaFileStoragePath
        {
            get
            {
                return ConfigurationManager.AppSettings["secondaryStorage"];
            }
        }
    }
}
