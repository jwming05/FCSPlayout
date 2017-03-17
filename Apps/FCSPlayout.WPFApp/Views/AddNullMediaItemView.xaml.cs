using FCSPlayout.WPF.Core;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPFApp.Views
{
    /// <summary>
    /// AddNullMediaItemView.xaml 的交互逻辑
    /// </summary>
    public partial class AddNullMediaItemView : UserControl,Prism.Interactivity.InteractionRequest.IInteractionRequestAware
    {
        public AddNullMediaItemView()
        {
            InitializeComponent();
        }

        public Action FinishInteraction
        {
            get;set;
        }

        public INotification Notification
        {
            get;set;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.FinishInteraction != null)
            {
                this.FinishInteraction();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var confrimation = this.Notification as AddNullMediaItemConfirmation;
            if (confrimation != null)
            {
                confrimation.Confirmed = true;
            }

            if (this.FinishInteraction != null)
            {
                this.FinishInteraction();
            }
        }
    }
}
