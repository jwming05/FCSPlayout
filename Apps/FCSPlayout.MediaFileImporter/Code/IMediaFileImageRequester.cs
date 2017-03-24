using System.Windows.Media.Imaging;
using FCSPlayout.AppInfrastructure;

namespace FCSPlayout.MediaFileImporter
{
    public interface IMediaFileImageRequester
    {
        string FilePath { get; }
        BitmapSource Image { get; set; }
        double Position { get; }
        MediaFileImageRequest RequestToken { get; set; }
    }
}