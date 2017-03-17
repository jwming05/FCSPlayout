using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("SettingGroups")]
    public class SettingGroup
    {
        [Key]
        public string Name { get; set; }
    }
}