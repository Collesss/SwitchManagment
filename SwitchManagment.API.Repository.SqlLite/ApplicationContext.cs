using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Repository.Entities;
using SwitchManagment.API.Repository.SqlLite.ConfigurationsModels;

namespace SwitchManagment.API.Repository.SqlLite
{
    public class ApplicationContext : DbContext
    {
        public DbSet<SwitchEntity> Switches { get; set; }

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

            modelBuilder.Entity<SwitchEntity>().HasData(initSwitches);

            base.OnModelCreating(modelBuilder);
        }
    }
}
