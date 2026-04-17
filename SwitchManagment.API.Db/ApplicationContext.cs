using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Db.ConfigurationsModels;
using SwitchManagment.API.Db.ConfigurationsModels.ACEs;
using SwitchManagment.API.Db.ConfigurationsModels.ACL;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Db.Entities.ACL.ACEs;

namespace SwitchManagment.API.Db
{
    public class ApplicationContext : DbContext
    {
        public DbSet<SwitchEntity> Switches { get; set; }

        #region ACL
        public DbSet<ACESwitchEntity> ACLSwitches { get; set; }

        public DbSet<ACEInterfaceEntity> ACLInterfaces { get; set; }

        public DbSet<ACEVlanEntity> ACLVlans { get; set; }

        public DbSet<ACEVlanOnInterfaceEntity> ACLVlanOnInterfaces { get; set; }
        #endregion


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
            modelBuilder.ApplyConfiguration(new SwitchEntityConfiguration());

            #region ACL
            modelBuilder.ApplyConfiguration(new ACESwitchEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ACEInterfaceEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ACEVlanEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ACEVlanOnInterfaceEntityConfiguration());
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
