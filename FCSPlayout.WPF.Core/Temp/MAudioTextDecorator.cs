using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FCSPlayout.WPF.Core
{
    public class MAudioTextDecorator : Control
    {
        public static readonly DependencyProperty BackColorHiProperty =
            DependencyProperty.Register("BackColorHi", typeof(Brush), typeof(MAudioTextDecorator),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(222, 232, 254)), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BackColorHi
        {
            get { return (Brush)GetValue(BackColorHiProperty); }
            set { SetValue(BackColorHiProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            int nIndent = 0;

            string[] pLevel = new string[] { "0dB", "-10", "-20", "-30", "-60" };
            string[] pGain = new string[] { "+20", "+10", "0dB", "-10", "-20" };

            double fStep = this.ActualHeight / (pLevel.Length - 1);

            Rect rcBG_Block = new Rect(0, 0, this.ActualWidth, fStep);

            Rect rcString = new Rect(0, 0, this.ActualWidth, this.FontSize);
            for (int i = 0; i < pLevel.Length; i++)
            {
                rcBG_Block.Y = nIndent + fStep * i;
                drawingContext.DrawRectangle(i % 2 == 0 ? this.BackColorHi : this.Background, null, rcBG_Block);
                if (i > 0)
                {
                    if (i != pLevel.Length - 1)
                    {
                        rcString.Y = rcBG_Block.Y - this.FontSize / 2.0 - 3;
                    }
                    else
                    {
                        rcString.Y = rcBG_Block.Y - this.FontSize - 3;
                    }
                }
                else
                {
                    rcString.Y = rcBG_Block.Y;
                }

                DrawText(drawingContext, pLevel[i], new Point(rcString.X, rcString.Y));
                var formattedText = GetFormattedText(pGain[i]);
                drawingContext.DrawText(formattedText, new Point(this.ActualWidth - formattedText.WidthIncludingTrailingWhitespace - 2, rcString.Y));
            }
        }

        private void DrawText(DrawingContext drawingContext, string text, Point origin)
        {
            FormattedText formattedText = GetFormattedText(text);
            drawingContext.DrawText(formattedText, origin);
        }

        private FormattedText GetFormattedText(string text)
        {
            double emSize = this.FontSize;
            Typeface typeface = new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch);
            return new FormattedText(text, System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, emSize, this.Foreground);
        }
    }
}
