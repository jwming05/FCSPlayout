using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;
using System.IO;

namespace FCSPlayout.AppInfrastructure
{
    internal class FileUploader
    {
        public static IDestinationStreamManager DestinationStreamCreator { get; set; }
        public static void UploadFile(string filePath, string destFileName, Action<int, object> progressReporter)
        {
            if (DestinationStreamCreator == null)
            {
                throw new InvalidOperationException("未设置DestinationStreamCreator。");
            }

            using (var srcStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                UploadFile(srcStream, destFileName, progressReporter, MediaFileStorage.Primary);
                srcStream.Seek(0, SeekOrigin.Begin);
                UploadFile(srcStream, destFileName, progressReporter, MediaFileStorage.Secondary);
            }
        }

        public static void UploadFile2(string filePath, string destFileName, Action<int, object> progressReporter)
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
                            Copy2(srcStream, destStream, destStream2, progressReporter, MediaFileStorage.Primary, MediaFileStorage.Secondary);
                        }

                        //Copy(srcStream, destStream, progressReporter, locationCategory);
                    }
                }
                catch
                {
                    DeleteDestinationStream(destFileName, MediaFileStorage.Primary);
                    DeleteDestinationStream(destFileName, MediaFileStorage.Secondary);
                    throw;
                }

                //UploadFile(srcStream, destFileName, progressReporter, MediaFileStorage.Primary);
                //srcStream.Seek(0, SeekOrigin.Begin);
                //UploadFile(srcStream, destFileName, progressReporter, MediaFileStorage.Secondary);
            }
        }

        private static void DeleteDestinationStream(string destFileName, MediaFileStorage fileStorage)
        {
            DestinationStreamCreator.Delete(destFileName, fileStorage);
        }

        private static void UploadFile(Stream srcStream, string destFileName, Action<int, object> progressReporter,
            MediaFileStorage locationCategory)
        {

            using (Stream destStream = CreateDestinationStream(destFileName, locationCategory))
            {
                Copy(srcStream, destStream, progressReporter, locationCategory);
            }
        }

        private static Stream CreateDestinationStream(string destFileName, MediaFileStorage fileStorage)
        {
            return DestinationStreamCreator.Create(destFileName, fileStorage);
        }

        private static void Copy(Stream srcStream, Stream destStream, Action<int, object> progressReporter, MediaFileStorage locationCategory)
        {
            byte[] buffer = new byte[1024];
            int count = srcStream.Read(buffer, 0, buffer.Length);

            long total = 0;
            long length = srcStream.Length;
            int percent = 0;

            while (count > 0)
            {
                destStream.Write(buffer, 0, count);

                total += count;

                var newPercent = (int)(((double)total * 100.0) / ((double)length));
                if (newPercent != percent)
                {
                    percent = newPercent;
                    //progressReporter((int)percent, locationCategory);
                    progressReporter(percent, locationCategory);
                }


                count = srcStream.Read(buffer, 0, buffer.Length);
            }
        }

        private static void Copy2(Stream srcStream, Stream destStream, Stream destStream2, Action<int, object> progressReporter, MediaFileStorage locationCategory, MediaFileStorage locationCategory2)
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
    public class MediaFileService
    {
        public static void Add(MediaFileEntity entity, string applicationName)
        {
            PlayoutRepository.AddMediaFile(entity, applicationName, UserService.CurrentUser.Id, UserService.CurrentUser.Name);
        }

        public static void DeleteMediaFile(MediaFileEntity entity, string applicationName)
        {
            PlayoutRepository.DeleteMediaFile(entity, applicationName, UserService.CurrentUser.Id, UserService.CurrentUser.Name);
        }

        public static PagingItems<MediaFileEntity> GetMediaFiles(MediaItemSearchOptions searchOptions, PagingInfo pagingInfo)
        {
            return PlayoutRepository.GetMediaFiles(searchOptions, pagingInfo);
        }

        public static void UploadFile(string filePath, string destFileName, IBackgroundWorkContext context)
        {
            //if (DestinationStreamCreator == null)
            //{
            //    throw new InvalidOperationException("未设置DestinationStreamCreator。");
            //}

            //FileUploader.UploadFile(filePath, destFileName, (i, s) => context.SetProgress(i, s));
            FileUploader.UploadFile2(filePath, destFileName, (i, s) => context.SetProgress(i, s));
        }

        public static IDestinationStreamManager DestinationStreamCreator
        {
            get { return FileUploader.DestinationStreamCreator; }
            set { FileUploader.DestinationStreamCreator=value; }
        }
    }
}
