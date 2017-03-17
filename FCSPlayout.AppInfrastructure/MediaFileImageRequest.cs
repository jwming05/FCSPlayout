using System;

namespace FCSPlayout.AppInfrastructure
{
    public class MediaFileImageRequest
    {
        public string Path { get; set; }

        public double Position { get; set; }

        public Action<IntPtr> Complete { get; set; }

        public bool Cancel { get; set; }
    }
}
