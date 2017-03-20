using FCSPlayout.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

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

        public DateTime CreationTime { get; set; }

        public DateTime ModificationTime { get; set; }

        public double Duration { get; set; }

        public int AudioGain
        {
            get;set;
        }

        public Guid? MediaFileCategoryId { get; set; }

        [ForeignKey("MediaFileCategoryId")]
        public MediaFileCategory MediaFileCategory { get; set; }

        public Guid? MediaFileChannelId { get; set; }

        [ForeignKey("MediaFileChannelId")]
        public MediaFileChannel MediaFileChannel { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeleteTime { get; set; }

        public UserEntity Creator
        {
            get;set;
        }

        [ForeignKey("Creator")]
        public Guid CreatorId { get; set; }
    }
}
