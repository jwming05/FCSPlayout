using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FCSPlayout.AppInfrastructure
{
    public class UserServiceAdapter : IUserService
    {
        public IUser CurrentUser
        {
            get
            {
                return UserService.CurrentUser;
            }
        }

        public bool Login(string name, string password)
        {
            return UserService.Login(name, password, ((WPFApplicationBase)Application.Current).Name);
        }
    }
}
