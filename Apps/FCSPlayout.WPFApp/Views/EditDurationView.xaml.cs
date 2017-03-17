using FCSPlayout.WPF.Core;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPFApp.Views
{
    /// <summary>
    /// EditMediaItemView.xaml 的交互逻辑
    /// </summary>
    public partial class EditDurationView : UserControl, Prism.Interactivity.InteractionRequest.IInteractionRequestAware
    {
        private EditDurationConfirmation _notification;

        public INotification Notification
        {
            get { return _notification; }
            set
            {
                _notification = value as EditDurationConfirmation;
                if (_notification != null)
                {
                    this.timeSpanUpDown.Value = _notification.Duration;
                }
            }
        }

        public Action FinishInteraction
        {
            get; set;
        }

        public EditDurationView()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var confrimation = this.Notification as EditDurationConfirmation;
            if (confrimation != null)
            {
                confrimation.Confirmed = true;
                if (this.timeSpanUpDown.Value != null)
                {
                    confrimation.Duration = this.timeSpanUpDown.Value.Value;
                }
            }

            this.FinishInteraction();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.FinishInteraction();
        }
    }
}
