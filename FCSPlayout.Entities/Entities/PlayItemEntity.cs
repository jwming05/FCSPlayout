using FCSPlayout.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("PlayItems")]
    public class PlayItemEntity : IGuidIdentifier, IModificationTimestamp, IRangeMarkable
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime StartTime { get; set; }
        public double PlayDuration { get; set; }

        //public DateTime? StopTime { get; set; }
        public double MarkerIn { get; set; }
        public double MarkerDuration { get; set; }

        public PlaybillItemEntity PlaybillItem { get; set; }

        [ForeignKey("PlaybillItem")]
        public Guid PlaybillItemId { get; set; }


        public DateTime ModificationTime
        {
            get;set;
        }

        public DateTime CreationTime
        {
            get;set;
        }

        //public DateTime GetStartTime()
        //{
        //    return this.OverrideStartTime ?? this.PlaybillItem.StartTime.Value;
        //}

        [NotMapped]
        public DateTime StopTime
        {
            get
            {
                return this.StartTime.Add(TimeSpan.FromSeconds(this.PlayDuration));
            }
        }

        //public TimeSpan GetDuration()
        //{
        //    return TimeSpan.FromSeconds(this.OverrideDuration ?? this.PlaybillItem.Duration);
        //}

        //public TimeSpan GetPlayDuration()
        //{
        //    return this.GetStopTime().Subtract(this.GetStartTime());
        //}
    }
}