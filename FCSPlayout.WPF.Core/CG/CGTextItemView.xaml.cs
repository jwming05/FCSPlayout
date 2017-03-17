using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// CGImageItemView.xaml 的交互逻辑
    /// </summary>
    public partial class CGTextItemView : UserControl
    {
        public static readonly DependencyProperty CGTextItemProperty =
            DependencyProperty.Register("CGTextItem", typeof(BindableCGTextItem), typeof(CGTextItemView), 
                new FrameworkPropertyMetadata(null));


        public CGTextItemView()
        {
            InitializeComponent();
        }

        public BindableCGTextItem CGTextItem
        {
            get { return (BindableCGTextItem)GetValue(CGTextItemProperty); }
            set { SetValue(CGTextItemProperty, value); }
        }
    }
}
