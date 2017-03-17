using System;
using System.Runtime.InteropServices;

namespace MLLicenseLib
{
    [ComVisible(true), Guid("FC774342-CE7B-4726-B661-0FD67E8AF347")]
    public interface IMPlatformLicense
    {
        void Timer();
    }
}
