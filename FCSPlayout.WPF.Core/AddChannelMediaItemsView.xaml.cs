using FCSPlayout.WPF.Core;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// AddNullMediaItemView.xaml 的交互逻辑
    /// </summary>
    public partial class AddChannelMediaItemsView : UserControl,Prism.Interactivity.InteractionRequest.IInteractionRequestAware
    {
        public AddChannelMediaItemsView()
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
            var confrimation = this.Notification as AddChannelMediaItemsConfirmation;
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
