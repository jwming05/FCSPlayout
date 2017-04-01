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
    public class MainViewModel : BindableBase
    {
        private readonly InteractionRequest<OpenFileDialogConfirmation> _openFileInteractionRequest;
        private readonly InteractionRequest<Notification> _displayMessageInteractionRequest;

        //private readonly DelegateCommand _collectGarbageCommand;
        //private PeriodicalGarbageCollector _garbageCollector = new PeriodicalGarbageCollector();

        private readonly InteractionRequest<LoginConfirmation> _loginInteractionRequest;

        private readonly DelegateCommand _loginCommand;


        public MainViewModel(InteractionRequests requests)
        {

            _openFileInteractionRequest = requests.OpenFileInteractionRequest;
            _displayMessageInteractionRequest = requests.DisplayMessageInteractionRequest;

            _loginInteractionRequest = requests.LoginInteractionRequest;

            //_collectGarbageCommand = new DelegateCommand(ExecuteCollectGarbage);
            _loginCommand = new DelegateCommand(Login);
        }

        private void Login()
        {
            this.LoginInteractionRequest.Raise(new LoginConfirmation()
            {
                Title="用户登录",
                LoginAction=(name,pwd)=> UserService.Login(name, pwd, App.Current.Name)
            }, 
            (c) => 
            {
                if (!c.Confirmed)
                {
                    App.Current.MainWindow.Close();
                    App.Current.Shutdown();
                }
            });
        }

        //private void ExecuteCollectGarbage()
        //{
        //    _garbageCollector.OnTimer();
        //}

        private void RaiseDisplayMessageInteractionRequest(string title, string message)
        {
            _displayMessageInteractionRequest.Raise(new Notification { Title = title, Content = message });
        }


        public IInteractionRequest OpenFileInteractionRequest
        {
            get
            {
                return _openFileInteractionRequest;
            }
        }

        public IInteractionRequest DisplayMessageInteractionRequest
        {
            get
            {
                return _displayMessageInteractionRequest;
            }
        }

        //public ICommand CollectGarbageCommand
        //{
        //    get
        //    {
        //        return _collectGarbageCommand;
        //    }
        //}

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