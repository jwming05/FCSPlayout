using FCSPlayout.Domain;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FCSPlayout.WPF.Core
{
    public abstract class ShellModelBase:BindableBase
    {
        private readonly DelegateCommand _loginCommand;
        private readonly InteractionRequest<LoginConfirmation> _loginInteractionRequest;
        private readonly IUserService _userService;

        protected ShellModelBase(IUserService userService)
        {
            _userService = userService;
            _loginCommand = new DelegateCommand(Login);
            _loginInteractionRequest = new InteractionRequest<LoginConfirmation>();
        }

        private void Login()
        {
            this.LoginInteractionRequest.Raise(new LoginConfirmation()
            {
                Title = "用户登录",
                LoginAction = (name, pwd) =>_userService.Login(name,pwd)/*UserService.Login(name, pwd, App.Current.Name)*/
            },
            (c) =>
            {
                if (!c.Confirmed)
                {
                    Application.Current.MainWindow.Close();
                    Application.Current.Shutdown();
                }
                else
                {
                    this.OnLogin();
                }
            });
        }

        protected virtual void OnLogin()
        {

        }

        public InteractionRequest<LoginConfirmation> LoginInteractionRequest
        {
            get
            {
                return _loginInteractionRequest;
            }
        }

        public DelegateCommand LoginCommand
        {
            get
            {
                return _loginCommand;
            }
        }
    }
}
