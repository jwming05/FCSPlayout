using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;

namespace FCSPlayout.AppInfrastructure
{
    public class UserService
    {
        private static User _user;

        public static User CurrentUser
        {
            get
            {
                return _user;
            }
        }

        public static bool Login(string name, string password, string applicationName)
        {
            if (_user != null)
            {
                throw new InvalidOperationException();
            }
            var u = PlayoutRepository.GetUser(name, PasswordProcessor.Current.Encrypt(password));
            if (u == null)
            {
                return false;
            }

            _user = new User { Id = u.Id, Name = u.Name };

            var action = new UserAction
            {
                Category = UserActionCategory.Login,
                ApplicationName = applicationName,
                Name = "登录",
                Description = string.Format("登录: {0}", applicationName),
            };

            UserService.AddAction(action);

            return true;
        }

        public static void AddAction(UserAction action)
        {
            action.UserName = CurrentUser.Name;
            action.UserId = CurrentUser.Id;

            PlayoutRepository.AddAction(action);
        }
    }
}
