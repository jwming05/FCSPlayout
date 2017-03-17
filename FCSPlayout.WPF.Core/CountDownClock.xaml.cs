using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// DigitalClock.xaml 的交互逻辑
    /// </summary>
    public partial class CountDownClock : UserControl
    {
        public static readonly DependencyProperty TickCommandProperty =
            DependencyProperty.Register("TickCommand", typeof(ICommand), typeof(CountDownClock));

        public DateTime StopTime
        {
            get { return (DateTime)GetValue(StopTimeProperty); }
            set { SetValue(StopTimeProperty, value); }
        }

        public static readonly DependencyProperty StopTimeProperty =
            DependencyProperty.Register("StopTime", typeof(DateTime), typeof(CountDownClock), 
                new FrameworkPropertyMetadata(DateTime.Now));


        private DateTime _date;
        private DateTime _dateTime;
        private DispatcherTimer _timer;

        private static Dictionary<DayOfWeek, string> WeekDayNames = new Dictionary<DayOfWeek, string>
        {
            { DayOfWeek.Sunday,    "星期日" },
            { DayOfWeek.Saturday,  "星期六" },
            { DayOfWeek.Friday,    "星期五" },
            { DayOfWeek.Thursday,  "星期四" },
            { DayOfWeek.Wednesday, "星期三" },
            { DayOfWeek.Tuesday,   "星期二" },
            { DayOfWeek.Monday,    "星期一" }
        };
        public CountDownClock()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += (s, e) => { UpdateDateTime(); };
            this.Loaded += (s, e) => { _timer.Start(); };
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            var now = DateTime.Now;
            if (TickCommand != null && TickCommand.CanExecute(now))
            {
                TickCommand.Execute(now);
            }

            var oldDateTime = _dateTime;
            _dateTime = now;

            if (oldDateTime.Hour != _dateTime.Hour ||
                oldDateTime.Minute != _dateTime.Minute ||
                oldDateTime.Second != _dateTime.Second)
            {
                this.txtTime.Text = _dateTime.ToString(@"HH\:mm\:ss");
            }

            var remain = DateTime.Now.Subtract(StopTime);
            if (remain < TimeSpan.Zero)
            {
                remain = TimeSpan.Zero;
            }
        }

        public ICommand TickCommand
        {
            get { return (ICommand)GetValue(TickCommandProperty); }
            set { SetValue(TickCommandProperty, value); }
        }
    }
}
