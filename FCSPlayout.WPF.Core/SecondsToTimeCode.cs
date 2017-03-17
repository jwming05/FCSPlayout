using FCSPlayout.Domain;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FCSPlayout.WPF.Core
{
    public class SecondsToTimeCode : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = TimeSpan.FromSeconds((double)value);
            if (time.Days > 0)
            {
                return string.Format("{4:D2}.{0:D2}:{1:D2}:{2:D2}:{3:D2}",
                    time.Hours, time.Minutes, time.Seconds, TimeCodeUtils.ToFrames(time.Milliseconds), time.Days);
            }
            else
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}",
                    time.Hours, time.Minutes, time.Seconds, TimeCodeUtils.ToFrames(time.Milliseconds));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
