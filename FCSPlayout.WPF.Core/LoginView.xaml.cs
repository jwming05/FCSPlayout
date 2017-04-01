using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// LoginView2.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl,Prism.Interactivity.InteractionRequest.IInteractionRequestAware
    {
        public LoginView()
        {
            InitializeComponent();
        }


        public string UserName
        {
            get { return this.txtUser.Text; }
        }

        public string Password
        {
            get { return pwdBox.Password; }
        }

        public INotification Notification
        {
            get;set;
        }

        public Action FinishInteraction
        {
            get;set;
        }

        public void ShowMessage(string message)
        {
            this.txtMessage.Text = message ?? string.Empty;
        }

        private void txtUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            ClearMessage();
        }

        private void pwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ClearMessage();
        }

        private void ClearMessage()
        {
            this.txtMessage.Text = string.Empty;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UserName))
            {
                ShowMessage("用户名不能为空。");
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                ShowMessage("密码不能为空。");
                return;
            }

            if (!LoginConfirmation.LoginAction(this.UserName, this.Password))
            {
                ShowMessage("登录失败，用户名或密码错误，或没有权限登录该系统。");
                return;
            }

            this.Close(true);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(false);
        }

        private LoginConfirmation LoginConfirmation
        {
            get { return (LoginConfirmation)this.Notification; }
        }        

        private void Close(bool confirmed)
        {
            this.LoginConfirmation.Confirmed = confirmed;
            this.FinishInteraction();
        }
    }
}
