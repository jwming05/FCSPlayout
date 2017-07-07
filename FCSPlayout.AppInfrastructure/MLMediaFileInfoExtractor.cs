using MPLATFORMLib;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FCSPlayout.AppInfrastructure
{
    public struct MediaFileInfo
    {
        public byte[] ThumbnailBytes { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public interface IMediaFileInfoExtractor
    {
        MediaFileInfo GetMediaFileInfo(string filePath, int thumbnailSize);
    }

    public static class MediaFileInfoExtractorExtensions
    {
        public static Task<MediaFileInfo> GetMediaFileInfoAsync(this IMediaFileInfoExtractor extractor,
            string filePath, int thumbnailSize)
        {
            return Task.Run(() => extractor.GetMediaFileInfo(filePath, thumbnailSize));
        }
    }

    public class MLMediaFileInfoExtractor : IMediaFileInfoExtractor, IDisposable
    {
        public static readonly MLMediaFileInfoExtractor Instance = new MLMediaFileInfoExtractor();

        private MFileClass _mfileObj;
        private object _locker = new object();

        private MLMediaFileInfoExtractor()
        {
            _mfileObj = new MFileClass();
        }

        public void Dispose()
        {
            if (_mfileObj != null)
            {
                Marshal.ReleaseComObject(_mfileObj);
                _mfileObj = null;
            }
        }

        private IntPtr GetMediaFileInfoInternal(string filePath, out double dblDuration)
        {
            IntPtr hBitmap = IntPtr.Zero;
            dblDuration = 0;
            if (_mfileObj != null)
            {
                try
                {
                    _mfileObj.FileNameSet(filePath, string.Empty);
                    _mfileObj.ObjectStart(null);
                    double dblIn, dblOut;

                    try
                    {
                        _mfileObj.FileInOutGet(out dblIn, out dblOut, out dblDuration);

                        if (dblDuration > 0)
                        {
                            MFrame mFrame = null;
                            _mfileObj.FileFrameGet(dblDuration / 2.0, 0.0, out mFrame);
                            long value = 0L;
                            mFrame.FrameVideoGetHbitmap(out value);

                            Marshal.ReleaseComObject(mFrame);
                            mFrame = null;
                            hBitmap = new System.IntPtr(value);
                        }

                    }
                    finally
                    {
                        _mfileObj.ObjectClose();
                    }
                }
                catch
                {
                    hBitmap = IntPtr.Zero;
                }
            }
            return hBitmap;
        }
        private static byte[] GetImageBytes(IntPtr hBitmap, int targetSize)
        {
            byte[] bytes = null;
            try
            {
                using (var bmp = System.Drawing.Image.FromHbitmap(hBitmap))
                {
                    NativeMethods.DeleteObject(hBitmap);
                    bytes = PhotoResizer.GetImageBytes(bmp, targetSize);
                }
            }
            catch
            {
                bytes = null;
            }
            return bytes;
        }

        public MediaFileInfo GetMediaFileInfo(string filePath, int thumbnailSize)
        {
            IntPtr hBitmap = IntPtr.Zero;
            double duration = 0.0;
            lock (_locker)
            {
                hBitmap = GetMediaFileInfoInternal(filePath, out duration);
            }

            return new MediaFileInfo
            {
                Duration = TimeSpan.FromSeconds(duration),
                ThumbnailBytes = hBitmap != IntPtr.Zero ? GetImageBytes(hBitmap, thumbnailSize) : null
            };
        }
    }
}
