using MLCHARGENLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FCSPlayout.CG
{
    public abstract class CGItem
    {
        //protected internal String ItemId { get; set; }
        public String ItemName { get; set; }

        public abstract string Attach(object obj);

        public abstract void Detach(object obj, string id);

        //public String Tag { get; set; }

        public abstract CGItem Clone();

        protected virtual void CopyCoreProperties(CGItem item)
        {
            item.ItemName = this.ItemName;
            //item.Tag = this.Tag;
        }

        public abstract XElement ToXElement();

        public abstract void Init(XElement element);
    }
}