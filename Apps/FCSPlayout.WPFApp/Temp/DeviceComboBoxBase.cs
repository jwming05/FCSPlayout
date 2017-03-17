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
    public abstract class DeviceComboBoxBase : CascadeableComboBox
    {
        public string SelectedDevice
        {
            get { return (string)GetValue(SelectedDeviceProperty); }
            set { SetValue(SelectedDeviceProperty, value); }
        }

        public static readonly DependencyProperty SelectedDeviceProperty =
            DependencyProperty.Register("SelectedDevice", typeof(string), typeof(DeviceComboBoxBase), 
            new FrameworkPropertyMetadata(null, OnSelectedDevicePropertyChanged));


        public static readonly DependencyProperty DeviceTypeProperty =
            DependencyProperty.Register("DeviceType", typeof(string), typeof(DeviceComboBoxBase), 
            new FrameworkPropertyMetadata(null, OnDeviceTypePropertyChanged));

        public static readonly DependencyProperty MDeviceProperty =
            DependencyProperty.Register("MDevice", typeof(IMDevice), typeof(DeviceComboBoxBase), 
            new FrameworkPropertyMetadata(null, OnMDevicePropertyChanged));

        private static void OnMDevicePropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((DeviceComboBoxBase)dpObj).OnMDeviceChanged((IMDevice)e.OldValue, (IMDevice)e.NewValue);
        }

        private static void OnDeviceTypePropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((DeviceComboBoxBase)dpObj).OnDeviceTypeChanged((string)e.OldValue, (string)e.NewValue);
        }

        private static void OnSelectedDevicePropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((DeviceComboBoxBase)dpObj).OnSelectedDeviceChanged((string)e.OldValue, (string)e.NewValue);
        }

        public DeviceComboBoxBase()
        {
            Binding binding = new Binding("SelectedDevice");
            binding.Source = this;
            binding.Mode = BindingMode.OneWayToSource; // BindingMode.TwoWay;
            this.SetBinding(SelectedItemProperty, binding);
        }

        public IMDevice MDevice
        {
            get { return (IMDevice)GetValue(MDeviceProperty); }
            set { SetValue(MDeviceProperty, value); }
        }

        public string DeviceType
        {
            get { return (string)GetValue(DeviceTypeProperty); }
            set { SetValue(DeviceTypeProperty, value); }
        }

        private void OnDeviceTypeChanged(string oldValue, string newValue)
        {
            UpdateItems();
        }

        private void OnMDeviceChanged(IMDevice oldValue, IMDevice newValue)
        {
            this.InitDownStream();
            UpdateItems();
        }

        private void UpdateItems()
        {
            this.Items.Clear();
            if (MDevice != null && this.DeviceType!=null)
            {
                int count;
                this.MDevice.DeviceGetCount(0, this.DeviceType, out count);
                string name, description;
                int index = 0;
                for (; index < count; index++)
                {
                    this.MDevice.DeviceGetByIndex(0, this.DeviceType, index, out name, out description);
                    this.Items.Add(name);
                }

                if (count > 0)
                {
                    string parameter;
                    this.MDevice.DeviceGet(this.DeviceType, out name, out parameter, out index);
                    this.SelectedIndex = index;
                }
            }
        }

        private void OnSelectedDeviceChanged(string oldValue, string newValue)
        {
            if(this.SelectedDevice!=null && this.MDevice != null && this.DeviceType != null)
            {
                this.MDevice.DeviceSet(this.DeviceType, this.SelectedDevice, string.Empty);
                if (this.DownStream != null)
                {
                    this.DownStream.OnUpstreamChanged();
                }
            }
        }

        protected override void OnUpstreamChanged()
        {
            UpdateItems();
        }

        protected override void InitDownStream()
        {
            if (this.DownStream != null)
            {
                this.DownStream.Init(this.MDevice);
            }
        }

        protected override void Init(object obj)
        {
            this.MDevice = obj as IMDevice;
        }
    }
}
