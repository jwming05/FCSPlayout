﻿//using FCSPlayout.WPF.Core;
using FCSPlayout.Entities;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPF.Core
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
                    //this.playerControl.Init(((FileMediaSource)_notification.Source).Path,
                    //    _notification.PlayRange);
                    //var src = (FileMediaSource)_notification.Source;
                    this.playerControl.Init(_notification.FilePath, _notification.PlayRange,PlayoutRepository.GetMPlaylistSettings());
                }
            }
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
