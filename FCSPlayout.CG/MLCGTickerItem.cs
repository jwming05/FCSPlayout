using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MLCHARGENLib;

namespace FCSPlayout.CG
{
    [Serializable]
    public class MLCGTickerItem:MLCGGenericItem
    {
        protected override void Attach(CoMLCharGen charGen, string id)
        {
            charGen.TickerAddNew("<ticker type='crawl'/>", this.X, this.Y, this.Width, this.Height, 
                Convert.ToInt32(this.IsRelative), Convert.ToInt32(this.Visible), ref id);
            charGen.TickerAddContent(id, this.Text, string.Empty);
        }

        protected override MLCGGenericItem CloneInternal()
        {
            var result = new MLCGTickerItem();
            result.Width = this.Width;
            result.Height = this.Height;
            result.Text = this.Text;
            return result;
        }

        public double Height { get; set; }
        public double Width { get; set; }
        public string Text { get; set; }

        protected override void InitInternal(XElement element)
        {
            base.InitInternal(element);

            this.Width=double.Parse(element.Attribute("Width").Value);
            this.Height=double.Parse(element.Attribute("Height").Value);
            this.Text=element.Attribute("Text").Value;
        }

        protected override void ToXElementInternal(XElement element)
        {
            base.ToXElementInternal(element);

            element.Add(new XAttribute("Width", this.Width.ToString()));
            element.Add(new XAttribute("Height", this.Height.ToString()));
            element.Add(new XAttribute("Text", this.Text));

        }
    }
}
