using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FCSPlayout.Entities
{
    public class BMDSwitcherInfo:IGuidIdentifier
    {
        //private int _id = int.MinValue;

        public BMDSwitcherInfo()
        {
            this.InputInfos = new List<BMDSwitcherInputInfo>();
        }

        [Key]
        public Guid Id
        {
            get;set;
        }

        [Required, MaxLength(256)]
        public string Address { get; set; }

        [Required, MaxLength(256)]
        public string Name { get; set; }

        public List<BMDSwitcherInputInfo> InputInfos { get; set; }
        public bool IsNew()
        {
            return this.Id == Guid.Empty; // int.MinValue;
        }

        public override string ToString()
        {
            return this.Name + (string.IsNullOrEmpty(this.Address) ? string.Empty : " (" + this.Address + ")");
        }
    }
}
