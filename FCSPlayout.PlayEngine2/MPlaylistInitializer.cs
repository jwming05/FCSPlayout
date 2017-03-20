using FCSPlayout.Domain;
using MPLATFORMLib;
using System;
using System.Runtime.InteropServices;

namespace FCSPlayout.PlayEngine
{
    class MPlaylistInitializer
    {
        public static void Initialize(MPlaylistClass mplaylist, MPlaylistSettings settings)
        {
            if (!string.IsNullOrEmpty(settings.VideoFormat))
            {
                //设置视频属性。
                SetVideoFormat(mplaylist, settings.VideoFormat);
            }


            if (!string.IsNullOrEmpty(settings.AudioFormat))
            {
                //设置音频属性。
                SetAudioFormat(mplaylist, settings.AudioFormat);
            }


            SetProperties(mplaylist);

            //设置背景
            SetBackground(mplaylist);

            //设置预览窗口。
            //SetPreviewWindow(mplaylist, info);
        }

        private static void SetProperties(MPlaylistClass mplaylist)
        {
            mplaylist.PropsSet("loop", "false");
            //mplaylist.PropsSet("playlist.on_next", "pause_out"/*"stop"*/);
            mplaylist.PropsSet("playlist.on_next", "stop");
            mplaylist.PropsSet("background_rewind ", "true");
        }

        private static void SetBackground(MPlaylistClass mplaylist)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo("PalBars.mpg");
            if (fi.Exists)
            {
                MItem mItem = null;
                mplaylist.PlaylistBackgroundSet(null, fi.FullName, "", out mItem);

                //mItem.FilePlayStart();
                Marshal.ReleaseComObject(mItem);
            }
        }

        private static void SetAudioFormat(MPlaylistClass mplaylist, string formatName)
        {
            int count = 0;
            mplaylist.FormatAudioGetCount(eMFormatType.eMFT_Convert, out count);
            int index = -1;
            MPLATFORMLib.M_AUD_PROPS audProps = new MPLATFORMLib.M_AUD_PROPS();
            string name;
            for (int i = 0; i < count; i++)
            {
                mplaylist.FormatAudioGetByIndex(eMFormatType.eMFT_Convert, i, out audProps, out name);
                if (string.Equals(name, formatName, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                mplaylist.FormatAudioSet(eMFormatType.eMFT_Convert, ref audProps);
            }

            //MPLATFORMLib.M_AUD_PROPS m_AUD_PROPS = default(MPLATFORMLib.M_AUD_PROPS);
            //if (info.AudioFormat > 0)
            //{
            //    string text;
            //    mplaylist.FormatAudioGetByIndex(eMFormatType.eMFT_Convert, info.AudioFormat, out m_AUD_PROPS, out text);
            //}
            //else
            //{
            //    m_AUD_PROPS.nSamplesPerSec = 48000;
            //    m_AUD_PROPS.nChannels = 2;
            //    m_AUD_PROPS.nBitsPerSample = 16;
            //}
            //mplaylist.FormatAudioSet(eMFormatType.eMFT_Convert, ref m_AUD_PROPS);
        }

        private static void SetVideoFormat(MPlaylistClass mplaylist, string formatName)
        {
            int count = 0;
            mplaylist.FormatVideoGetCount(eMFormatType.eMFT_Convert, out count);
            int index = -1;
            MPLATFORMLib.M_VID_PROPS vidProps = new MPLATFORMLib.M_VID_PROPS();
            string name;
            for (int i = 0; i < count; i++)
            {
                mplaylist.FormatVideoGetByIndex(eMFormatType.eMFT_Convert, i, out vidProps, out name);
                if (string.Equals(name, formatName, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                mplaylist.FormatVideoSet(eMFormatType.eMFT_Convert, ref vidProps);
            }

            //MPLATFORMLib.M_VID_PROPS m_VID_PROPS = default(MPLATFORMLib.M_VID_PROPS);

            //m_VID_PROPS.dblRate = info.FPS;
            //m_VID_PROPS.e3DFormat = MPLATFORMLib.eM3DFormat.eM3D_None;
            //m_VID_PROPS.eVideoFormat = info.VideoFormat; // info.VideoFormat.GetMPVideoFormat();
            //m_VID_PROPS.eScaleType = EnumUtils.ToMPScaleType(info.StretchMode);

            ////this._logger.AddLog(ENUM_LogType.INFO, "Video format: " + info.VideoFormat.ToString());
            ////this._logger.AddLog(ENUM_LogType.INFO, "ML Video format: " + m_VID_PROPS.eVideoFormat.ToString());

            //if (info.ExternalKeyMode)
            //{
            //    m_VID_PROPS.fccType = MPLATFORMLib.eMFCC.eMFCC_ARGB32;
            //}

            //if (/*info.VideoFormat.DisplayMode == ENUM_VIDEO_DISPLAY_MODE.PROGRESSIVE*/info.Progressive)
            //{
            //    m_VID_PROPS.eInterlace = MPLATFORMLib.eMInterlace.eMI_Progressive;
            //}
            //else
            //{
            //    m_VID_PROPS.eInterlace = MPLATFORMLib.eMInterlace.eMI_Default;
            //}
            //mplaylist.FormatVideoSet(eMFormatType.eMFT_Convert, ref m_VID_PROPS);
        }
    }
}
