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
        IMediaSource AutoPaddingMediaSource { get; }

        /// <summary>
        /// 获取播放项的最小时长。
        /// </summary>
        TimeSpan MinPlayDuration { get; }
    }
}
