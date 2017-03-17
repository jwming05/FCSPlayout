using System;
using System.Runtime.InteropServices;

namespace FCSPlayout.AppInfrastructure
{
    public static class NativeMethods
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
