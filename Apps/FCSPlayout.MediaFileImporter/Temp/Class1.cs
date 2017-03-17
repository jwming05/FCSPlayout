using MPLATFORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.MediaFileImporter
{
    class Class1
    {
        public double Duration { get; private set; }
        public double FileDuration { get; private set; }
        public double Markin { get; private set; }
        public double Markout { get; private set; }
        public STRUCT_MediaInfo MediaInfo { get; private set; }
        public string MediaInfoString { get; private set; }
        public string MediaInfoStringComplete { get; private set; }

        protected void RetrieveExtraInfo(MFileClass mFile)
        {
            if (string.IsNullOrEmpty(this.MediaInfoString) && mFile != null)
            {
                STRUCT_MediaInfo mediaInfo = default(STRUCT_MediaInfo);
                string formatName = string.Empty;
                string resolution = string.Empty;
                int audioTracks = 0;

                try
                {
                    System.Collections.Generic.Dictionary<string, string> info = null; // new System.Collections.Generic.Dictionary<string, string>();
                    string inform = string.Empty;

                    string filePath = string.Empty;
                    mFile.FileNameGet(out filePath);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        if (FileInfoSingleton.Instance.GetMediaInfo(filePath, out info, out inform))
                        {
                            formatName = FileInfoSingleton.Instance.GetPropertyValue(info, "file::info::format_name");
                            audioTracks = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::audio_tracks");
                            mediaInfo.AudioStreamsCount = audioTracks;
                            if (audioTracks > 0)
                            {
                                mediaInfo.AudioBitRate = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::audio.0::bit_rate");
                                mediaInfo.AudioChannels = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::audio.0::channels");
                                mediaInfo.AudioFrequency = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::audio.0::sample_rate");
                                mediaInfo.AudioBitrateMode = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::audio.0::bits").ToString();
                                mediaInfo.AudioFormat = FileInfoSingleton.Instance.GetPropertyValue(info, "file::info::audio.0::codec_name");
                            }

                            int propertyIntValue = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::video_tracks");
                            mediaInfo.VideoStreamsCount = propertyIntValue;
                            if (propertyIntValue > 0)
                            {
                                int propertyIntValue2 = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::video.0::width");
                                int propertyIntValue3 = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::video.0::height");
                                resolution = propertyIntValue2.ToString() + "x" + propertyIntValue3.ToString();
                                if (string.Compare(resolution, "352x288") == 0 ||
                                    string.Compare(resolution, "360x288") == 0 ||
                                    string.Compare(resolution, "352x240") == 0 ||
                                    string.Compare(resolution, "360x240") == 0)
                                {
                                    resolution = "CIF";
                                }
                                else if (string.Compare(resolution, "720x576") == 0 || string.Compare(resolution, "720x480") == 0 || string.Compare(resolution, "720x486") == 0)
                                {
                                    resolution = "SD";
                                }
                                else if (string.Compare(resolution, "1280x720") == 0)
                                {
                                    resolution = "HDr";
                                }
                                else if (string.Compare(resolution, "1920x1080") == 0)
                                {
                                    resolution = "HD";
                                }
                                else if (string.Compare(resolution, "2048x1080") == 0)
                                {
                                    resolution = "2K";
                                }
                                else if (string.Compare(resolution, "3840x2160") == 0 || string.Compare(resolution, "4096x2160") == 0)
                                {
                                    resolution = "4K";
                                }

                                mediaInfo.VideoFormat = FileInfoSingleton.Instance.GetPropertyValue(info, "file::info::video.0::codec_name");
                                if (string.Compare(mediaInfo.VideoFormat, "MPEG Video Version 2", true) == 0)
                                {
                                    mediaInfo.VideoFormat = "MPEG-2";
                                }
                                mediaInfo.VideoBitrateMode = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::video.0::has_b_frames").ToString();
                                mediaInfo.VideoBitRate = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::audio.0::kbps_avg_video");
                                if (mediaInfo.VideoBitRate <= 0)
                                {
                                    mediaInfo.VideoBitRate = FileInfoSingleton.Instance.GetPropertyIntValue(info, "file::info::bitrate");
                                }
                                string propertyValue = FileInfoSingleton.Instance.GetPropertyValue(info, "file::info::video.0::r_frame_rate");
                                double framerate = 0.0;
                                mediaInfo.VideoFramerate = framerate;
                                mediaInfo.VideoStandard = VideoStandard.Other.ToString();
                                if (!string.IsNullOrEmpty(propertyValue))
                                {
                                    try
                                    {
                                        string[] array = propertyValue.Split(new char[]
                                        {
                                                '/'
                                        });
                                        if (array != null && array.Length == 2)
                                        {
                                            framerate = (double)(System.Convert.ToInt32(array[0]) * 100 / System.Convert.ToInt32(array[1])) / 100.0;
                                        }
                                        if (framerate == 25.0 || framerate == 50.0)
                                        {
                                            mediaInfo.VideoStandard = VideoStandard.PAL.ToString();
                                        }
                                        else if (framerate == 29.97 || framerate == 30.0 || framerate == 59.94 || framerate == 60.0)
                                        {
                                            mediaInfo.VideoStandard = VideoStandard.NTSC.ToString();
                                        }
                                        else
                                        {
                                            mediaInfo.VideoStandard = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:00.0#}", new object[]
                                            {
                                                    framerate
                                            }) + "fps";
                                        }
                                        mediaInfo.VideoFramerate = framerate;
                                    }
                                    catch
                                    {
                                    }
                                }

                                mediaInfo.VideoBitResolution = 0;
                                string propertyValue2 = FileInfoSingleton.Instance.GetPropertyValue(info, "file::info::video.0::display_ar");
                                mediaInfo.VideoAspectRatio = 1.0;
                                try
                                {
                                    string[] array2 = propertyValue2.Split(new char[]
                                    {
                                            ':'
                                    });
                                    if (array2 != null && array2.Length == 2)
                                    {
                                        mediaInfo.VideoAspectRatio = (double)System.Convert.ToInt32(array2[0]) / (1.0 * (double)System.Convert.ToInt32(array2[1]));
                                    }
                                }
                                catch
                                {
                                }
                                if (this.FileDuration == 0.0)
                                {
                                    this.FileDuration = FileInfoSingleton.Instance.GetPropertyDoubleValue(info, "file::info::duration");
                                }

                                if (this.Duration == 0.0)
                                {
                                    this.Markin = 0.0;
                                    this.Markout = this.FileDuration;
                                    //base.NotifyPropertyChanged("Duration");
                                }
                            }
                            string propertyValue3 = FileInfoSingleton.Instance.GetPropertyValue(info, "file::info::start_timecode");
                            //if (!string.IsNullOrEmpty(propertyValue3))
                            //{
                            //    TIMECODE tIMECODE = new TIMECODE();
                            //    tIMECODE.FillFromString(propertyValue3);
                            //    this.TimeCodeIn = tIMECODE.AddSeconds(base.Markin, this.FPS);
                            //    this.TimeCodeOut = this.TimeCodeIn.AddSeconds(this.Duration, this.FPS);
                            //}

                            //if (this.TimeCodeOut.IsEmpty && this.TimeCodeIn.IsEmpty)
                            //{
                            //    this.TimeCodeIn.Clear();
                            //    this.TimeCodeOut = this.TimeCodeIn.AddSeconds(this.Duration, this.FPS);
                            //}
                            this.MediaInfoStringComplete = inform;
                            //base.NotifyPropertyChanged("MediaInfoStringComplete");
                        }
                    }
                }
                catch
                {
                    mediaInfo.Clean();
                }

                this.MediaInfo = mediaInfo;
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                //if (!this.IsBitmapFile())
                {
                    int spaceIndex;
                    if (!string.IsNullOrEmpty(formatName))
                    {
                        //string text3 = formatName;
                        spaceIndex = formatName.IndexOf(" ");
                        if (spaceIndex > 0)
                        {
                            formatName = formatName.Substring(0, spaceIndex).Trim();
                        }
                        stringBuilder.Append(formatName).Append(" ");
                    }

                    if (!string.IsNullOrEmpty(resolution))
                    {
                        stringBuilder.Append(resolution).Append(" ");
                    }

                    if (string.IsNullOrEmpty(mediaInfo.VideoFormat))
                    {
                        stringBuilder.Append("No video info ");
                    }
                    else
                    {
                        stringBuilder.Append(mediaInfo.VideoStandard).Append("  ");
                        string videoFormat = mediaInfo.VideoFormat;
                        spaceIndex = videoFormat.IndexOf(" ");
                        if (spaceIndex > 0)
                        {
                            videoFormat = videoFormat.Substring(0, spaceIndex).Trim();
                        }
                        stringBuilder.Append(videoFormat).Append(" ");
                        if (mediaInfo.VideoBitRate > 0)
                        {
                            stringBuilder.Append(string.Format("{0:0}", (double)mediaInfo.VideoBitRate / 1024.0 / 1024.0)).Append("Mbs ");
                        }
                    }

                    if (string.IsNullOrEmpty(mediaInfo.AudioFormat))
                    {
                        stringBuilder.Append("No audio info");
                    }
                    else
                    {
                        string audioFormat = mediaInfo.AudioFormat;
                        spaceIndex = audioFormat.IndexOf(" ");
                        if (spaceIndex > 0)
                        {
                            audioFormat = audioFormat.Substring(0, spaceIndex).Trim();
                        }
                        stringBuilder.Append(audioFormat).Append(" ");
                        if (mediaInfo.AudioBitRate > 0)
                        {
                            stringBuilder.Append(string.Format("{0:0}", (double)mediaInfo.AudioBitRate / 1000.0)).Append("Kbs ");
                        }
                        stringBuilder.Append(string.Format("{0:0}", (double)mediaInfo.AudioFrequency / 1000.0)).Append("KHz ");
                        stringBuilder.Append(string.Format("{0:0}", mediaInfo.AudioChannels * audioTracks)).Append("Ch");
                    }
                }
                this.MediaInfoString = stringBuilder.ToString();
                //base.NotifyPropertyChanged("MediaInfoString");
            }
        }
    }
}
