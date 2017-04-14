using FCSPlayout.CG;
using FCSPlayout.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FCSPlayout.Entities
{
    [Table("PlaybillItems")]
    [XmlInclude(typeof(MediaFileEntity))]
    [XmlInclude(typeof(ChannelInfo))]
    public class PlaybillItemEntity : IGuidIdentifier, IModificationTimestamp, IRangeMarkable
    {
        #region Constructors
        public PlaybillItemEntity()
        {

        }

        public PlaybillItemEntity(IPlaybillItem billItem,IMediaSourceConverter mediaSourceConverter)
        {
            this.Id = billItem.Id;

            //this.Playbill = billEntity;

            this.StartTime = billItem.StartTime;
            this.MarkerIn = billItem.PlayRange.StartPosition.TotalSeconds;
            this.ScheduleMode = billItem.ScheduleMode;
            this.MarkerDuration = billItem.PlayRange.Duration.TotalSeconds;

            if (billItem.MediaSource.Category != MediaSourceCategory.Null)
            {
                this.MediaSourceId = billItem.MediaSource.Id;
                if (mediaSourceConverter != null)
                {
                    this.MediaSource = mediaSourceConverter.ToEntity<MediaSourceEntity>(billItem.MediaSource);
                }
            }
            else
            {
                this.MediaSourceTitle = billItem.MediaSource.Title;
                this.MediaSourceDuration = billItem.MediaSource.Duration.Value.TotalSeconds;
            }

            var autoBillItem = billItem as AutoPlaybillItem;
            if (autoBillItem != null)
            {
                this.IsAutoPadding = autoBillItem.IsAutoPadding;
            }


            var fileMediaSource = billItem.MediaSource as IFileMediaSource;
            if (fileMediaSource != null)
            {
                this.AudioGain = fileMediaSource.AudioGain;
            }

            if (billItem.CGItems != null)
            {
                this.CGContents = CGItemCollection.ToXml(billItem.CGItems);
            }

            
            
        }
        #endregion Constructors

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
        [System.Xml.Serialization.XmlIgnore]
        public Guid? PlaybillId { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public PlaybillEntity Playbill { get; set; }

        [System.Xml.Serialization.XmlIgnore]
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
