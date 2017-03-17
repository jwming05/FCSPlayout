using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.MediaFileImporter
{
    public abstract class UploadStreamCreator
    {
        public abstract Stream Create(string destFileName, MediaFileStorage locationCategory);
    }

    class FileSystemUploadStreamCreator : UploadStreamCreator
    {
        private static readonly FileSystemUploadStreamCreator _instance = new FileSystemUploadStreamCreator();

        internal static FileSystemUploadStreamCreator Instance
        {
            get
            {
                return _instance;
            }
        }

        public override Stream Create(string destFileName, MediaFileStorage locationCategory)
        {
            string filePath = GetFilePath(destFileName, locationCategory);

            return new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        }

        private string GetFilePath(string destFileName, MediaFileStorage locationCategory)
        {
            string dirPath = GetDirectoryPath(locationCategory);
            return System.IO.Path.Combine(dirPath, destFileName);

            //return System.IO.Path.Combine(dirPath, Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(destFileName));
        }

        private string GetDirectoryPath(MediaFileStorage locationCategory)
        {
            string dir = null;
            switch (locationCategory)
            {
                case MediaFileStorage.Primary:
                    dir = @"c:\temp\01";
                    break;
                case MediaFileStorage.Secondary:
                    dir = @"c:\temp\02";
                    break;
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }
    }
}