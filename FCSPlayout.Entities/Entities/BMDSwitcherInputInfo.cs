using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    public class BMDSwitcherInputInfo:IGuidIdentifier
    {
        //private int _id = int.MinValue;

        [Key]
        public Guid Id
        {
            get;set;
        }

        public string Name { get; set; }

        public long Value { get; set; }

        [ForeignKey("SwitcherId"), Required]
        public BMDSwitcherInfo Switcher { get; set; }
        public Guid SwitcherId { get; set; }

        public ChannelInfo Channel { get; set; }

        [ForeignKey("Channel")]
        public Guid? ChannelId { get; set; }
    }
}
