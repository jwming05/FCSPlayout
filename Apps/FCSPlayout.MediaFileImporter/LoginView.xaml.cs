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

namespace FCSPlayout.MediaFileImporter
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl
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
    }
}
