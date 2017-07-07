using System;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public abstract class MediaFileDurationGetter
    {
        public abstract TimeSpan GetDuration(string filePath);

        public async Task<TimeSpan> GetDurationAsync(string filePath)
        {
            return await Task.Run<TimeSpan>(() => GetDuration(filePath));
        }
    }
}
