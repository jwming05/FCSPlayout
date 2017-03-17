using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// CGImageItemView.xaml 的交互逻辑
    /// </summary>
    public partial class CGTickerItemView : UserControl
    {
        public static readonly DependencyProperty CGTickerItemProperty =
            DependencyProperty.Register("CGTickerItem", typeof(BindableCGTickerItem), typeof(CGTickerItemView), 
                new FrameworkPropertyMetadata(null));


        public CGTickerItemView()
        {
            InitializeComponent();
        }

        public BindableCGTickerItem CGTickerItem
        {
            get { return (BindableCGTickerItem)GetValue(CGTickerItemProperty); }
            set { SetValue(CGTickerItemProperty, value); }
        }
    }
}
