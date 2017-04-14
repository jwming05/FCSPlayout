using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PixelFormatViewer
{
    public class MaskConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;

            string result= System.Convert.ToString((byte)value, 2);
            if (result.Length < 8)
            {
                var leftPadding = new string('0', 8 - result.Length);
                result = leftPadding + result;
            }
            return result+"  0x"+((byte)value).ToString("X2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
