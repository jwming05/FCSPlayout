using MPLATFORMLib;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// PreviewControl.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewControl : UserControl
    {
        public static readonly DependencyProperty MObjectProperty =
            DependencyProperty.Register("MObject", typeof(IMObject), typeof(PreviewControl), 
                new FrameworkPropertyMetadata(null, OnMObjectPropertyChanged));

        private static void OnMObjectPropertyChanged(DependencyObject dpObj,DependencyPropertyChangedEventArgs e)
        {
            ((PreviewControl)dpObj).OnMObjectChanged((IMObject)e.OldValue,(IMObject)e.NewValue);
        }

        private void OnMObjectChanged(IMObject oldValue, IMObject newValue)
        {
            if (this.MObject != null)
            {
                Start(this.MObject);
            }
            else
            {
                Stop();
            }
        }

        public PreviewControl()
        {
            InitializeComponent();
        }

        public IMObject MObject
        {
            get { return (IMObject)GetValue(MObjectProperty); }
            set { SetValue(MObjectProperty, value); }
        }

        public void Start(IMObject mobj)
        {
            string name;
            mobj.ObjectNameGet(out name);

            this.mediaElement.Source = new Uri("mplatform://" + name);
            this.audioMeter.SetControlledObject(mobj);
        }

        public void Stop()
        {
            this.mediaElement.Source = null;
            this.audioMeter.SetControlledObject(null);
        }
    }
}
