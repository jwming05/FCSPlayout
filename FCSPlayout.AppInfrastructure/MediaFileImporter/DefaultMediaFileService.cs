using FCSPlayout.Domain;
using FCSPlayout.Entities;

namespace FCSPlayout.AppInfrastructure
{
    public class DefaultMediaFileService : IMediaFileService
    {
        private IFileUploader _fileUploader;

        public DefaultMediaFileService(IFileUploader fileUploader)
        {
            _fileUploader = fileUploader;
        }
        public void Add(MediaFileEntity entity, string applicationName)
        {
            PlayoutRepository.AddMediaFile(entity, applicationName, UserService.CurrentUser.Id, UserService.CurrentUser.Name);
        }

        public void DeleteMediaFile(MediaFileEntity entity, string applicationName)
        {
            PlayoutRepository.DeleteMediaFile(entity, applicationName, UserService.CurrentUser.Id, UserService.CurrentUser.Name);
        }

        public PagingItems<MediaFileEntity> GetMediaFiles(MediaItemSearchOptions searchOptions, PagingInfo pagingInfo)
        {
            return PlayoutRepository.GetMediaFiles(searchOptions, pagingInfo);
        }

        public void UploadFile(string filePath, string destFileName, IBackgroundWorkContext context)
        {
            _fileUploader.UploadFile(filePath, destFileName, (i, s) => context.SetProgress(i, s));
        }
    }
}
