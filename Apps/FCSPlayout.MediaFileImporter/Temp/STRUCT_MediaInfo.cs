using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.MediaFileImporter
{
    public struct STRUCT_MediaInfo
    {
        /// <summary>
        /// 获取或设置音频流（音轨）数量。
        /// </summary>
        public int AudioStreamsCount
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置第一个音频流的比特率。
        /// </summary>
        public int AudioBitRate
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置第一个音频流的通道数量。
        /// </summary>
        public int AudioChannels
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置第一个音频流的采样率。
        /// </summary>
        public int AudioFrequency
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置第一个音频流的编解码器名称。
        /// </summary>
        public string AudioFormat
        {
            get;
            set;
        }

        public string AudioBitrateMode
        {
            get;
            set;
        }

        public int VideoStreamsCount
        {
            get;
            set;
        }

        public string VideoFormat
        {
            get;
            set;
        }

        public string VideoBitrateMode
        {
            get;
            set;
        }

        public int VideoBitRate
        {
            get;
            set;
        }

        public string VideoStandard
        {
            get;
            set;
        }

        public int VideoBitResolution
        {
            get;
            set;
        }

        public double VideoFramerate
        {
            get;
            set;
        }

        public double VideoAspectRatio
        {
            get;
            set;
        }

        public void Clean()
        {
        }
    }
}
