using System;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.MediaFileImporter
{
    /// <summary>
    /// PlaySlider2.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewPlaySlider : UserControl
    {
        public double InPosition
        {
            get { return (double)GetValue(InPositionProperty); }
            set { SetValue(InPositionProperty, value); }
        }

        public double OutPosition
        {
            get { return (double)GetValue(OutPositionProperty); }
            set { SetValue(OutPositionProperty, value); }
        }

        public static readonly DependencyProperty OutPositionProperty =
            DependencyProperty.Register("OutPosition", typeof(double), typeof(PreviewPlaySlider), 
                new FrameworkPropertyMetadata(0.0, OnOutPositionPropertyChanged));


        public static readonly DependencyProperty InPositionProperty =
            DependencyProperty.Register("InPosition", typeof(double), typeof(PreviewPlaySlider), 
                new FrameworkPropertyMetadata(0.0, OnInPositionPropertyChanged));


        private static RoutedEvent PositionChangedEvent =
            EventManager.RegisterRoutedEvent("PositionChanged", RoutingStrategy.Direct,
                typeof(RoutedEventHandler), typeof(PreviewPlaySlider));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double), typeof(PreviewPlaySlider),
                new FrameworkPropertyMetadata(0.0, OnPositionPropertyChanged, CoercePosition));

        public static readonly DependencyProperty MaxPositionProperty =
            DependencyProperty.Register("MaxPosition", typeof(double), typeof(PreviewPlaySlider),
                new FrameworkPropertyMetadata(0.0, OnMaxPositionPropertyChanged), ValidateMaxPositionProperty);

        public static readonly DependencyProperty RaiseValueChangedPrecisionProperty =
            DependencyProperty.Register("RaiseValueChangedPrecision", typeof(double), typeof(PreviewPlaySlider),
                new FrameworkPropertyMetadata(0.0));

        private static bool ValidateMaxPositionProperty(object value)
        {
            return ((double)value) >= 0.0;
        }

        private static void OnPositionPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PreviewPlaySlider)dpObj).OnPositionChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnInPositionPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PreviewPlaySlider)dpObj).OnInPositionChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnOutPositionPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PreviewPlaySlider)dpObj).OnOutPositionChanged((double)e.OldValue, (double)e.NewValue);
        }
        private static object CoercePosition(DependencyObject dpObj, object baseValue)
        {
            var slider = (PreviewPlaySlider)dpObj;

            long frames =(long)(((double)baseValue) / 0.04);

            double value = frames * 0.04;

            return Math.Min(slider.MaxPosition, Math.Max(0.0, value/*(double)baseValue*/));
        }

        private static void OnMaxPositionPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PreviewPlaySlider)dpObj).OnMaxPositionChanged((double)e.OldValue, (double)e.NewValue);
        }

        private bool _raisePositionChangedEvent;
        private bool _updatingSliderValue;
        private bool _updateLowerValue;
        private bool _updateHigherValue;

        public PreviewPlaySlider()
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
            this.rangeSlider.Maximum = this.MaxPosition;
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

        private void OnInPositionChanged(double oldValue, double newValue)
        {
            _updateLowerValue = true;
            this.rangeSlider.LowerValue = this.InPosition;
            _updateLowerValue = false;
        }

        private void OnOutPositionChanged(double oldValue, double newValue)
        {
            _updateHigherValue = true;
            this.rangeSlider.HigherValue = this.OutPosition;
            _updateHigherValue = false;
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

        private void rangeSlider_HigherValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_updateHigherValue)
            {
                this.OutPosition = this.rangeSlider.HigherValue;
            }
        }

        private void rangeSlider_LowerValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_updateLowerValue)
            {
                this.InPosition = this.rangeSlider.LowerValue;
            }
        }
    }
}
