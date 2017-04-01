using System;

namespace FCSPlayout.Domain
{
    public interface IUser
    {
        Guid Id { get; }
        string Name { get; }
        string[] Roles { get; }

        bool IsInRole(string roleName);

        //public bool IsAdmin()
        //{
        //    return this.IsInRole(RoleNames.Admin);
        //}
    }

    public static class UserExtensions
    {
        public static bool IsAdmin(this IUser user)
        {
            return user.IsInRole(RoleNames.Admin);
        }
    }
}
