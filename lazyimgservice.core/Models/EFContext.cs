using Microsoft.EntityFrameworkCore;

namespace lazyimgservice.core.Models
{
    public class EFContext : DbContext
    {
        public EFContext()
        {
            if (!_created)
            {
                _created = true;
                //Database.EnsureDeleted();
                Database.EnsureCreated();
            }
        }
        private static bool _created = false;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=db/lazyimgservice.db");


        }


        public DbSet<ImageRecord> Images { get; set; }
        public DbSet<FileRecord> Files { get; set; }       
        public DbSet<ImageGroup> ImageGroups { get; set; }
        public DbSet<TagLink> TagLinks { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
