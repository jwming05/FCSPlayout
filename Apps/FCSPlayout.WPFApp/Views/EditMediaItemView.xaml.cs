using FCSPlayout.Entities;
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
    public partial class EditMediaItemView : UserControl, Prism.Interactivity.InteractionRequest.IInteractionRequestAware
    {
        private EditMediaItemConfirmation _notification;

        public INotification Notification
        {
            get { return _notification; }
            set
            {
                _notification = value as EditMediaItemConfirmation;
                if (_notification != null)
                {

                    this.playerControl.Init(_notification.FilePath,
                        _notification.PlayRange,PlayoutRepository.GetMPlaylistSettings());
                }
            }
        }

        private string GetAudioFormat()
        {
            throw new NotImplementedException();
        }

        private string GetVideoFormat()
        {
            throw new NotImplementedException();
        }

        public Action FinishInteraction
        {
            get; set;
        }

        public EditMediaItemView()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var confrimation = this.Notification as EditMediaItemConfirmation;
            if (confrimation != null)
            {
                confrimation.Confirmed = true;
                confrimation.PlayRange = this.playerControl.PlayRange;
            }

            this.FinishInteraction();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.FinishInteraction();
        }
    }
}
