using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("Settings")]
    public class SettingInfo
    {
        [Key, Column(Order = 1)]
        [Required, ForeignKey("Scope")]
        public Guid ScopeId { get; set; }

        [Key, Column(Order = 2)]
        [Required, ForeignKey("Group")]
        public string GroupName { get; set; }

        [Key, Column(Order = 3), Required]
        public string Name { get; set; }
        public string Value { get; set; }
        public string Tag { get; set; }

        public SettingScope Scope { get; set; }
        public SettingGroup Group { get; set; }
    }
}