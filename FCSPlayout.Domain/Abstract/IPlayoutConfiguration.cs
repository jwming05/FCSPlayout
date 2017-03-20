using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public interface IPlayoutConfiguration
    {
        /// <summary>
        /// 获取用于自动垫片的媒体源。
        /// </summary>
        IMediaSource AutoPaddingMediaSource { get; set; }

        /// <summary>
        /// 获取播放项的最小时长。
        /// </summary>
        TimeSpan MinPlayDuration { get; set; }

        /// <summary>
        /// 获取作为没有原生时长的媒体源（例如外部设备）的默认时长。
        /// </summary>
        TimeSpan DefaultDuration { get; set; }

        /// <summary>
        /// 获取或设置开播时间容差。
        /// </summary>
        TimeSpan PlayTimeTolerance { get; set; }

        /// <summary>
        /// 获取或设置最小加载延迟。
        /// </summary>
        TimeSpan MinLoadDelay { get; set; }

        /// <summary>
        /// 获取或设置最大加载延迟。
        /// </summary>
        TimeSpan MaxLoadDelay { get; set; }

        /// <summary>
        /// 获取或设置最小预加载延迟。
        /// </summary>
        TimeSpan MinPreLoadDelay { get; set; }

        /// <summary>
        /// 获取或设置最大预加载延迟。
        /// </summary>
        TimeSpan MaxPreLoadDelay { get; set; }
    }
}
