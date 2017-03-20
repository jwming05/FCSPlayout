using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("MediaFileCategory")]
    [Serializable]
    public class MediaFileCategory : IGuidIdentifier
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}