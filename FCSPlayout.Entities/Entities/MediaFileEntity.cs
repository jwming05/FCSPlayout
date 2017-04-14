using FCSPlayout.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace FCSPlayout.Entities
{
    [Table("MediaFiles")]
    [Serializable]
    public class MediaFileEntity : MediaSourceEntity, IModificationTimestamp, IRangeMarkable
    {
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public double MarkerIn { get; set; }
        public double MarkerDuration { get; set; }
        
        public double Duration { get; set; }

        public int AudioGain
        {
            get;set;
        }

        public DateTime CreationTime { get; set; }
        public DateTime ModificationTime { get; set; }

        public Guid? MediaFileCategoryId { get; set; }

        [ForeignKey("MediaFileCategoryId")]
        [XmlIgnore]
        public MediaFileCategory MediaFileCategory { get; set; }

        public Guid? MediaFileChannelId { get; set; }

        [ForeignKey("MediaFileChannelId")]
        [XmlIgnore]
        public MediaFileChannel MediaFileChannel { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeleteTime { get; set; }

        [XmlIgnore]
        public UserEntity Creator
        {
            get;set;
        }

        [ForeignKey("Creator")]
        public Guid CreatorId { get; set; }

        public MediaFileMetadata Metadata { get; set; }
    }

    
    [Serializable]
    public class MediaFileMetadata
    {
        [Key]
        [ForeignKey("MediaFile")]
        public Guid Id { get; set; }

        public byte[] Icon { get; set; }

        public string MediaInformation { get; set; }

        [XmlIgnore]
        public MediaFileEntity MediaFile { get; set; }
    }
}