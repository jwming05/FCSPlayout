﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("Users")]
    [Serializable]
    public class UserEntity : IGuidIdentifier
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(50)]
        [Required]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required]
        public string Password { get; set; }

        public bool Locked { get; set; }

        public List<Role> Roles { get; set; }
    }
}
