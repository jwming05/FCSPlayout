using MPLATFORMLib;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// MLPreviewControl.xaml 的交互逻辑
    /// </summary>
    public partial class MLPreviewControl : UserControl
    {
        public static DependencyProperty MObjectProperty = MAudioMeter.MObjectProperty.AddOwner(typeof(MLPreviewControl));

        public MLPreviewControl()
        {
            InitializeComponent();
        }

        public IMObject MObject
        {
            get { return (IMObject)GetValue(MObjectProperty); }
            set { SetValue(MObjectProperty, value); }
        }
    }

    public class MediaElementSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IMObject mobject = value as IMObject;
            if (mobject != null)
            {
                string name = null;
                mobject.ObjectNameGet(out name);
                return new Uri("mplatform://" + name);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
