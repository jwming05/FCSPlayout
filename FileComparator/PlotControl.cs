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
                new FrameworkPropertyMetadata(null, OnPointsPropertyChanged));

        public double OriginX
        {
            get { return (double)GetValue(OriginXProperty); }
            set { SetValue(OriginXProperty, value); }
        }

        public double OriginY
        {
            get { return (double)GetValue(OriginYProperty); }
            set { SetValue(OriginYProperty, value); }
        }

        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

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

        private static void OnPointsPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
        }


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

            var pt1 = new Point();
            var pt2 = new Point(100, 100);

            pt1 = MapWindowToViewport(pt1);
            pt2 = MapWindowToViewport(pt2);

            drawingContext.DrawLine(this.LinePen, pt1, pt2);

            //if(this.Points==null || !this.Points.Any())
            //{
            //    return;
            //}


            //Point prevPoint=this.MapInfo.MapWindowToViewport(this.Points.First());
            
            //foreach(var pt in this.Points)
            //{
            //    var current= this.MapInfo.MapWindowToViewport(pt);
            //    drawingContext.DrawLine(this.LinePen, prevPoint,current);
            //    prevPoint = current;
            //}
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
