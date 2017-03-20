using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("MediaFileChannel")]
    [Serializable]
    public class MediaFileChannel : IGuidIdentifier
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}