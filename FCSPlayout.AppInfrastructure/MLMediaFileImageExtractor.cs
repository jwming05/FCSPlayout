using MPLATFORMLib;
using System;
using System.Runtime.InteropServices;

namespace FCSPlayout.AppInfrastructure
{
    public class MLMediaFileImageExtractor : MediaFileImageExtractor,IMediaFileImageExtractor
    {
        private MFileClass _mfileObj;
        private object _locker = new object();

        public MLMediaFileImageExtractor()
        {
            _mfileObj = new MFileClass();
        }

        public override IntPtr GetHBitmap(string filePath, double position)
        {
            IntPtr result = IntPtr.Zero;
            if (_mfileObj != null)
            {
                try
                {
                    _mfileObj.FileNameSet(filePath, string.Empty);
                    _mfileObj.ObjectStart(null);

                    try
                    {
                        result = GetHBitmapCore(position);
                    }
                    finally
                    {
                        _mfileObj.ObjectClose();
                    }
                }
                catch
                {
                    result = IntPtr.Zero;
                }
            }
            return result;
        }

        private IntPtr GetHBitmapCore(double position)
        {
            MFrame mFrame = null;
            _mfileObj.FileFrameGet(position, 0.0, out mFrame);
            M_AV_PROPS m_AV_PROPS = default(M_AV_PROPS);
            mFrame.FrameAVPropsGet(out m_AV_PROPS);

            m_AV_PROPS.vidProps.nWidth = m_AV_PROPS.vidProps.nWidth / 5;
            m_AV_PROPS.vidProps.nHeight = m_AV_PROPS.vidProps.nHeight / 5;
            m_AV_PROPS.vidProps.nRowBytes = m_AV_PROPS.vidProps.nRowBytes / 5;
            MFrame mFrame2 = null;
            mFrame.FrameConvert(ref m_AV_PROPS.vidProps, out mFrame2, string.Empty);
            long value = 0L;
            mFrame2.FrameVideoGetHbitmap(out value);

            Marshal.ReleaseComObject(mFrame);
            Marshal.ReleaseComObject(mFrame2);
            mFrame2 = null;
            mFrame = null;

            return new System.IntPtr(value);
        }

        private IntPtr GetHBitmapCore2(double position)
        {
            MFrame mFrame = null;
            _mfileObj.FileFrameGet(position, 0.0, out mFrame);
            long value = 0L;
            mFrame.FrameVideoGetHbitmap(out value);

            Marshal.ReleaseComObject(mFrame);
            mFrame = null;
            return new System.IntPtr(value);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_mfileObj != null)
            {
                Marshal.ReleaseComObject(_mfileObj);
                _mfileObj = null;
            }
        }

        public byte[] GetImageBytes(string filePath, double position, int targetSize)
        {
            IntPtr hBitmap = IntPtr.Zero;
            lock (_locker)
            {
                hBitmap = GetHBitmapInternal(filePath, position);
            }

            return hBitmap != IntPtr.Zero ? GetImageBytes(hBitmap, targetSize) : null;
        }

        private IntPtr GetHBitmapInternal(string filePath, double position)
        {
            IntPtr hBitmap = IntPtr.Zero;
            if (_mfileObj != null)
            {
                try
                {
                    _mfileObj.FileNameSet(filePath, string.Empty);
                    _mfileObj.ObjectStart(null);

                    try
                    {
                        MFrame mFrame = null;
                        _mfileObj.FileFrameGet(position, 0.0, out mFrame);
                        long value = 0L;
                        mFrame.FrameVideoGetHbitmap(out value);

                        Marshal.ReleaseComObject(mFrame);
                        mFrame = null;
                        hBitmap = new System.IntPtr(value);
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
                    bytes= PhotoResizer.GetImageBytes(bmp, targetSize);
                }
            }
            catch
            {
                bytes = null;
            }
            return bytes;
        }
    }
}