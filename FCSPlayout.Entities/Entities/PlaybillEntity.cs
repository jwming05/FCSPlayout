using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Entities
{
    [Table("Playbills")]
    public class PlaybillEntity : IGuidIdentifier, IModificationTimestamp //IObjectWithState, 
    {
        [Key]
        public Guid Id { get; set; }

        public List<PlaybillItemEntity> PlaybillItems { get; set; }

        public DateTime StartTime { get; set; }

        public double Duration { get; set; }

        public DateTime ModificationTime
        {
            get;set;
        }

        public DateTime CreationTime
        {
            get;set;
        }

        [NotMapped]
        public DateTime StopTime
        {
            get { return this.StartTime.Add(TimeSpan.FromSeconds(this.Duration)); }
        }

        [NotMapped]
        public List<PlayItemEntity> PlayItems { get; internal set; }
    }
}
