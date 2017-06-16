using FCSPlayout.Domain;
using System;
using System.IO;

namespace FCSPlayout.AppInfrastructure
{
    public class DefaultFileUploader : IFileUploader
    {
        public DefaultFileUploader(IDestinationStreamManager destinationStreamCreator)
        {
            this.DestinationStreamCreator = destinationStreamCreator;
        }
        public IDestinationStreamManager DestinationStreamCreator { get; private set; }

        public void UploadFile(string filePath, string destFileName, Action<int, object> progressReporter)
        {
            if (DestinationStreamCreator == null)
            {
                throw new InvalidOperationException("未设置DestinationStreamCreator。");
            }

            using (var srcStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    using (Stream destStream = CreateDestinationStream(destFileName, MediaFileStorage.Primary))
                    {
                        using (Stream destStream2 = CreateDestinationStream(destFileName, MediaFileStorage.Secondary))
                        {
                            Copy(srcStream, destStream, destStream2, progressReporter, MediaFileStorage.Primary, MediaFileStorage.Secondary);
                        }
                    }
                }
                catch
                {
                    DeleteDestinationStream(destFileName, MediaFileStorage.Primary);
                    DeleteDestinationStream(destFileName, MediaFileStorage.Secondary);
                    throw;
                }
            }
        }

        private void DeleteDestinationStream(string destFileName, MediaFileStorage fileStorage)
        {
            DestinationStreamCreator.Delete(destFileName, fileStorage);
        }

        private Stream CreateDestinationStream(string destFileName, MediaFileStorage fileStorage)
        {
            return DestinationStreamCreator.Create(destFileName, fileStorage);
        }

        private static void Copy(Stream srcStream, Stream destStream, Stream destStream2,
            Action<int, object> progressReporter, MediaFileStorage locationCategory, MediaFileStorage locationCategory2)
        {
            byte[] buffer = new byte[1024];
            int count = srcStream.Read(buffer, 0, buffer.Length);

            long total = 0;
            long length = srcStream.Length;
            int percent = 0;

            while (count > 0)
            {
                destStream.Write(buffer, 0, count);
                destStream2.Write(buffer, 0, count);

                total += count;

                var newPercent = (int)(((double)total * 100.0) / ((double)length));
                if (newPercent != percent)
                {
                    percent = newPercent;
                    //progressReporter((int)percent, locationCategory);
                    progressReporter(percent, locationCategory);
                    progressReporter(percent, locationCategory2);
                }
                count = srcStream.Read(buffer, 0, buffer.Length);
            }
        }
    }
}
