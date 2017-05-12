using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileComparator
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            this.Loaded += Window1_Loaded;
            this.plotControl.ScaleX = 10;
            this.plotControl.ScaleY = -10;
            this.plotControl.Points = new MyPoints();
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            this.plotControl.SizeChanged += PlotControl_SizeChanged;

            this.plotControl.OriginX = this.plotControl.RenderSize.Width / 2;
            this.plotControl.OriginY = this.plotControl.RenderSize.Height / 2;

            this.sliderOriginX.Minimum = -1 * this.plotControl.RenderSize.Width;
            this.sliderOriginX.Maximum = this.plotControl.RenderSize.Width;

            this.sliderOriginY.Minimum = -1 * this.plotControl.RenderSize.Height;
            this.sliderOriginY.Maximum = this.plotControl.RenderSize.Height;
        }

        private void PlotControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.plotControl.OriginX = this.plotControl.RenderSize.Width / 2;
            this.plotControl.OriginY = this.plotControl.RenderSize.Height / 2;

            this.sliderOriginX.Minimum = -1 * this.plotControl.RenderSize.Width;
            this.sliderOriginX.Maximum = this.plotControl.RenderSize.Width;

            this.sliderOriginY.Minimum = -1 * this.plotControl.RenderSize.Height;
            this.sliderOriginY.Maximum = this.plotControl.RenderSize.Height;
        }
    }

    class MyPoints : IEnumerable<Point>
    {
        public IEnumerator<Point> GetEnumerator()
        {
            for(double x = 0.0; x <= Math.PI * 2; x += Math.PI / 10)
            {
                yield return new Point(x, Math.Sin(x));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
