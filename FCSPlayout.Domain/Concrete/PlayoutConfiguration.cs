using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlayoutConfiguration : IPlayoutConfiguration
    {
        private static IPlayoutConfiguration _current=new PlayoutConfiguration();
        private static TimeSpan _defaultDuration=TimeSpan.FromHours(1);

        public static IPlayoutConfiguration Current
        {
            get
            {
                return _current;
            }

            set
            {
                _current = value;
            }
        }

        public PlayoutConfiguration()
        {
            this.MinPlayDuration = TimeSpan.FromSeconds(4);
            this.AutoPaddingMediaSource = new AutoPaddingMediaSource();

            this.PlayTimeTolerance = TimeSpan.FromMilliseconds(100);

            this.MinLoadDelay = TimeSpan.FromSeconds(1);
            this.MaxLoadDelay = TimeSpan.FromSeconds(2);

            this.MinPreLoadDelay = TimeSpan.FromSeconds(1.5);
            this.MaxPreLoadDelay= TimeSpan.FromSeconds(3);
        }

        public virtual IMediaSource AutoPaddingMediaSource
        {
            get; set;
        }

        public TimeSpan DefaultDuration
        {
            get
            {
                return _defaultDuration;
            }
            set
            {
                _defaultDuration = value;
            }
        }

        public virtual TimeSpan MinPlayDuration
        {
            get; set;
        }

        public TimeSpan PlayTimeTolerance { get; set; }

        /// <summary>
        /// 获取或设置最小加载延迟。
        /// </summary>
        public TimeSpan MinLoadDelay { get; set; }

        /// <summary>
        /// 获取或设置最大加载延迟。
        /// </summary>
        public TimeSpan MaxLoadDelay { get; set; }

        /// <summary>
        /// 获取或设置最小预加载延迟。
        /// </summary>
        public TimeSpan MinPreLoadDelay { get; set; }

        /// <summary>
        /// 获取或设置最大预加载延迟。
        /// </summary>
        public TimeSpan MaxPreLoadDelay { get; set; }
    }
}
