using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("MediaSource")]
    public abstract class MediaSourceEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public double Duration { get; set; }
    }
}