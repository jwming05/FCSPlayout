using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace FCSPlayout.Entities
{
    public class PlayoutDbContext:DbContext
    {
        internal const string DefaultName = "__Default__";

        static PlayoutDbContext()
        {
            Database.SetInitializer(new PlayoutDatabaseInitializer());
        }

        public DbSet<MediaFileChannel> MediaFileChannels { get; set; }
        public DbSet<MediaFileCategory> MediaFileCategories { get; set; }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<MediaFileEntity> MediaFiles { get; set; }

        public DbSet<UserAction> UserActions { get; set; }
        public DbSet<ApplicationInfo> Applications { get; set; }
        public DbSet<MachineInfo> Machines { get; set; }
        public DbSet<SettingGroup> SettingGroups { get; set; }
        public DbSet<SettingInfo> Settings { get; set; }
        public DbSet<SettingScope> SettingScopes { get; set; }
        public DbSet<ChannelInfo> ChannelInfos { get; set; }

        public DbSet<PlaybillEntity> Playbills { get; set; }
        public DbSet<PlayItemEntity> PlayItems { get; set; }
        public DbSet<PlaybillItemEntity> PlaybillItems { get; set; }
        public DbSet<BMDSwitcherInfo> BMDSwitchers { get; set; }

        public DbSet<BMDSwitcherInputInfo> BMDSwitcherInputs { get; set; }

        public DbSet<PlayRecord> PlayRecords { get; set; }

        public override int SaveChanges()
        {
            foreach (DbEntityEntry<IGuidIdentifier> entry in this.ChangeTracker.Entries<IGuidIdentifier>())
            {
                if (entry.Entity.Id == Guid.Empty)
                {
                    entry.Entity.Id = Guid.NewGuid();
                }
            }

            DateTime now = DateTime.Now;

            foreach (DbEntityEntry<ICreationTimestamp> entry in this.ChangeTracker.Entries<ICreationTimestamp>())
            {
                if (entry.Entity.CreationTime == EmptyDateTime)
                {
                    entry.Entity.CreationTime = now;

                }
            }

            foreach (DbEntityEntry<IModificationTimestamp> entry in this.ChangeTracker.Entries<IModificationTimestamp>())
            {
                entry.Entity.ModificationTime = now;
            }

            return base.SaveChanges();
        }

        private static DateTime EmptyDateTime = new DateTime();
    }
}
