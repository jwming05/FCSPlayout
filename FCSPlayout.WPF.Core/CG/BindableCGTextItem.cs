using FCSPlayout.CG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FCSPlayout.WPF.Core
{
    public class BindableCGTextItem : BindableMLCGItemBase<MLCGTextItem>
    {
        public BindableCGTextItem()
            : base(new MLCGTextItem(), true)
        {
        }

        public BindableCGTextItem(MLCGTextItem item)
            : base(item, false)
        {
        }

        public string Text
        {
            get { return this.Item.Text; }
            set
            {
                this.Item.Text = value;
                RaisePropertyChanged(nameof(this.Text));
            }
        }

        public int FontHeight
        {
            get { return this.Item.FontHeight; }
            set
            {
                this.Item.FontHeight = value;
                RaisePropertyChanged(nameof(this.FontHeight));
            }
        }

        public Color TextColor
        {
            get
            {
                var bytes = BitConverter.GetBytes(this.Item.TextColor);
                var a = bytes[3];
                var b = bytes[2];
                var g = bytes[1];
                var r = bytes[0];

                return Color.FromRgb(r, g, b);
            }

            set
            {
                var bytes = new byte[4];
                bytes[3] = value.A;
                bytes[2] = value.B;
                bytes[1] = value.G;
                bytes[0] = value.R;

                this.Item.TextColor = BitConverter.ToInt32(bytes, 0);
            }
        }
    }
}
