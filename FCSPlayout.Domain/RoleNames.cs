using System.Collections.Generic;

namespace FCSPlayout.Domain
{
    public class RoleNames : ApplicationNames
    {
        public const string Admin = "Admin";

        public static string[] AllRoleNames;

        static RoleNames()
        {
            AllRoleNames = new string[AllApplicationNames.Length + 1];
            System.Array.Copy(AllApplicationNames, AllRoleNames, AllApplicationNames.Length);
            AllRoleNames[AllRoleNames.Length - 1] = Admin;
        }
    }
}
