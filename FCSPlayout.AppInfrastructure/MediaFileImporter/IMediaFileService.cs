using FCSPlayout.Domain;
using FCSPlayout.Entities;

namespace FCSPlayout.AppInfrastructure
{
    public interface IMediaFileService
    {
        void Add(MediaFileEntity entity, string applicationName);

        void DeleteMediaFile(MediaFileEntity entity, string applicationName);

        PagingItems<MediaFileEntity> GetMediaFiles(MediaItemSearchOptions searchOptions, PagingInfo pagingInfo);

        void UploadFile(string filePath, string destFileName, IBackgroundWorkContext context);

        //public IDestinationStreamManager DestinationStreamCreator
        //{
        //    get { return FileUploader.DestinationStreamCreator; }
        //    set { FileUploader.DestinationStreamCreator = value; }
        //}
    }
}
