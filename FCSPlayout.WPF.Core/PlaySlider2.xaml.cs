using System;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// PlaySlider2.xaml 的交互逻辑
    /// </summary>
    public partial class PlaySlider2 : UserControl
    {
        private static RoutedEvent PositionChangedEvent =
            EventManager.RegisterRoutedEvent("PositionChanged", RoutingStrategy.Direct,
                typeof(RoutedEventHandler), typeof(PlaySlider2));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double), typeof(PlaySlider2),
                new FrameworkPropertyMetadata(0.0, OnPositionPropertyChanged, CoercePosition));

        public static readonly DependencyProperty MaxPositionProperty =
            DependencyProperty.Register("MaxPosition", typeof(double), typeof(PlaySlider2),
                new FrameworkPropertyMetadata(0.0, OnMaxPositionPropertyChanged), ValidateMaxPositionProperty);

        public static readonly DependencyProperty RaiseValueChangedPrecisionProperty =
            DependencyProperty.Register("RaiseValueChangedPrecision", typeof(double), typeof(PlaySlider2),
                new FrameworkPropertyMetadata(0.0));

        private static bool ValidateMaxPositionProperty(object value)
        {
            return ((double)value) >= 0.0;
        }

        private static void OnPositionPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlaySlider2)dpObj).OnPositionChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static object CoercePosition(DependencyObject dpObj, object baseValue)
        {
            var slider = (PlaySlider2)dpObj;

            long frames =(long)(((double)baseValue) / 0.04);

            double value = frames * 0.04;

            return Math.Min(slider.MaxPosition, Math.Max(0.0, value/*(double)baseValue*/));
        }

        private static void OnMaxPositionPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlaySlider2)dpObj).OnMaxPositionChanged((double)e.OldValue, (double)e.NewValue);
        }

        private bool _raisePositionChangedEvent;
        private bool _updatingSliderValue;

        public PlaySlider2()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler PositionChanged
        {
            add { this.AddHandler(PositionChangedEvent, value); }
            remove { this.RemoveHandler(PositionChangedEvent, value); }
        }

        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public double MaxPosition
        {
            get { return (double)GetValue(MaxPositionProperty); }
            set { SetValue(MaxPositionProperty, value); }
        }

        public double RaiseValueChangedPrecision
        {
            get { return (double)GetValue(RaiseValueChangedPrecisionProperty); }
            set { SetValue(RaiseValueChangedPrecisionProperty, value); }
        }

        private void OnMaxPositionChanged(double oldValue, double newValue)
        {

            //var pos = this.Position;
            //pos = Math.Min(this.MaxPosition, Math.Max(0, pos));

            if (0.0 != this.Position)
            {
                this.Position = 0.0; // 相当于外部更改Position
            }

            this.slider.Maximum = this.MaxPosition;
        }

        private void OnPositionChanged(double oldValue, double newValue)
        {
            if (_raisePositionChangedEvent)
            {
                //case 1:来自用户拖动
                if (this.RaiseValueChangedPrecision <= 0.0 ||
                    System.Math.Abs(newValue - oldValue) >= this.RaiseValueChangedPrecision)
                {
                    this.RaiseEvent(new RoutedEventArgs(PositionChangedEvent));
                }          
            }
            else
            {
                //case 2:来自Position绑定源
                _updatingSliderValue = true;
                this.slider.Value = this.Position;
                _updatingSliderValue = false;
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_updatingSliderValue)
            {
                // 来自外部（用户拖动）
                // 更新Position属性。
                _raisePositionChangedEvent = true;
                this.Position = slider.Value;
                _raisePositionChangedEvent = false;
            }
            else
            {
                // 来自内部（Position绑定源变化时设置Value属性）
            }
        }
    }
}
