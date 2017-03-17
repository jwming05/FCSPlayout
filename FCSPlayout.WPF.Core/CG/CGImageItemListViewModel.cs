using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCSPlayout.CG;
using Prism.Mvvm;
using Prism.Commands;
using System.Windows.Input;

namespace FCSPlayout.WPF.Core
{
    public class CGImageItemListViewModel: CGItemListViewModelBase<MLCGImageItem>
    {
        public CGImageItemListViewModel()
        {
        }

        protected override BindableMLCGItemBase<MLCGImageItem> CreateBindableItem(MLCGImageItem item)
        {
            return new BindableCGImageItem(item);
        }

        protected override BindableMLCGItemBase<MLCGImageItem> CreateBindableItem()
        {
            return new BindableCGImageItem() { Name = "未命名" };
        }    
    }
}
