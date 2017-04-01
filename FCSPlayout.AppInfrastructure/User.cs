using FCSPlayout.Domain;
using System;
using System.Linq;

namespace FCSPlayout.AppInfrastructure
{
    public class User: IUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string[] Roles { get; internal set; }

        public bool IsInRole(string roleName)
        {
            return this.Roles.Any(r => string.Equals(r, roleName, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsAdmin()
        {
            return this.IsInRole(RoleNames.Admin);
        }
    }
}