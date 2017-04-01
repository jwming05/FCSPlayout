using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("Roles")]
    [Serializable]
    public class Role : IGuidIdentifier
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(50)]
        [Required]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        public List<UserEntity> Users { get; set; }
    }
}