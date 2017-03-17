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
    public class MAudioChannel : Control
    {
        #region
        public static readonly DependencyProperty ColorLevelBackProperty =
            DependencyProperty.Register("ColorLevelBack", typeof(Brush), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ColorLevelOrgProperty =
            DependencyProperty.Register("ColorLevelOrg", typeof(Brush), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(Brushes.Silver, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ColorLevelHiProperty =
            DependencyProperty.Register("ColorLevelHi", typeof(Brush), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ColorLevelMidProperty =
            DependencyProperty.Register("ColorLevelMid", typeof(Brush), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(Brushes.Yellow, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ColorLevelLoProperty =
            DependencyProperty.Register("ColorLevelLo", typeof(Brush), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(Brushes.Green, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ColorOutlineProperty =
            DependencyProperty.Register("ColorOutline", typeof(Brush), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ColorGainSliderProperty =
            DependencyProperty.Register("ColorGainSlider", typeof(Brush), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty OutlineProperty =
            DependencyProperty.Register("Outline", typeof(double), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender), ValidateOutlineValue);

        public static readonly DependencyProperty LevelOrgProperty =
            DependencyProperty.Register("LevelOrg", typeof(double), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(-10.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceLevelValue));

        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(double), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(-20.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceLevelValue)/*, ValidateLevelValue*/);

        public static readonly DependencyProperty GainProperty =
            DependencyProperty.Register("Gain", typeof(double), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceGainValue));

        public static readonly DependencyProperty RiskProperty =
            DependencyProperty.Register("Risk", typeof(double), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender), ValidateRiskValue);

        public static readonly DependencyProperty TextLeftProperty =
            DependencyProperty.Register("TextLeft", typeof(string[]), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextRightProperty =
            DependencyProperty.Register("TextRight", typeof(string[]), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        private static bool ValidateOutlineValue(object value)
        {
            return (double)value >= 0.0;
        }

        //private static bool ValidateLevelValue(object value)
        //{
        //    double dValue = (double)value;
        //    return dValue >= -60.0 && dValue <= 0.0;
        //}

        private static object CoerceLevelValue(DependencyObject dpObj, object value)
        {
            double dValue = (double)value;
            return Math.Min(0.0, Math.Max(-60.0, dValue));
        }

        private static object CoerceGainValue(DependencyObject dpObj, object value)
        {
            double dValue = (double)value;
            return Math.Min(20.0, Math.Max(-20.0, dValue));
        }

        private static bool ValidateRiskValue(object value)
        {
            double dValue = (double)value;
            return dValue >= 0.0;
        }

        public static RoutedEvent GainChangedEvent = EventManager.RegisterRoutedEvent("GainChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MAudioChannel));

        public RoutedEvent EnableChangedEvent = EventManager.RegisterRoutedEvent("EnableChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MAudioChannel));

        public static readonly DependencyProperty EnableControllerProperty =
            DependencyProperty.Register("EnableController", typeof(CheckBox), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(null, OnEnableControllerPropertyChanged));

        private static void OnEnableControllerPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((MAudioChannel)dpObj).OnEnableControllerChanged((CheckBox)e.OldValue, (CheckBox)e.NewValue);
        }
        #endregion

        #region
        public Brush ColorLevelBack
        {
            get { return (Brush)GetValue(ColorLevelBackProperty); }
            set { SetValue(ColorLevelBackProperty, value); }
        }

        public Brush ColorLevelOrg
        {
            get { return (Brush)GetValue(ColorLevelOrgProperty); }
            set { SetValue(ColorLevelOrgProperty, value); }
        }

        public Brush ColorLevelHi
        {
            get { return (Brush)GetValue(ColorLevelHiProperty); }
            set { SetValue(ColorLevelHiProperty, value); }
        }

        public Brush ColorLevelMid
        {
            get { return (Brush)GetValue(ColorLevelMidProperty); }
            set { SetValue(ColorLevelMidProperty, value); }
        }

        public Brush ColorLevelLo
        {
            get { return (Brush)GetValue(ColorLevelLoProperty); }
            set { SetValue(ColorLevelLoProperty, value); }
        }

        public Brush ColorOutline
        {
            get { return (Brush)GetValue(ColorOutlineProperty); }
            set { SetValue(ColorOutlineProperty, value); }
        }

        public Brush ColorGainSlider
        {
            get { return (Brush)GetValue(ColorGainSliderProperty); }
            set { SetValue(ColorGainSliderProperty, value); }
        }

        public double Outline
        {
            get { return (double)GetValue(OutlineProperty); }
            set { SetValue(OutlineProperty, value); }
        }

        public double LevelOrg
        {
            get { return (double)GetValue(LevelOrgProperty); }
            set { SetValue(LevelOrgProperty, value); }
        }

        public double Level
        {
            get { return (double)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public double Gain
        {
            get { return (double)GetValue(GainProperty); }
            set { SetValue(GainProperty, value); }
        }

        public double Risk
        {
            get { return (double)GetValue(RiskProperty); }
            set { SetValue(RiskProperty, value); }
        }

        public string[] TextLeft
        {
            get { return (string[])GetValue(TextLeftProperty); }
            set { SetValue(TextLeftProperty, value); }
        }

        public string[] TextRight
        {
            get { return (string[])GetValue(TextRightProperty); }
            set { SetValue(TextRightProperty, value); }
        }

        public CheckBox EnableController
        {
            get { return (CheckBox)GetValue(EnableControllerProperty); }
            set { SetValue(EnableControllerProperty, value); }
        }
        #endregion

        public event RoutedEventHandler GainChanged
        {
            add
            {
                this.AddHandler(GainChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(GainChangedEvent, value);
            }
        }

        public event RoutedEventHandler EnableChanged
        {
            add { this.AddHandler(EnableChangedEvent, value); }
            remove { this.RemoveHandler(EnableChangedEvent, value); }
        }

        private float _indent = 3.0f;
        private float _gainH = 2.0f;

        protected override void OnRender(DrawingContext drawingContext)
        {
            // Draw background
            Rect rcBack = new Rect(_indent, _gainH / 2.0f, this.ActualWidth - _indent * 2, this.ActualHeight - _gainH);
            drawingContext.DrawRectangle(this.ColorLevelBack, null, rcBack);

            // Draw level (original)
            Rect rcLevelOrg = rcBack;
            rcLevelOrg.Height *= Level2Pos(this.LevelOrg/*_levelOrg*/);
            rcLevelOrg.Y = rcBack.Bottom - rcLevelOrg.Height;
            drawingContext.DrawRectangle(this.ColorLevelOrg, null, rcLevelOrg);


            // Draw level (out)
            {
                // Hi - 0..-6
                // Mid - -6 ..-12
                // Low ... Other
                Rect rcLevelOut = rcBack;

                // Draw higher levels
                Brush brLevel;
                if (this.Level >= -10.0)
                {
                    brLevel = this.ColorLevelHi;
                }
                else if (this.Level >= -20.0)
                {
                    brLevel = this.ColorLevelMid;
                }
                else
                {
                    brLevel = this.ColorLevelLo;
                }


                rcLevelOut.Height *= Level2Pos(this.Level);
                rcLevelOut.Y = rcBack.Bottom - rcLevelOut.Height;

                drawingContext.DrawRectangle(brLevel, null, rcLevelOut);

                if (this.Level >= -10.0f)
                {
                    rcLevelOut.Height = rcBack.Height * Level2Pos(-10.0f);
                    rcLevelOut.Y = rcBack.Bottom - rcLevelOut.Height;
                    drawingContext.DrawRectangle(this.ColorLevelMid, null, rcLevelOut);
                }

                if (this.Level >= -20.0)
                {
                    rcLevelOut.Height = rcBack.Height * Level2Pos(-20.0f);
                    rcLevelOut.Y = rcBack.Bottom - rcLevelOut.Height;
                    drawingContext.DrawRectangle(this.ColorLevelLo, null, rcLevelOut);
                }
            }

            // Draw rectangle
            if (this.Outline > 0)
            {
                Pen penOutline = new Pen(this.ColorOutline, this.Outline);
                drawingContext.DrawRectangle(null, penOutline, rcBack);
            }

            // Draw risks 
            if (this.Risk > 0)
            {
                Pen penRisk = new Pen(this.ColorLevelBack, 1);
                for (double fPos = this.Risk; fPos < rcBack.Height; fPos += (this.Risk + 1.0f))
                {
                    // Draw risk
                    drawingContext.DrawLine(penRisk, new Point(rcBack.Left, rcBack.Bottom - fPos), new Point(rcBack.Right - 1.0f, rcBack.Bottom - fPos));
                }
            }

            // Draw text
            //if ((_leftText != null && _leftText.Length > 1))
            //{
            //    float fStep = (float)rcBack.Height / (_leftText.Length - 1);

            //    Brush brText = this.Foreground; // new SolidColorBrush(this.ForeColor);

            //    Rect rcString = new Rect(0, 0, this.ActualWidth/*Bounds.Width*/, this.FontSize/*Font.Height*/);
            //    for (int i = 0; i < _leftText.Length; i++)
            //    {
            //        rcString.Y = fStep * i;
            //        //var ft=new FormattedText(leftText[i],System.Globalization.CultureInfo.CurrentUICulture,FlowDirection.LeftToRight,new Typeface(),
            //        //drawingContext.DrawText(new FormattedText())
            //        //e.Graphics.DrawString(leftText[i], this.Font, brText, rcString);
            //    }
            //}

            //if ((rightText != null && rightText.Length > 1))
            //{
            //    float fStep = (float)rcBack.Height / (rightText.Length - 1);

            //    Brush brText = new SolidBrush(this.ForeColor);

            //    StringFormat strFormat = new StringFormat();
            //    strFormat.Alignment = StringAlignment.Far;

            //    RectangleF rcString = new RectangleF(0, 0, Bounds.Width, Font.Height);
            //    for (int i = 0; i < rightText.Length; i++)
            //    {
            //        rcString.Y = fStep * i;
            //        e.Graphics.DrawString(rightText[i], this.Font, brText, rcString, strFormat);
            //    }
            //}

            // Draw gain
            Pen penGain = new Pen(this.ColorGainSlider, _gainH);
            float fGainPos = (float)(rcBack.Bottom - rcBack.Height * Gain2Pos(this.Gain));
            drawingContext.DrawLine(penGain, new Point(0, fGainPos), new Point(this.ActualWidth, fGainPos));
        }

        // For non-lenear display
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

                this.Gain = ((rcBack.Bottom - y) / rcBack.Height) * 40.0f - 20.0f;

                //Invalidate();

                // Notify about playlist changing
                OnGainChanged();
            }

            base.OnMouseMove(e);
        }

        //protected override void OnMouseDown(MouseButtonEventArgs e)
        //{
        //    float fIndent = 3.0f;
        //    float fGainH = 2.0f;
        //    Rect rcBack = new Rect(fIndent, fGainH / 2.0f, this.ActualWidth - fIndent * 2, this.ActualWidth - fGainH);

        //    var y = e.GetPosition(this).Y;
        //    var minY = rcBack.Bottom - rcBack.Height;
        //    var maxY = rcBack.Bottom;

        //    y = y < minY ? minY : (y > maxY ? maxY : y);

        //    this.Gain = ((rcBack.Bottom - y) / rcBack.Height) * 40.0f - 20.0f;
        //    //Invalidate();

        //    if (this.OnGainChanged != null)
        //    {
        //        this.OnGainChanged(this, e);
        //    }

        //    base.OnMouseDown(e);
        //}

        private void OnGainChanged()
        {
            this.RaiseEvent(new RoutedEventArgs(GainChangedEvent));
        }
        

        private void OnEnableControllerChanged(CheckBox oldValue, CheckBox newValue)
        {
            if (oldValue != null)
            {
                oldValue.Checked -= checkBoxOn_CheckedChanged;
                oldValue.Unchecked -= checkBoxOn_CheckedChanged;
            }

            if (newValue != null)
            {
                newValue.Checked += checkBoxOn_CheckedChanged;
                newValue.Unchecked += checkBoxOn_CheckedChanged;
            }
        }

        private void OnEnableChanged()
        {
            this.RaiseEvent(new RoutedEventArgs(EnableChangedEvent));
        }
        private void checkBoxOn_CheckedChanged(object sender, RoutedEventArgs e)
        {
            OnEnableChanged();
        }

    
    }
}
