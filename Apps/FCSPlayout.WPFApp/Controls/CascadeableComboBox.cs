using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPFApp
{
    public abstract class CascadeableComboBox : ComboBox, ICascadeable
    {
        public static readonly DependencyProperty DownStreamProperty =
            CascadeableObject.DownStreamProperty.AddOwner(typeof(CascadeableComboBox),
                new FrameworkPropertyMetadata(null, OnDownStreamPropertyChanged));

        private static void OnDownStreamPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((CascadeableComboBox)dpObj).OnDownStreamChanged((ICascadeable)e.OldValue,(ICascadeable)e.NewValue);
        }

        private void OnDownStreamChanged(ICascadeable oldValue, ICascadeable newValue)
        {
            InitDownStream();
        }

        protected abstract void InitDownStream();

        public ICascadeable DownStream
        {
            get { return (ICascadeable)GetValue(DownStreamProperty); }
            set { SetValue(DownStreamProperty, value); }
        }

        
        void ICascadeable.OnUpstreamChanged()
        {
            this.OnUpstreamChanged();

            // 强制传播一次
            if (this.DownStream != null)
            {
                this.DownStream.OnUpstreamChanged();
            }
        }

        void ICascadeable.Init(object obj)
        {
            this.Init(obj);
        }

        protected abstract void OnUpstreamChanged();
        protected abstract void Init(object obj);
    }
}
