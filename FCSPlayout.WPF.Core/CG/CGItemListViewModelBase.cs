using FCSPlayout.CG;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FCSPlayout.WPF.Core
{
    public abstract class CGItemListViewModelBase<T> : BindableBase
        where T : MLCGGenericItem
    {
        private ObservableCollection<BindableMLCGItemBase<T>> _cgItemsList = new ObservableCollection<BindableMLCGItemBase<T>>();
        private CGItemCollection _cgItems = new CGItemCollection();
        private BindableMLCGItemBase<T> _selectedItem = null;
        private readonly DelegateCommand _addItemCommand;
        private readonly DelegateCommand _removeItemCommand;

        public CGItemListViewModelBase()
        {
            _addItemCommand = new DelegateCommand(AddItem, CanAddItem);
            _removeItemCommand = new DelegateCommand(RemoveItem, CanRemoveItem);
        }

        private bool CanRemoveItem()
        {
            return this.SelectedItem != null;
        }

        private void RemoveItem()
        {
            if (CanRemoveItem())
            {
                var item = this.SelectedItem;
                this.CGItemsList.Remove(item);
                this.CGItems.Remove(item.Item);

                this.SelectedItem = null;
            }
        }

        private bool CanAddItem()
        {
            return true;
        }

        private void AddItem()
        {
            if (CanAddItem())
            {
                var newItem = CreateBindableItem();
                this.CGItems.Add(newItem.Item);
                this.CGItemsList.Add(newItem);

                this.SelectedItem = newItem;
            }
        }

        public ObservableCollection<BindableMLCGItemBase<T>> CGItemsList
        {
            get { return _cgItemsList; }
        }

        public CG.CGItemCollection CGItems
        {
            get { return _cgItems; }
            set
            {
                _cgItems.Reset(value != null ? value.OfType<T>() : (IEnumerable<T>)value);

                _cgItemsList.Clear();
                foreach (T item in _cgItems)
                {
                    _cgItemsList.Add(CreateBindableItem(item));
                }
            }
        }

        protected abstract BindableMLCGItemBase<T> CreateBindableItem(T item);
        protected abstract BindableMLCGItemBase<T> CreateBindableItem();

        public BindableMLCGItemBase<T> SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(() => this.SelectedItem);
                OnPropertyChanged(() => this.HasSelectedItem);
                _removeItemCommand.RaiseCanExecuteChanged();
            }
        }

        public bool HasSelectedItem
        {
            get { return this.SelectedItem != null; }
        }

        public ICommand AddItemCommand
        {
            get
            {
                return _addItemCommand;
            }
        }

        public ICommand RemoveItemCommand
        {
            get
            {
                return _removeItemCommand;
            }
        }
    }
}
