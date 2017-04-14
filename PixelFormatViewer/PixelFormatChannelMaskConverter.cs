using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PixelFormatViewer
{
    public class PixelFormatChannelMaskConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;

            var channelMask = (PixelFormatChannelMask)value;
            StringBuilder sb = new StringBuilder();
            sb.Append("PixelFormatChannelMask {");
            if (channelMask.Mask != null)
            {
                foreach(byte mask in channelMask.Mask)
                {
                    sb.AppendFormat("{0}, ",mask/*System.Convert.ToString(mask,2)*/);
                }
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append("}");
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
