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
using Prism.Interactivity.InteractionRequest;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// DateTimeControl.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimeControl : UserControl,Prism.Interactivity.InteractionRequest.IInteractionRequestAware
    {
        public static readonly DependencyProperty ShowFramesProperty =
            DependencyProperty.Register("ShowFrames", typeof(bool), typeof(DateTimeControl), new PropertyMetadata(true, OnShowFramesPropertyChanged));

        private static void OnShowFramesPropertyChanged(DependencyObject dbObj, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeControl)dbObj).OnShowFramesChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private void OnShowFramesChanged(bool oldValue, bool newValue)
        {
            this.timeCodeControl.ShowFrames = this.ShowFrames;
        }

        public bool ShowFrames
        {
            get { return (bool)GetValue(ShowFramesProperty); }
            set { SetValue(ShowFramesProperty, value); }
        }

        private EditDateTimeConfirmation _notification;

        public INotification Notification
        {
            get { return _notification; }
            set
            {
                _notification = (EditDateTimeConfirmation)value;
                if (_notification != null)
                {
                    this.dtPicker.Value = _notification.Time.Date;
                    this.timeCodeControl.TimeCode = _notification.Time.TimeOfDay;
                }
            }
        }

        public Action FinishInteraction
        {
            get;set;
        }

        public DateTimeControl()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (_notification != null)
            {
                _notification.Time = this.dtPicker.Value.Value.Add(this.timeCodeControl.TimeCode);
                _notification.Confirmed = true;
            }

            if (this.FinishInteraction != null)
            {
                this.FinishInteraction();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_notification != null)
            {
                _notification.Confirmed = false;
            }

            if (this.FinishInteraction != null)
            {
                this.FinishInteraction();
            }
        }
    }
}
