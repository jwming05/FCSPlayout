using System.Data.Entity;

namespace FCSPlayout.Entities
{
    public class PlayoutDatabaseInitializer : DropCreateDatabaseIfModelChanges<PlayoutDbContext>
    {
        protected override void Seed(PlayoutDbContext context)
        {
            base.Seed(context);
            var nullApp = new ApplicationInfo { Name = PlayoutDbContext.DefaultName };
            context.Applications.Add(nullApp);

            var nullMachine = new MachineInfo { Name = PlayoutDbContext.DefaultName };
            context.Machines.Add(nullMachine);

            context.SettingGroups.Add(new SettingGroup { Name = PlayoutDbContext.DefaultName });
            context.SettingScopes.Add(new SettingScope { ApplicationName = PlayoutDbContext.DefaultName, MachineName = PlayoutDbContext.DefaultName });

            //context.ChannelInfos.Add(new ChannelInfo { Title = PlayoutDbContext.DefaultName, Special = true });

            context.Users.Add(new UserEntity { Name = "admin", Password = "admin", Locked = false });
        }
    }
}