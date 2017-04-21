using FCSPlayout.Domain;
using System;

namespace FCSPlayout.AppInfrastructure
{
    public struct AddPlayItemPayload
    {
        public MediaItem MediaItem { get; set; }
        public PlayScheduleMode ScheduleMode { get; set; }
        public DateTime? StartTime { get; set; }
    }
}
