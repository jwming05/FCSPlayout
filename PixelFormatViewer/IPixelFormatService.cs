using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PixelFormatViewer
{
    public interface IPixelFormatService
    {
        Task<IEnumerable<PixelFormat>> GetAllPixelFormatsAsync();
    }
}
