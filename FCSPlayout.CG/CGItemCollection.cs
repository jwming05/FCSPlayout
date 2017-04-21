using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FCSPlayout.CG
{
    [Serializable]
    public class CGItemCollection : Collection<CGItem>
    {
        public CGItemCollection()
        {

        }

        public CGItemCollection(IList<CGItem> other)
        {
            this.AddRange(other);
        }

        public void Reset(IEnumerable<CGItem> other)
        {
            this.Clear();
            AddRange(other);
            //if (other != null)
            //{
            //    foreach (var item in other)
            //    {
            //        this.Add(item);
            //    }
            //}
        }

        public void AddRange(IEnumerable<CGItem> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    this.Add(item.Clone());
                }
            } 
        }

        public static CGItemCollection FromXElement(XElement element)
        {
            CGItemCollection items = new CGItemCollection();
            foreach(var child in element.Elements())
            {
                var temp = Type.GetType(child.Attribute("type").Value);
                try
                {
                    CGItem item = (CGItem)Activator.CreateInstance(temp);
                    item.Init(child/*element*/);
                    items.Add(item);
                }
                catch
                {

                }
            }
            return items;
            //throw new NotImplementedException();
        }

        public XElement ToXElement(string elementName)
        {
            XElement element = new XElement(elementName);
            foreach(CGItem item in this)
            {
                var temp = item.ToXElement();
                temp.Add(new XAttribute("type", item.GetType().AssemblyQualifiedName));
                element.Add(temp);
            }
            return element;
        }

        private const string CGItemsTagName = "cg-items";
        public static CGItemCollection FromXml(string cgContents)
        {
            var reader = new StringReader(cgContents);
            return FromXElement(XElement.Load(reader));
        }

        public static string ToXml(CGItemCollection cgItems)
        {
            XElement root = cgItems.ToXElement(CGItemsTagName);
            StringWriter writer = new StringWriter();
            root.Save(writer);
            return writer.ToString();
        }

        public CGItemCollection Clone()
        {
            var result = new CGItemCollection();
            foreach(var item in this)
            {
                result.Add(item.Clone());
            }
            return result;
            //throw new NotImplementedException();
        }
    }
}
