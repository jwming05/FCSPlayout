using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FCSPlayout.WPF.Core
{
    public class MAudioChannelDrawer:Control
    {
        private IMAudioChannelDrawerHost _host;
        private float _indent = 3.0f;
        private float _gainH = 2.0f;

        public MAudioChannelDrawer(IMAudioChannelDrawerHost host)
        {
            _host = host;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // Draw background
            Rect rcBack = new Rect(_indent, _gainH / 2.0f, this.ActualWidth - _indent * 2, this.ActualHeight - _gainH);
            drawingContext.DrawRectangle(_host.ColorLevelBack, null, rcBack);

            // Draw level (original)
            Rect rcLevelOrg = rcBack;
            rcLevelOrg.Height *= Level2Pos(_host.LevelOrg/*_levelOrg*/);
            rcLevelOrg.Y = rcBack.Bottom - rcLevelOrg.Height;
            drawingContext.DrawRectangle(_host.ColorLevelOrg, null, rcLevelOrg);


            // Draw level (out)
            {
                // Hi - 0..-6
                // Mid - -6 ..-12
                // Low ... Other
                Rect rcLevelOut = rcBack;

                // Draw higher levels
                Brush brLevel;
                if (_host.Level >= -10.0)
                {
                    brLevel = _host.ColorLevelHi;
                }
                else if (_host.Level >= -20.0)
                {
                    brLevel = _host.ColorLevelMid;
                }
                else
                {
                    brLevel = _host.ColorLevelLo;
                }


                rcLevelOut.Height *= Level2Pos(_host.Level);
                rcLevelOut.Y = rcBack.Bottom - rcLevelOut.Height;

                drawingContext.DrawRectangle(brLevel, null, rcLevelOut);

                if (_host.Level >= -10.0f)
                {
                    rcLevelOut.Height = rcBack.Height * Level2Pos(-10.0f);
                    rcLevelOut.Y = rcBack.Bottom - rcLevelOut.Height;
                    drawingContext.DrawRectangle(_host.ColorLevelMid, null, rcLevelOut);
                }

                if (_host.Level >= -20.0)
                {
                    rcLevelOut.Height = rcBack.Height * Level2Pos(-20.0f);
                    rcLevelOut.Y = rcBack.Bottom - rcLevelOut.Height;
                    drawingContext.DrawRectangle(_host.ColorLevelLo, null, rcLevelOut);
                }
            }

            // Draw rectangle
            if (_host.Outline > 0.0)
            {
                Pen penOutline = new Pen(_host.ColorOutline, _host.Outline);
                drawingContext.DrawRectangle(null, penOutline, rcBack);
            }

            // Draw risks 
            if (_host.Risk > 0.0)
            {
                Pen penRisk = new Pen(_host.ColorLevelBack, 1);
                for (double fPos = _host.Risk; fPos < rcBack.Height; fPos += (_host.Risk + 1.0f))
                {
                    // Draw risk
                    drawingContext.DrawLine(penRisk, new Point(rcBack.Left, rcBack.Bottom - fPos), new Point(rcBack.Right - 1.0f, rcBack.Bottom - fPos));
                }
            }

            // Draw text

            // Draw gain
            Pen penGain = new Pen(_host.ColorGainSlider, _gainH);
            float fGainPos = (float)(rcBack.Bottom - rcBack.Height * Gain2Pos(_host.Gain));
            drawingContext.DrawLine(penGain, new Point(0, fGainPos), new Point(this.ActualWidth, fGainPos));
        }

        private static double Level2Pos(double levelValue)
        {
            if (levelValue < -30)
            {
                // 0..0.25
                levelValue = levelValue > -60.0f ? levelValue : -60.0f;  // [-60,-30)
                return 0.25f * (levelValue + 60.0f) / 30.0f;     // [0, 0.25)
            }
            else
            {
                levelValue = levelValue > 0 ? 0.0f : levelValue;  // [-30,0]
                return 0.25f + 0.75f * (levelValue + 30.0f) / 30.0f;  // [0.25,1]
            }

        }


        private static double Gain2Pos(double fGain)
        {
            fGain = fGain > 20.0f ? 20.0f : (fGain < -20.0f ? -20.0f : fGain);  // [-20,20]

            return (fGain + 20.0f) / 40.0f; // [0,1]
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                Rect rcBack = new Rect(_indent, _gainH / 2.0f, this.ActualWidth - _indent * 2, this.ActualHeight - _gainH);

                var y = e.GetPosition(this).Y;
                var minY = rcBack.Bottom - rcBack.Height;
                var maxY = rcBack.Bottom;

                y = y < minY ? minY : (y > maxY ? maxY : y);

                _host.Gain = ((rcBack.Bottom - y) / rcBack.Height) * 40.0f - 20.0f;

                //Invalidate();

                // Notify about playlist changing
                //OnGainChanged();
            }

            base.OnMouseMove(e);
        }
    }

    public interface IMAudioChannelDrawerHost
    {
        Brush ColorGainSlider { get; }
        Brush ColorLevelBack { get; }
        Brush ColorLevelHi { get; }
        Brush ColorLevelLo { get; }
        Brush ColorLevelMid { get; }
        Brush ColorLevelOrg { get; }
        Brush ColorOutline { get; }
        double Gain { get; set; }
        double Level { get; }
        double LevelOrg { get; }
        double Outline { get; }
        double Risk { get; }
    }
}
