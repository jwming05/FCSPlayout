using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("Machines")]
    public class MachineInfo
    {
        [Key]
        public string Name { get; set; }
    }
}