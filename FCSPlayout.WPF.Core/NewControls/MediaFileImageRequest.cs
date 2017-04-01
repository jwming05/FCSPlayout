using System;

namespace FCSPlayout.WPF.Core
{
    public class MediaFileImageRequest
    {
        public string Path { get; set; }

        public double Position { get; set; }

        public Action<IntPtr> Complete { get; set; }
        public bool Cancel { get; set; }
    }
}
