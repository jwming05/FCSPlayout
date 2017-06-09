using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FloatExplorer
{
    class BitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var t = value.GetType();
                var tc = Type.GetTypeCode(t);
                string str = null;
                int length = 0;
                switch (tc)
                {
                    case TypeCode.SByte:
                        length = 8;
                        str = System.Convert.ToString((sbyte)value, 2);
                        break;
                    case TypeCode.Byte:
                        length = 8;
                        str = System.Convert.ToString((byte)value, 2);
                        break;
                    case TypeCode.Int16:
                        length = 16;
                        str = System.Convert.ToString((short)value, 2);
                        break;
                    case TypeCode.UInt16:
                        length = 16;
                        str = System.Convert.ToString((ushort)value, 2);
                        break;
                    case TypeCode.Int32:
                        length = 32;
                        str = System.Convert.ToString((int)value, 2);
                        break;
                    case TypeCode.UInt32:
                        length = 32;
                        str = System.Convert.ToString((uint)value, 2);
                        break;
                    case TypeCode.Int64:
                        length = 64;
                        str = System.Convert.ToString((long)value, 2);
                        break;
                    //case TypeCode.UInt64:
                    //    length = 64;
                    //    str = System.Convert.ToString((ulong)value, 2);
                    //    break;
                }

                if (str != null)
                {
                    var sb = new StringBuilder();
                    sb.Append('0', length - str.Length);
                    sb.Append(str);
                    return sb.ToString();
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FloatSignConverter : IValueConverter
    {
        private static readonly uint _signMask=0x80000000;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var uintValue = (uint)value;
                return (uintValue & _signMask) == 0 ? "0" : "1";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FloatExponentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return ((byte)value) - 127;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
