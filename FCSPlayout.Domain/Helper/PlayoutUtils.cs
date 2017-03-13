using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    internal static class PlayoutUtils
    {
        public static void ValidatePlayDuration(TimeSpan duration)
        {
            if (duration < PlayoutConfiguration.Current.MinPlayDuration)
            {
                throw new ArgumentException(
                    string.Format("时长{0}无效，播放项的时长不能小于{1}。", 
                    duration, PlayoutConfiguration.Current.MinPlayDuration));
            }
        }
    }
}
