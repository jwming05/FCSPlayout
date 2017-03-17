using FCSPlayout.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("PlayRecords")]
    public class PlayRecord:IGuidIdentifier
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PlayItemId { get; set; }
        public Guid PlaybillItemId { get; set; }

        public PlayScheduleMode ScheduleMode { get; set; }

        public Guid SourceId { get; set; }
        public string SourceTitle { get; set; }
        public double? SourceDuration { get; set; }
        public MediaSourceCategory SourceCategory { get; set; }

        public DateTime PlayItemStartTime { get; set; }

        // 对应于PlayItem.PlayRange.StartPosition
        public double PlayItemMarkerIn { get; set; }

        // 对应于PlayItem.PlayDuration
        public double PlayItemDuration { get; set; }

        public DateTime? ActualStartTime { get; set; }
        
        public DateTime? ActualStopTime { get; set; }

        //public DateTime? LoadTime { get; set; }

        public double? LoadMarkerIn { get; set; }
        public double? LoadMarkerDuration { get; set; }
    }
}
