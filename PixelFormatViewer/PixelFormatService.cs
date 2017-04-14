using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PixelFormatViewer
{
    class PixelFormatService: IPixelFormatService
    {
        public Task<IEnumerable<PixelFormat>> GetAllPixelFormatsAsync()
        {
            return Task.Run<IEnumerable<PixelFormat>>(() => GetAllPixelFormats());
        }

        private IEnumerable<PixelFormat> GetAllPixelFormats()
        {
            var type = typeof(PixelFormats);
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var pi in propertyInfos)
            {
                if (pi.PropertyType == typeof(PixelFormat) && pi.CanRead)
                {
                    yield return (PixelFormat)pi.GetValue(null);
                }
            }
        }
    }
}
