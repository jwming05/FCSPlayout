using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileComparator
{
    class PlotControl:Control
    {
        //public static readonly DependencyProperty MapInfoProperty =
        //    DependencyProperty.Register("MapInfo", typeof(ViewportMapInfo), typeof(PlotControl), 
        //        new FrameworkPropertyMetadata(null, OnMapInfoPropertyChanged));

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(IEnumerable<Point>), typeof(PlotControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置窗口坐标原点在视口中的X坐标。
        /// </summary>
        public double OriginX
        {
            get { return (double)GetValue(OriginXProperty); }
            set { SetValue(OriginXProperty, value); }
        }

        /// <summary>
        /// 获取或设置窗口坐标原点在视口中的Y坐标。
        /// </summary>
        public double OriginY
        {
            get { return (double)GetValue(OriginYProperty); }
            set { SetValue(OriginYProperty, value); }
        }

        /// <summary>
        /// 获取或设置X轴上视口度量与窗口度量之比（视口度量/窗口度量）。
        /// </summary>
        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        /// <summary>
        /// 获取或设置Y轴上视口度量与窗口度量之比（视口度量/窗口度量）。
        /// </summary>
        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register("ScaleY", typeof(double), typeof(PlotControl), 
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender), ValidateScale);


        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register("ScaleX", typeof(double), typeof(PlotControl), 
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender), ValidateScale);


        public static readonly DependencyProperty OriginYProperty =
            DependencyProperty.Register("OriginY", typeof(double), typeof(PlotControl), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));


        public static readonly DependencyProperty OriginXProperty =
            DependencyProperty.Register("OriginX", typeof(double), typeof(PlotControl), 
                new FrameworkPropertyMetadata(0.0,FrameworkPropertyMetadataOptions.AffectsRender));

        private static bool ValidateScale(object value)
        {
            return ((double)value) != 0.0;
        }

        //private static void OnMapInfoPropertyChanged(DependencyObject dpObj,DependencyPropertyChangedEventArgs e)
        //{
        //}



        public PlotControl()
        {
            this.LinePen = new Pen(Brushes.Black, 1.0);
        }

        public IEnumerable<Point> Points
        {
            get { return (IEnumerable<Point>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        //public ViewportMapInfo MapInfo
        //{
        //    get { return (ViewportMapInfo)GetValue(MapInfoProperty); }
        //    set { SetValue(MapInfoProperty, value); }
        //}

        public Pen LinePen { get; private set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // draw x axis
            drawingContext.DrawLine(this.LinePen, new Point(0,this.OriginY), new Point(this.RenderSize.Width, this.OriginY));
            
            // draw y axis
            drawingContext.DrawLine(this.LinePen, new Point(this.OriginX, 0), new Point(this.OriginX, this.RenderSize.Height));


            if (this.Points!=null && this.Points.Any())
            {
                var prevPt = MapWindowToViewport(this.Points.First());
                foreach(var pt in this.Points)
                {
                    var current= MapWindowToViewport(pt);
                    drawingContext.DrawLine(this.LinePen, prevPt, current);
                    prevPt = current;
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
        }

        /// <summary>
        /// 把窗口坐标映射为视口坐标。
        /// </summary>
        /// <param name="pt">指定表示窗口坐标的点。</param>
        /// <returns>表示视口坐标的点。</returns>
        public Point MapWindowToViewport(Point pt)
        {
            return new Point(pt.X * ScaleX + OriginX, pt.Y * ScaleY + OriginY);
        }

        /// <summary>
        /// 把视口坐标映射为窗口坐标。
        /// </summary>
        /// <param name="pt">指定表示视口坐标的点。</param>
        /// <returns>表示窗口坐标的点。</returns>
        public Point MapViewportToWindow(Point pt)
        {
            return new Point((pt.X - OriginX) / ScaleX, (pt.Y - OriginY) / ScaleY);
        }
    }
}
