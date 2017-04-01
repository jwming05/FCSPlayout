using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FCSPlayout.WPF.Core
{
    public interface IMediaFileImageResolver
    {
        void ResolveAsync(IMediaFileImageRequester requester);
        BitmapSource Resolve(string filePath, double position);
        BitmapSource Decode(byte[] imageBytes);
    }
}
