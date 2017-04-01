using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;
using System.Linq;

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
            var userEntity = PlayoutRepository.GetUser(name, PasswordProcessor.Current.Encrypt(password));
            if (userEntity == null)
            {
                return false;
            }

            var user = new User { Id = userEntity.Id, Name = userEntity.Name,Roles=userEntity.Roles.Select(r=>r.Name).ToArray() };
            if(!user.IsInRole(applicationName) && !user.IsAdmin())
            {
                return false;
            }

            _user = user;
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
