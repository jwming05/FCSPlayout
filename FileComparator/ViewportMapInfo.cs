using System.Windows;

namespace FileComparator
{
    public class ViewportMapInfo
    {
        /// <summary>
        /// 获取或设置窗口坐标原点在视口中的坐标。
        /// </summary>
        public Point Origin { get; set; }

        /// <summary>
        /// 获取或设置X轴上视口度量与窗口度量之比（视口度量/窗口度量）。
        /// </summary>
        public double XScale { get; set; }

        /// <summary>
        /// 获取或设置Y轴上视口度量与窗口度量之比（视口度量/窗口度量）。
        /// </summary>
        public double YScale { get; set; }

        /// <summary>
        /// 把窗口坐标映射为视口坐标。
        /// </summary>
        /// <param name="pt">指定表示窗口坐标的点。</param>
        /// <returns>表示视口坐标的点。</returns>
        public Point MapWindowToViewport(Point pt)
        {
            return new Point(pt.X * XScale + Origin.X, pt.Y * YScale + Origin.Y);
        }

        /// <summary>
        /// 把视口坐标映射为窗口坐标。
        /// </summary>
        /// <param name="pt">指定表示视口坐标的点。</param>
        /// <returns>表示窗口坐标的点。</returns>
        public Point MapViewportToWindow(Point pt)
        {
            return new Point((pt.X - Origin.X) / XScale, (pt.Y - Origin.Y) / YScale);
        }
    }
}
