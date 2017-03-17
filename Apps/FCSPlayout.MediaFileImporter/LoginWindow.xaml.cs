using FCSPlayout.AppInfrastructure;
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
using System.Windows.Shapes;

namespace FCSPlayout.MediaFileImporter
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string UserName { get { return this.loginView.UserName; } }

        public string Password { get { return this.loginView.Password; } }

        public string ApplicationName { get; set; }

        public LoginWindow()
        {
            InitializeComponent();

            //this.Loaded += LoginWindow_Loaded;
        }

        

        public LoginWindow(string applicationName):this()
        {
            this.ApplicationName = applicationName;
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

            if (!UserService.Login(this.UserName, this.Password,this.ApplicationName))
            {
                ShowMessage("用户名或密码错误。");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        

        private void ShowMessage(string message)
        {
            this.loginView.ShowMessage(message);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
