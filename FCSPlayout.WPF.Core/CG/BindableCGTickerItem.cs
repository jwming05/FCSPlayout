using FCSPlayout.CG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPF.Core
{
    public class BindableCGTickerItem : BindableMLCGItemBase<MLCGTickerItem>
    {
        public BindableCGTickerItem()
            : base(new MLCGTickerItem(), true)
        {
        }

        public BindableCGTickerItem(MLCGTickerItem item)
            : base(item, false)
        {
        }

        public string Text
        {
            get { return this.Item.Text; }
            set
            {
                this.Item.Text = value;
                OnPropertyChanged(() => this.Text);
            }
        }

        public double Height
        {
            get { return this.Item.Height; }
            set
            {
                this.Item.Height = value;
                OnPropertyChanged(() => this.Height);
            }
        }
        public double Width
        {
            get { return this.Item.Width; }
            set
            {
                this.Item.Width = value;
                OnPropertyChanged(() => this.Width);
            }
        }

        //public int FontHeight
        //{
        //    get { return this.Item.FontHeight; }
        //    set
        //    {
        //        this.Item.FontHeight = value;
        //        OnPropertyChanged(() => this.FontHeight);
        //    }
        //}
    }
}
