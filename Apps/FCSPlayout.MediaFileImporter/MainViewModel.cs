using FCSPlayout.AppInfrastructure;

using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace FCSPlayout.MediaFileImporter
{
    public class MainViewModel :ShellModelBase //  BindableBase
    {
        //private readonly DelegateCommand _collectGarbageCommand;
        //private PeriodicalGarbageCollector _garbageCollector = new PeriodicalGarbageCollector();
        //private readonly InteractionRequest<LoginConfirmation> _loginInteractionRequest;
        //private readonly DelegateCommand _loginCommand;

        public MainViewModel(IUserService userService, InteractionRequests requests)
            :base(userService)
        {
            this.OpenFileInteractionRequest = requests.OpenFileInteractionRequest;
            this.DisplayMessageInteractionRequest = requests.DisplayMessageInteractionRequest;
            this.SaveFileInteractionRequest=requests.SaveFileInteractionRequest;

            //_loginInteractionRequest = requests.LoginInteractionRequest;
            //_collectGarbageCommand = new DelegateCommand(ExecuteCollectGarbage);
            //_loginCommand = new DelegateCommand(Login);
        }

        //private void Login()
        //{
        //    this.LoginInteractionRequest.Raise(new LoginConfirmation()
        //    {
        //        Title="用户登录",
        //        LoginAction=(name,pwd)=> UserService.Login(name, pwd, App.Current.Name)
        //    }, 
        //    (c) => 
        //    {
        //        if (!c.Confirmed)
        //        {
        //            App.Current.MainWindow.Close();
        //            App.Current.Shutdown();
        //        }
        //    });
        //}

        //private void ExecuteCollectGarbage()
        //{
        //    _garbageCollector.OnTimer();
        //}

        //private void RaiseDisplayMessageInteractionRequest(string title, string message)
        //{
        //    DisplayMessageInteractionRequest.Raise(new Notification { Title = title, Content = message });
        //}


        public IInteractionRequest OpenFileInteractionRequest
        {
            get;private set;
        }

        public IInteractionRequest DisplayMessageInteractionRequest
        {
            get;private set;
        }

        public IInteractionRequest SaveFileInteractionRequest
        {
            get; private set;
        }

        //public ICommand CollectGarbageCommand
        //{
        //    get
        //    {
        //        return _collectGarbageCommand;
        //    }
        //}

        //public InteractionRequest<LoginConfirmation> LoginInteractionRequest
        //{
        //    get
        //    {
        //        return _loginInteractionRequest;
        //    }
        //}

        //public DelegateCommand LoginCommand
        //{
        //    get
        //    {
        //        return _loginCommand;
        //    }
        //}
    }
}