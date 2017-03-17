using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// DigitalClock.xaml 的交互逻辑
    /// </summary>
    public partial class DigitalClock : UserControl
    {
        

        // Using a DependencyProperty as the backing store for TickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickCommandProperty =
            DependencyProperty.Register("TickCommand", typeof(ICommand), typeof(DigitalClock));


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
        public DigitalClock()
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

            var date = _dateTime.Date;
            if (date != _date)
            {
                _date = date;
                this.txtDate.Text = string.Format("{0:yyyy年MM月dd日} {1}", _date, WeekDayNames[_date.DayOfWeek]);
            }
        }

        public ICommand TickCommand
        {
            get { return (ICommand)GetValue(TickCommandProperty); }
            set { SetValue(TickCommandProperty, value); }
        }
    }
}
