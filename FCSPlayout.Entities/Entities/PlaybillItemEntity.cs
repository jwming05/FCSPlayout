using FCSPlayout.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Entities
{
    [Table("PlaybillItems")]
    public class PlaybillItemEntity : IGuidIdentifier, IModificationTimestamp, IRangeMarkable
    {
        [Key]
        public Guid Id { get; set; }
        // SourceId
        public DateTime? StartTime { get; set; }

        public double MarkerIn { get; set; }
        public double MarkerDuration { get; set; }

        public double GetStopPosition()
        {
            return this.MarkerIn + this.MarkerDuration;
        }

        public PlayScheduleMode ScheduleMode { get; set; }

        //public Guid SegmentId { get; set; }

        [ForeignKey("Playbill")]
        public Guid? PlaybillId { get; set; }

        
        public PlaybillEntity Playbill { get; set; }

        public List<PlayItemEntity> PlayItems { get; set; }

        
        public Entities.MediaSourceEntity MediaSource { get; set; }

        // 为空是为了容许空素材。
        [ForeignKey("MediaSource")]
        public Guid? MediaSourceId { get; set; }

        //[NotMapped]
        //public ObjectState State
        //{
        //    get; set;
        //}

        public DateTime ModificationTime
        {
            get;set;
        }

        public DateTime CreationTime
        {
            get;set;
        }

        public bool IsAutoPadding { get; set; }

        // 下面两个属性用于空素材的情况下。
        public string MediaSourceTitle { get; set; }
        public double? MediaSourceDuration { get; set; }

        public int AudioGain
        {
            get; set;
        }

        public string CGContents { get; set; }
    }
}
