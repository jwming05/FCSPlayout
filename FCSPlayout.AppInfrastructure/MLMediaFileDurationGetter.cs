using FCSPlayout.Domain;
using MPLATFORMLib;
using System;
using System.Runtime.InteropServices;

namespace FCSPlayout.AppInfrastructure
{
    public class MLMediaFileDurationGetter : MediaFileDurationGetter, IDisposable
    {
        private MFileClass _mfileObj;

        public MLMediaFileDurationGetter()
        {
            _mfileObj = new MFileClass();
        }

        ~MLMediaFileDurationGetter()
        {
            Dispose(false);
        }

        public override TimeSpan GetDuration(string filePath)
        {
            double dblDuration = 0;
            if (_mfileObj != null)
            {
                _mfileObj.FileNameSet(filePath, string.Empty);
                _mfileObj.ObjectStart(null);

                double dblIn, dblOut;
                try
                {
                    _mfileObj.FileInOutGet(out dblIn, out dblOut, out dblDuration);
                }
                finally
                {
                    _mfileObj.ObjectClose();
                }
            }
            
            return TimeSpan.FromSeconds(dblDuration);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            if (_mfileObj != null)
            {
                Marshal.ReleaseComObject(_mfileObj);
                _mfileObj = null;
            }
        }
    }
}
