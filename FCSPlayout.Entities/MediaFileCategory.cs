using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("MediaFileCategory")]
    public class MediaFileCategory 
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}