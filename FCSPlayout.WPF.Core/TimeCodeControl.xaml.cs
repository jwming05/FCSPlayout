using FCSPlayout.Domain;
using System;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// TimeCodeControl.xaml 的交互逻辑
    /// </summary>
    public partial class TimeCodeControl : UserControl
    {
        public static readonly DependencyProperty TimeCodeProperty =
            DependencyProperty.Register("TimeCode", typeof(TimeSpan), typeof(TimeCodeControl), new PropertyMetadata(TimeSpan.Zero, OnTimeCodePropertyChanged));

        private static void OnTimeCodePropertyChanged(DependencyObject dbObj, DependencyPropertyChangedEventArgs e)
        {
            ((TimeCodeControl)dbObj).OnTimeCodeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        public static readonly DependencyProperty ShowFramesProperty =
            DependencyProperty.Register("ShowFrames", typeof(bool), typeof(TimeCodeControl), new PropertyMetadata(true, OnShowFramesPropertyChanged));

        private static void OnShowFramesPropertyChanged(DependencyObject dbObj, DependencyPropertyChangedEventArgs e)
        {
            ((TimeCodeControl)dbObj).OnShowFramesChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        

        private Action _onValueChangedAction;
        private Action _onTimeCodeChangedAction;

        public TimeCodeControl()
        {
            InitializeComponent();

            _onTimeCodeChangedAction = OnTimeCodeChangedAction;
            _onValueChangedAction = OnValueChangedAction;

            this.iudHour.Value = 0;
            this.iudMinute.Value = 0;
            this.iudSecond.Value = 0;
            this.iudFrame.Value = 0;

            this.stackPanel.AddHandler(IntegerUpDown.ValueChangedEvent, new RoutedEventHandler(IntegerUpDown_ValueChanged));
        }

        public TimeSpan TimeCode
        {
            get { return (TimeSpan)GetValue(TimeCodeProperty); }
            set { SetValue(TimeCodeProperty, value); }
        }

        public bool ShowFrames
        {
            get { return (bool)GetValue(ShowFramesProperty); }
            set { SetValue(ShowFramesProperty, value); }
        }

        private void IntegerUpDown_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (_onValueChangedAction != null)
            {
                _onValueChangedAction();
            }
        }

        private void OnTimeCodeChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            if (_onTimeCodeChangedAction != null)
            {
                _onTimeCodeChangedAction();
            }
        }

        private void OnShowFramesChanged(bool oldValue, bool newValue)
        {
            if (this.ShowFrames)
            {
                this.iudFrame.Visibility = Visibility.Visible;
                this.frameLabel.Visibility = Visibility.Visible;
            }
            else
            {
                this.iudFrame.Visibility = Visibility.Collapsed;
                this.frameLabel.Visibility = Visibility.Collapsed;
            }
        }

        private void OnValueChangedAction()
        {
            if (iudHour != null && iudMinute != null && iudSecond != null && iudFrame != null)
            {
                _onTimeCodeChangedAction = null;
                this.TimeCode = new TimeSpan(0, iudHour.Value ?? 0, iudMinute.Value ?? 0, iudSecond.Value ?? 0, TimeCodeUtils.ToMilliseconds(iudFrame.Value ?? 0));
                _onTimeCodeChangedAction = OnTimeCodeChangedAction;
            }
        }
        private void OnTimeCodeChangedAction()
        {
            if (iudHour != null && iudMinute != null && iudSecond != null && iudFrame != null)
            {
                _onValueChangedAction = null;
                this.iudHour.Value = this.TimeCode.Hours;
                this.iudMinute.Value = this.TimeCode.Minutes;
                this.iudSecond.Value = this.TimeCode.Seconds;
                this.iudFrame.Value = TimeCodeUtils.ToFrames(this.TimeCode.Milliseconds);
                _onValueChangedAction = OnValueChangedAction;
            }
        }
    }
}
