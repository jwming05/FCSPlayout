using FCSPlayout.CG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                OnPropertyChanged(() => this.Text);
            }
        }

        public int FontHeight
        {
            get { return this.Item.FontHeight; }
            set
            {
                this.Item.FontHeight = value;
                OnPropertyChanged(() => this.FontHeight);
            }
        }
    }
}
