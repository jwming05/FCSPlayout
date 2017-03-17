using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// CGImageItemView.xaml 的交互逻辑
    /// </summary>
    public partial class CGImageItemView : UserControl
    {
        

        // Using a DependencyProperty as the backing store for CGImageItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CGImageItemProperty =
            DependencyProperty.Register("CGImageItem", typeof(BindableCGImageItem), typeof(CGImageItemView), 
                new FrameworkPropertyMetadata(null));


        public CGImageItemView()
        {
            InitializeComponent();
        }

        public BindableCGImageItem CGImageItem
        {
            get { return (BindableCGImageItem)GetValue(CGImageItemProperty); }
            set { SetValue(CGImageItemProperty, value); }
        }
    }
}
