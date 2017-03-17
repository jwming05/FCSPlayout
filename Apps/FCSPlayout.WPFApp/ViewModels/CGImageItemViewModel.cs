using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCSPlayout.CG;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace FCSPlayout.WPFApp.ViewModels
{
    public class CGImageItemViewModel: MLCGItemViewModelBase<MLCGImageItem> /*BindableBase*/
    {
        protected override BindableMLCGItemBase<MLCGImageItem> CreateBindableItem(MLCGImageItem item)
        {
            return new BindableCGImageItem(item);
        }
    }

    public abstract class MLCGItemViewModelBase<T> : BindableBase 
        where T : MLCGGenericItem
    {
        private ObservableCollection<BindableMLCGItemBase<T>> _bindableCGImageItems = 
            new ObservableCollection<BindableMLCGItemBase<T>>();
        private CGItemCollection _cgImageItems = new CGItemCollection();
        private BindableMLCGItemBase<T> _selectedCGImageItem;

        protected MLCGItemViewModelBase()
        {
        }

        public ObservableCollection<BindableMLCGItemBase<T>> BindableCGImageItems
        {
            get
            {
                return _bindableCGImageItems;
            }
        }

        public CGItemCollection CGItems
        {
            get { return _cgImageItems; }
            set
            {
                _cgImageItems.Reset(value.OfType<T>());
                _bindableCGImageItems.Clear();
                foreach (T item in _cgImageItems)
                {
                    _bindableCGImageItems.Add(/*new BindableCGImageItem(item)*/CreateBindableItem(item));
                }
            }
        }

        protected abstract BindableMLCGItemBase<T> CreateBindableItem(T item);

        public BindableMLCGItemBase<T> SelectedCGItem
        {
            get { return _selectedCGImageItem; }
            set
            {
                _selectedCGImageItem = value;
                OnPropertyChanged(() => this.SelectedCGItem);
            }
        }
    }
}
