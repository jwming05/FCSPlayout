using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("SettingScopes")]
    public class SettingScope:IGuidIdentifier
    {
        [Key]
        public Guid Id { get; set; }

        [Index("IDX_SettingScopes", 1, IsUnique = true)]
        [Required, ForeignKey("Application")]
        public string ApplicationName { get; set; }

        [Index("IDX_SettingScopes", 2, IsUnique = true)]
        [Required, ForeignKey("Machine")]
        public string MachineName { get; set; }

        public ApplicationInfo Application { get; set; }

        public MachineInfo Machine { get; set; }
    }
}