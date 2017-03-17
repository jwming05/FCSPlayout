using FCSPlayout.CG;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPF.Core
{
    
    public class BindableCGImageItem : BindableMLCGItemBase<MLCGImageItem>
    {
        public BindableCGImageItem()
            :base(new MLCGImageItem(),true)
        {
        }
        public BindableCGImageItem(MLCGImageItem item)
            :base(item,false)
        {
        }

        public string File
        {
            get { return this.Item.File; }
            set
            {
                this.Item.File = value;
                OnPropertyChanged(() => this.File);
            }
        }
    }
}
