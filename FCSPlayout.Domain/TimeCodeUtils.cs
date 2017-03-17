using System;

namespace FCSPlayout.Domain
{
    public static class TimeCodeUtils
    {
        public static int MillisecondsPerFrame = 40;
        public static double FramesPerSecond = 1000.0 / MillisecondsPerFrame;

        public static int ToFrames(int milliseconds)
        {
            return (int)Math.Floor(milliseconds * FramesPerSecond / 1000.0);
        }

        public static int ToMilliseconds(int frames)
        {
            return (int)Math.Floor(frames * 1000.0 / FramesPerSecond);
        }
    }
}
