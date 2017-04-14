using FCSPlayout.CG;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPF.Core
{
    public abstract class BindableMLCGItemBase<T> : BindableBase
        where T : MLCGGenericItem
    {
        private readonly T _item;
        private bool _isNew;

        protected BindableMLCGItemBase(T item, bool isNew)
        {
            _item = item;
            _isNew = isNew;
        }

        public double X
        {
            get { return _item.X; }
            set
            {
                _item.X = value;
                this.RaisePropertyChanged(nameof(this.X));
            }
        }

        public double Y
        {
            get { return _item.Y; }
            set
            {
                _item.Y = value;
                RaisePropertyChanged(nameof(this.Y));
            }
        }

        public bool IsRelative
        {
            get { return _item.IsRelative; }
            set
            {
                _item.IsRelative = value;
                RaisePropertyChanged(nameof(this.IsRelative));
            }
        }

        public string Name
        {
            get { return _item.ItemName; }
            set
            {
                _item.ItemName = value;
                RaisePropertyChanged(nameof(this.Name));
            }
        }

        internal T Item
        {
            get
            {
                return _item;
            }
        }

        public bool IsNew
        {
            get
            {
                return _isNew;
            }
        }
    }
}
