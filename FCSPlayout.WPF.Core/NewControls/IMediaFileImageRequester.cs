using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FCSPlayout.WPF.Core
{
    public interface IMediaFileImageRequester
    {
        string FilePath { get; }
        ImageSource Image { get; set; }
        double Position { get; }
        MediaFileImageRequest RequestToken { get; set; }
    }
}