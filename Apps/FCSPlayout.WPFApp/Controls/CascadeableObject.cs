using System.Windows;

namespace FCSPlayout.WPFApp
{
    public class CascadeableObject : DependencyObject
    {
        public static readonly DependencyProperty DownStreamProperty =
            DependencyProperty.Register("DownStream", typeof(ICascadeable), typeof(CascadeableObject),
            new FrameworkPropertyMetadata(null));

        //public ICascadeable DownStream
        //{
        //    get { return (ICascadeable)GetValue(DownStreamProperty); }
        //    set { SetValue(DownStreamProperty, value); }
        //}
    }
}
