using FCSPlayout.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("PlayItems")]
    public class PlayItemEntity : IGuidIdentifier, IModificationTimestamp, IRangeMarkable
    {
        #region Constructors
        public PlayItemEntity()
        {

        }

        public PlayItemEntity(IPlayItem playItem, IMediaSourceConverter mediaSourceConverter)
        {
            this.Id = playItem.Id;

            this.StartTime = playItem.StartTime;
            this.PlayDuration = playItem.CalculatedPlayDuration.TotalSeconds;
            this.MarkerIn = playItem.PlayRange.StartPosition.TotalSeconds;
            this.MarkerDuration = playItem.PlayRange.Duration.TotalSeconds;

            this.PlaybillItemId = playItem.PlaybillItem.Id;
            this.PlaybillItem = new PlaybillItemEntity(playItem.PlaybillItem, mediaSourceConverter);
        }
        #endregion Constructors

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
        [System.Xml.Serialization.XmlIgnore]
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