using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Db.ConfigurationsModels;
using SwitchManagment.API.Db.Entities;

namespace SwitchManagment.API.Db
{
    public class ApplicationContext : DbContext
    {
        public DbSet<SwitchEntity> Switches { get; set; }

        public DbSet<CredentialEntity> Credentials { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IEnumerable<SwitchEntity> initSwitches = Enumerable.Range(1, 5)
                .Select(i => new SwitchEntity() { Id = i, IpOrName = $"192.168.0.{i}", Location = $"Loc{i}", Description = $"Desc{i}" });

            modelBuilder.ApplyConfiguration(new SwitchEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CredentialEntityConfiguration());

            modelBuilder.Entity<SwitchEntity>().HasData(initSwitches);

            base.OnModelCreating(modelBuilder);
        }
    }
}
