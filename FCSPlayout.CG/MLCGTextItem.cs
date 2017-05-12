using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MLCHARGENLib;
using System.Windows.Media;

namespace FCSPlayout.CG
{
    [Serializable]
    public class MLCGTextItem : MLCGGenericItem
    {
        //#AABBGGRR
        private CG_TEXT_PROPS _textProps = new CG_TEXT_PROPS()
        {
            nTextColor = Convert.ToInt32(0x7F00FF00),
            nFontHeight = 24 /*int.MaxValue*/
        };

        protected override void Attach(CoMLCharGen charGen, string id)
        {
            charGen.AddNewTextItem(ref _textProps, this.X, this.Y, 
                Convert.ToInt32(this.IsRelative), Convert.ToInt32(this.Visible), ref id);
        }

        protected override MLCGGenericItem CloneInternal()
        {
            var result = new MLCGTextItem();
            result._textProps = this._textProps;
            return result;
        }

        #region CG_TEXT_PROPS

        /// <summary>
        /// font face of the item
        /// </summary>
        public string FontFace
        {
            get { return _textProps.bsFontFace; }
            set { _textProps.bsFontFace = value; }
        }

        /// <summary>
        /// item text
        /// </summary>
        public string Text
        {
            get { return _textProps.bsTextString; }
            set { _textProps.bsTextString = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double OutlineWidth
        {
            get { return _textProps.dblOutlineWidth; }
            set { _textProps.dblOutlineWidth = value; }
        }

        public eCG_TextType TextType
        {
            get { return _textProps.eTextType; }
            set { _textProps.eTextType = value; }
        }

        public eCG_DTFlags DTFlags
        {
            get { return _textProps.nDTFlags; }
            set { _textProps.nDTFlags = value; }
        }

        public int FontHeight
        {
            get { return _textProps.nFontHeight; }
            set { _textProps.nFontHeight = value; }
        }

        public int OutlineColor
        {
            get { return _textProps.nOutlineColor; }
            set { _textProps.nOutlineColor = value; }
        }

        public int TextColor
        {
            get { return _textProps.nTextColor; }
            set { _textProps.nTextColor = value; }
        }

        public int TimeOffset
        {
            get { return _textProps.nTimeOffset; }
            set { _textProps.nTimeOffset = value; }
        }
        #endregion

        protected override void InitInternal(XElement element)
        {
            base.InitInternal(element);

            // TODO: 
            this.FontHeight = int.Parse(element.Attribute("FontHeight").Value);
            this.TextColor =int.Parse(element.Attribute("TextColor").Value);
            this.Text = element.Attribute("Text").Value;
        }

        protected override void ToXElementInternal(XElement element)
        {
            base.ToXElementInternal(element);

            // TODO: 
            element.Add(new XAttribute("FontHeight", this.FontHeight.ToString()));
            element.Add(new XAttribute("TextColor", this.TextColor.ToString()));
            element.Add(new XAttribute("Text", this.Text));
        }
    }
}