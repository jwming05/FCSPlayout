using System.Windows.Media.Imaging;

namespace FCSPlayout.WPF.Core
{
    public interface IMediaFileImageRequester
    {
        string FilePath { get; }
        BitmapSource Image { get; set; }
        double Position { get; }
        MediaFileImageRequest RequestToken { get; set; }
    }
}