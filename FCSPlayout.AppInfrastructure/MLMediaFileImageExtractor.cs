using MPLATFORMLib;
using System;
using System.Runtime.InteropServices;

namespace FCSPlayout.AppInfrastructure
{
    public class MLMediaFileImageExtractor : MediaFileImageExtractor
    {
        private MFileClass _mfileObj;

        public MLMediaFileImageExtractor()
        {
            _mfileObj = new MFileClass();
        }

        protected override IntPtr GetHBitmap(string filePath, double position)
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

        public override void Dispose()
        {
            base.Dispose();
            if (_mfileObj != null)
            {
                Marshal.ReleaseComObject(_mfileObj);
                _mfileObj = null;
            }
        }
    }
}