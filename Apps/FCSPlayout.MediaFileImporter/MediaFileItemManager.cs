using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FCSPlayout.MediaFileImporter
{
    public class MediaFileItemManager
    {
        public MediaFileItemManager(IUserService userService, 
            IImageSourceDecoder imageSourceDecoder, IImagePlaceholderProvider placeholderProvider, 
            IMediaFileInfoExtractor mediaFileInfoExtractor, IMediaFilePathResolver filePathResolver)
        {
            this.UserService = userService;
            this.MediaFileInfoExtractor = mediaFileInfoExtractor;
            this.FilePathResolver = filePathResolver;

            BindableMediaFileItem.ImageSourceDecoder = imageSourceDecoder;
            BindableMediaFileItem.PlaceholderImage = placeholderProvider.Placeholder;
        }

        public BindableMediaFileItem Create(MediaFileEntity item)
        {
            return new BindableMediaFileItem(item, this.ResolvePath(item.FileName));
        }

        private string ResolvePath(string fileName)
        {
            return this.FilePathResolver.Resolve(fileName);
        }

        private IMediaFilePathResolver FilePathResolver { get; set; }

        public BindableMediaFileItem Create(string fileName)
        {
            var mediaInfo = this.MediaFileInfoExtractor.GetMediaFileInfo(fileName, 150);

            return new BindableMediaFileItem(fileName, this.UserService.CurrentUser.Id, mediaInfo.Duration,mediaInfo.ThumbnailBytes);    
        }

        public Task<BindableMediaFileItem> CreateAsync(string fileName)
        {
            return Task.Run(() => Create(fileName));
        }

        public IUserService UserService
        {
            get; private set;
        }

        public IMediaFileInfoExtractor MediaFileInfoExtractor { get; private set; }

        public IEnumerable<BindableMediaFileItem> Create(IEnumerable<string> fileNames)
        {
            List<BindableMediaFileItem> items = new List<BindableMediaFileItem>();
            foreach(var file in fileNames)
            {
                items.Add(Create(file));
            }
            return items;
        }
        public Task<IEnumerable<BindableMediaFileItem>> CreateAsync(IEnumerable<string> fileNames)
        {
            return Task.Run(() => Create(fileNames));
        }
    }
}
