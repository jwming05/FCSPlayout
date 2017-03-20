using MLCHARGENLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FCSPlayout.CG
{
    [Serializable]
    public abstract class MLCGGenericItem:MLCGItemBase
    {
        protected MLCGGenericItem()
        {
            this.Visible = true;
        }
        public double X { get; set; }
        public double Y { get; set; }

        public bool IsRelative { get; set; }
        public bool Visible { get; set; }
        protected string FileNameOrItemDesc { get; set; }

        protected override void Attach(CoMLCharGen charGen, string id)
        {
            charGen.AddNewItem(this.FileNameOrItemDesc, this.X, this.Y, Convert.ToInt32(this.IsRelative), Convert.ToInt32(this.Visible), ref id);
        }

        public override CGItem Clone()
        {
            MLCGGenericItem item = this.CloneInternal();
            item.X = this.X;
            item.Y = this.Y;
            item.IsRelative = this.IsRelative;
            item.Visible = this.Visible;
            item.FileNameOrItemDesc = this.FileNameOrItemDesc;

            base.CopyCoreProperties(item);
            return item;
        }

        protected abstract MLCGGenericItem CloneInternal();

        public override void Init(XElement element)
        {
            this.X=double.Parse(element.Attribute("X").Value);

            this.Y=double.Parse(element.Attribute("Y").Value);

            this.IsRelative=bool.Parse(element.Attribute("IsRelative").Value);

            this.Visible=bool.Parse(element.Attribute("Visible").Value);

            this.FileNameOrItemDesc=element.Attribute("FileNameOrItemDesc").Value;

            InitInternal(element);
        }

        protected virtual void InitInternal(XElement element)
        {
        }

        public override XElement ToXElement()
        {
            XElement element = new XElement("cgitem");
            element.Add(new XAttribute("X", this.X.ToString()));
            element.Add(new XAttribute("Y", this.Y.ToString()));
            element.Add(new XAttribute("IsRelative", this.IsRelative.ToString()));
            element.Add(new XAttribute("Visible", this.Visible.ToString()));
            element.Add(new XAttribute("FileNameOrItemDesc", this.FileNameOrItemDesc));
            ToXElementInternal(element);
            return element;
        }

        protected virtual void ToXElementInternal(XElement element)
        {
        }
    }
}
