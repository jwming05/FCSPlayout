using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using System.IO;

namespace FCSPlayout.MediaFileImporter
{
    public class FileSystemDestinationStreamCreator : IDestinationStreamCreator
    {
        private static readonly FileSystemDestinationStreamCreator _instance = new FileSystemDestinationStreamCreator();

        public static FileSystemDestinationStreamCreator Instance
        {
            get
            {
                return _instance;
            }
        }

        private FileSystemDestinationStreamCreator()
        {
        }

        public Stream Create(string destFileName, MediaFileStorage fileStorage)
        {
            return new FileStream(MediaFilePathResolver.Current.Resolve(destFileName, fileStorage),
                FileMode.CreateNew, FileAccess.Write, FileShare.None);
        }
    }
}
