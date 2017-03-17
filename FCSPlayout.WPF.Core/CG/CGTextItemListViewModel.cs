using FCSPlayout.CG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPF.Core
{
    public class CGTextItemListViewModel : CGItemListViewModelBase<MLCGTextItem>
    {
        public CGTextItemListViewModel()
        {
        }

        protected override BindableMLCGItemBase<MLCGTextItem> CreateBindableItem(MLCGTextItem item)
        {
            return new BindableCGTextItem(item);
        }

        protected override BindableMLCGItemBase<MLCGTextItem> CreateBindableItem()
        {
            return new BindableCGTextItem() { Name = "未命名" };
        }
    }
}
