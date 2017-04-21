using MPLATFORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FCSPlayout.WPFApp
{
    public abstract class FormatComboBoxBase : CascadeableComboBox
    {
        public static readonly DependencyProperty SelectedFormatProperty =
            DependencyProperty.Register("SelectedFormat", typeof(string), typeof(FormatComboBoxBase),
            new FrameworkPropertyMetadata(null, OnSelectedFormatPropertyChanged));

        private static void OnSelectedFormatPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((FormatComboBoxBase)dpObj).OnSelectedFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        public static readonly DependencyProperty MFormatObjectProperty =
            DependencyProperty.Register("MFormatObject", typeof(IMFormat), typeof(FormatComboBoxBase),
            new FrameworkPropertyMetadata(null, OnMFormatObjectPropertyChanged));

        private static void OnMFormatObjectPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((FormatComboBoxBase)dpObj).OnMFormatObjectChanged((IMFormat)e.OldValue, (IMFormat)e.NewValue);
        }

        public static readonly DependencyProperty FormatTypeProperty =
            DependencyProperty.Register("FormatType", typeof(eMFormatType), typeof(FormatComboBoxBase),
            new FrameworkPropertyMetadata(eMFormatType.eMFT_Convert));

        public FormatComboBoxBase()
        {
            Binding binding = new Binding("SelectedFormat");
            binding.Source = this;
            binding.Mode = BindingMode.OneWayToSource; // BindingMode.TwoWay;
            this.SetBinding(SelectedItemProperty, binding);
        }

        public IMFormat MFormatObject
        {
            get { return (IMFormat)GetValue(MFormatObjectProperty); }
            set { SetValue(MFormatObjectProperty, value); }
        }

        public eMFormatType FormatType
        {
            get { return (eMFormatType)GetValue(FormatTypeProperty); }
            set { SetValue(FormatTypeProperty, value); }
        }

        public string SelectedFormat
        {
            get { return (string)GetValue(SelectedFormatProperty); }
            set { SetValue(SelectedFormatProperty, value); }
        }       

        protected abstract void OnSelectedFormatChanged();
        protected abstract int PopulateFormatItems();

        protected override void OnUpstreamChanged()
        {
            UpdateItems();
        }

        protected override void InitDownStream()
        {
            if (this.DownStream != null)
            {
                this.DownStream.Init(this.MFormatObject);
            }
        }

        protected override void Init(object obj)
        {
            this.MFormatObject = obj as IMFormat;
        }

        private void OnMFormatObjectChanged(IMFormat oldValue, IMFormat newValue)
        {
            this.InitDownStream();
            UpdateItems();
        }

        private void UpdateItems()
        {
            this.Items.Clear();
            if (this.MFormatObject != null)
            {
                this.SelectedIndex = PopulateFormatItems();
            }
        }

        private void OnSelectedFormatChanged(string oldValue, string newValue)
        {
            if (this.SelectedFormat != null && this.MFormatObject != null)
            {
                OnSelectedFormatChanged();
                if (DownStream != null)
                {
                    DownStream.OnUpstreamChanged();
                }
            }
        }
    }
}
