using FCSPlayout.CG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPF.Core
{
    public class CGTickerItemListViewModel : CGItemListViewModelBase<MLCGTickerItem>
    {
        public CGTickerItemListViewModel()
        {
        }

        protected override BindableMLCGItemBase<MLCGTickerItem> CreateBindableItem(MLCGTickerItem item)
        {
            return new BindableCGTickerItem(item);
        }

        protected override BindableMLCGItemBase<MLCGTickerItem> CreateBindableItem()
        {
            return new BindableCGTickerItem() { Name = "未命名" };
        }
    }
}
