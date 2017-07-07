using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System.Threading.Tasks;

namespace FCSPlayout.AppInfrastructure
{
    public interface IMediaFileService
    {
        void Add(MediaFileEntity entity, string applicationName);

        void DeleteMediaFile(MediaFileEntity entity, string applicationName);

        PagingItems<MediaFileEntity> GetMediaFiles(MediaItemSearchOptions searchOptions, PagingInfo pagingInfo);

        void UploadFile(string filePath, string destFileName, IBackgroundWorkContext context);
    }

    public static class MediaFileServiceExtensions
    {
        public static Task<PagingItems<MediaFileEntity>> GetMediaFilesAsync(this IMediaFileService mediaFileService, 
            MediaItemSearchOptions searchOptions, PagingInfo pagingInfo)
        {
            return Task.Run(() => mediaFileService.GetMediaFiles(searchOptions, pagingInfo));
        }
    }
}
