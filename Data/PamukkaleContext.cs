using Microsoft.EntityFrameworkCore;
using Swilago.Data.Procedures;
using Swilago.Data.Tables;

namespace Swilago.Data
{
    public class PamukkaleContext : DbContext
    {
        public PamukkaleContext(DbContextOptions<PamukkaleContext> options) : base(options)
        {
            Database.SetCommandTimeout(3600);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TRestaurant>()
                .HasKey(k => new { k.ResName });

            modelBuilder.Entity<TUserRecord>()
                .HasKey(k => new { k.ModifiedDate, k.UserEmail });
        }

        public DbSet<TRestaurant> Restaurant { get; set; }

        public DbSet<TUserRecord> UserRecord { get; set; }

        public DbSet<PAllStatistics> AllStatistics { get; set; }

        public DbSet<PDateStatistics> DateStatistics { get; set; }

        public DbSet<PPublicApi> PublicApi { get; set; }
    }
}
