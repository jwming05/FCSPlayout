using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("Applications")]
    public class ApplicationInfo
    {
        [Key]
        public string Name { get; set; }
    }
}