using FCSPlayout.Domain;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FCSPlayout.WPF.Core
{
    public class TimeCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            if (value.GetType() == typeof(TimeSpan))
            {
                TimeSpan time = (TimeSpan)value;
                return string.Format("{0}:{1:D2}:{2:D2}:{3:D2}",
                    time.Hours, time.Minutes, time.Seconds, TimeCodeUtils.ToFrames(time.Milliseconds));
            }

            if (value.GetType() == typeof(DateTime))
            {
                var dt = (DateTime)value;
                TimeSpan time = dt.TimeOfDay;
                return string.Format("{0:yyyy-MM-dd} {1:D2}:{2:D2}:{3:D2}:{4:D2}",
                    dt.Date, time.Hours, time.Minutes, time.Seconds, TimeCodeUtils.ToFrames(time.Milliseconds));
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
