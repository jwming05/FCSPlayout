﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("MediaSource")]
    [Serializable]
    public abstract class MediaSourceEntity : IGuidIdentifier
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}