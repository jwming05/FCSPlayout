using System;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public abstract class MediaFileDurationGetter
    {
        private static MediaFileDurationGetter _current;

        public static MediaFileDurationGetter Current
        {
            get
            {
                return _current;
            }

            set
            {
                _current = value;
            }
        }

        public abstract TimeSpan GetDuration(string filePath);

        public async Task<TimeSpan> GetDurationAsync(string filePath)
        {
            return await Task.Run<TimeSpan>(() => GetDuration(filePath));
        }
    }
}
