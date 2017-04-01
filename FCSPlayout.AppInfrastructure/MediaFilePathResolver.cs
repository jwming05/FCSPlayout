using FCSPlayout.Domain;

namespace FCSPlayout.AppInfrastructure
{
    public class MediaFilePathResolver: IMediaFilePathResolver
    {
        private static IMediaFilePathResolver _current=new MediaFilePathResolver(MediaFileStorage.Primary);

        public static IMediaFilePathResolver Current
        {
            get
            {
                return _current;
            }

            set
            {
                _current = value;
            }
        }

        public MediaFilePathResolver(MediaFileStorage currentStorage)
        {
            this.CurrentStorage = currentStorage;
        }

        public string GetDirectory(MediaFileStorage fileStorage)
        {
            return fileStorage == MediaFileStorage.Primary ? System.Configuration.ConfigurationManager.AppSettings["primaryStorage"] :
                    System.Configuration.ConfigurationManager.AppSettings["secondaryStorage"];
        }

        public MediaFileStorage CurrentStorage
        {
            get;set;
        }
    }
}
