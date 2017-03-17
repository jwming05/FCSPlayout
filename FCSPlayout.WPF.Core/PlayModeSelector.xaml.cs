using FCSPlayout.Domain;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// PlayModeSelector.xaml 的交互逻辑
    /// </summary>
    public partial class PlayModeSelector : UserControl
    {
        public static readonly DependencyProperty PlayScheduleInfoProperty =
            DependencyProperty.Register("PlayScheduleInfo", typeof(PlayScheduleInfo), typeof(PlayModeSelector),
                new FrameworkPropertyMetadata(null, OnPlayModePropertyChanged));

        private static void OnPlayModePropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlayModeSelector)dpObj).OnPlayModeChanged((PlayScheduleInfo)e.NewValue, (PlayScheduleInfo)e.OldValue);
        }

        private TimeSpan _time;
        private DateTime _date;
        private DateTime _startTime;
        private PlayScheduleMode _playScheduleMode;

        public PlayModeSelector()
        {
            InitializeComponent();
            this.dtPicker.Value = DateTime.Now.Date;
        }

        public PlayScheduleInfo PlayScheduleInfo
        {
            get { return (PlayScheduleInfo)this.GetValue(PlayScheduleInfoProperty); }
            set { this.SetValue(PlayScheduleInfoProperty, value); }
        }

        private PlayScheduleMode PlayScheduleMode
        {
            get
            {
                return _playScheduleMode;
            }
            set
            {
                if (_playScheduleMode != value || this.PlayScheduleInfo == null)
                {
                    _playScheduleMode = value;
                    switch (_playScheduleMode)
                    {
                        case PlayScheduleMode.Auto:
                            this.PlayScheduleInfo = PlayScheduleInfo.Ordered();
                            break;
                        case PlayScheduleMode.Timing:
                            this.PlayScheduleInfo = PlayScheduleInfo.Timing(this.StartTime);
                            break;
                        case PlayScheduleMode.TimingBreak:
                            this.PlayScheduleInfo = PlayScheduleInfo.TimingBreak(this.StartTime);
                            break;
                    }

                    if (this.panelDateTime != null)
                    {
                        this.panelDateTime.IsEnabled = _playScheduleMode != PlayScheduleMode.Auto;
                    }

                }
            }
        }

        private DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    if (this.PlayScheduleInfo != null && this.PlayScheduleInfo.Mode != PlayScheduleMode.Auto)
                    {
                        this.PlayScheduleInfo.StartTime = _startTime;
                    }
                }
            }
        }

        private DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    UpdateStartTime();
                }
            }
        }

        public TimeSpan Time
        {
            get { return _time; }
            set
            {
                if (_time != value)
                {
                    _time = value;
                    UpdateStartTime();
                }
            }
        }

        private void UpdateStartTime()
        {
            this.StartTime = this.Date.Add(this.Time);
        }

        private void OnPlayModeChanged(PlayScheduleInfo newValue, PlayScheduleInfo oldValue)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.PlayScheduleMode = (PlayScheduleMode)((RadioButton)sender).Tag;
        }

        private void dtPicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (dtPicker.Value != null)
            {
                this.Date = dtPicker.Value.Value.Date;
            }
        }
    }

    public static class PlayModeCategoryDescription
    {
        public static PlayScheduleMode Timing
        {
            get { return PlayScheduleMode.Timing; }
        }

        public static PlayScheduleMode TimingBreak
        {
            get { return PlayScheduleMode.TimingBreak; }
        }

        public static PlayScheduleMode Auto
        {
            get { return PlayScheduleMode.Auto; }
        }
    }
}
