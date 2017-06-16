using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using System.IO;
using System;

namespace FCSPlayout.MediaFileImporter
{
    public class FileSystemDestinationStreamManager : IDestinationStreamManager
    {
        private IMediaFilePathResolver _mediaFilePathResolver;

        //private static readonly FileSystemDestinationStreamManager _instance = new FileSystemDestinationStreamManager();

        //public static FileSystemDestinationStreamManager Instance
        //{
        //    get
        //    {
        //        return _instance;
        //    }
        //}

        //private FileSystemDestinationStreamManager()
        //{
        //}

        public FileSystemDestinationStreamManager(IMediaFilePathResolver mediaFilePathResolver)
        {
            _mediaFilePathResolver = mediaFilePathResolver;
        }
        public Stream Create(string destFileName, MediaFileStorage fileStorage)
        {
            //return new FileStream(MediaFilePathResolver.Current.Resolve(destFileName, fileStorage),
            //    FileMode.CreateNew, FileAccess.Write, FileShare.None);

            return new FileStream(ResolvePath(destFileName, fileStorage),
                FileMode.CreateNew, FileAccess.Write, FileShare.None);
        }

        public void Delete(string destFileName, MediaFileStorage fileStorage)
        {
            var path = ResolvePath(destFileName, fileStorage);
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {

            }
        }

        private string ResolvePath(string destFileName, MediaFileStorage fileStorage)
        {
            return _mediaFilePathResolver.Resolve(destFileName, fileStorage);
        }
    }
}
