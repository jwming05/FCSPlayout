using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FCSPlayout.WPF.Core
{
    public interface IImageSourceDecoder
    {
        ImageSource Decode(byte[] imageBytes);
    }

    public class DefaultImageSourceDecoder : IImageSourceDecoder
    {
        public static readonly DefaultImageSourceDecoder Instance = new DefaultImageSourceDecoder();
        private DefaultImageSourceDecoder()
        {

        }
        public ImageSource Decode(byte[] imageBytes)
        {
            if (imageBytes != null && imageBytes.Length>0)
            {
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    var decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    if (decoder.Frames.Count > 0)
                    {
                        return decoder.Frames[0];
                    }
                    
                }
            }
            return null;
        }
    }

    public interface IImagePlaceholderProvider
    {
        ImageSource Placeholder { get; }
    }

    public class DefaultImagePlaceholderProvider : IImagePlaceholderProvider
    {
        public DefaultImagePlaceholderProvider(int width,int height)
        {
            this.Placeholder = new WriteableBitmap(width, height, 96, 96, PixelFormats.Rgb24, null);
        }

        public ImageSource Placeholder
        {
            get;private set;
        }
    }
}
