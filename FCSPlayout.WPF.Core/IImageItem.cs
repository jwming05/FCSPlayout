using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FCSPlayout.WPF.Core
{
    public interface IImageItem
    {
        BitmapSource Image { get; set; }

        byte[] ImageBytes { get; }

        TimeSpan Duration { get; }

        string FilePath { get; }
    }
}
