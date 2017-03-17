using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// MAudioChannelWrapper.xaml 的交互逻辑
    /// </summary>
    public partial class MAudioChannel : UserControl,IMAudioChannelDrawerHost
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
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnGainPropertyChanged, CoerceGainValue));

        public static readonly DependencyProperty RiskProperty =
            DependencyProperty.Register("Risk", typeof(double), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsRender), ValidateRiskValue);

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

        public static readonly DependencyProperty ChannelEnabledProperty =
            DependencyProperty.Register("ChannelEnabled", typeof(bool), typeof(MAudioChannel),
                new FrameworkPropertyMetadata(true, OnChannelEnabledPropertyChanged));

        public static RoutedEvent ChannelEnabledChangedEvent =
            EventManager.RegisterRoutedEvent("ChannelEnabledChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MAudioChannel));


        private static void OnChannelEnabledPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((MAudioChannel)dpObj).OnChannelEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private static void OnGainPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((MAudioChannel)dpObj).OnGainChanged((double)e.OldValue, (double)e.NewValue);
        }

        #endregion






        private MAudioChannelDrawer _audioChannelDrawer;

        public MAudioChannel()
        {
            InitializeComponent();

            _audioChannelDrawer = new MAudioChannelDrawer(this);
            _audioChannelDrawer.MinHeight = 10;
            _audioChannelDrawer.MinWidth = 15;
            _audioChannelDrawer.HorizontalAlignment = HorizontalAlignment.Center;
            grid.Children.Add(_audioChannelDrawer);

            Grid.SetRow(_audioChannelDrawer, 0);
        }

        public bool ChannelEnabled
        {
            get { return (bool)GetValue(ChannelEnabledProperty); }
            set { SetValue(ChannelEnabledProperty, value); }
        }

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

        public event RoutedEventHandler ChannelEnabledChanged
        {
            add { this.AddHandler(ChannelEnabledChangedEvent, value); }
            remove { this.RemoveHandler(ChannelEnabledChangedEvent, value); }
        }

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

        private void OnChannelEnabledChanged(bool oldValue, bool newValue)
        {
            this.RaiseEvent(new RoutedEventArgs(ChannelEnabledChangedEvent));
        }

        private void OnGainChanged(double oldValue, double newValue)
        {
            this.RaiseEvent(new RoutedEventArgs(GainChangedEvent));
        }

        internal void Refresh()
        {
            _audioChannelDrawer.InvalidateVisual();
        }
    }
}
