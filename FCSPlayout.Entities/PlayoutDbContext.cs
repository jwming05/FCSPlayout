using System.Data.Entity;

namespace FCSPlayout.Entities
{
    public class PlayoutDbContext:DbContext
    {
        public DbSet<MediaFileChannel> MediaFileChannels { get; set; }
        public DbSet<MediaFileCategory> MediaFileCategories { get; set; }
    }
}
