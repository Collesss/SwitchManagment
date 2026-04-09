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

        public DbSet<InterfaceEntity> Interfaces { get; set; }

        #region ACL
        public DbSet<ACESwitchEntity> ACLSwitches { get; set; }

        public DbSet<ACEInterfaceEntity> ACLInterfaces { get; set; }

        public DbSet<ACEVlanEntity> ACLVlans { get; set; }

        public DbSet<ACEVlanOnInterfaceEntity> ACLVlanOnInterfaces { get; set; }
        #endregion

        //public DbSet<CredentialEntity> Credentials { get; set; }

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
            /*
            IEnumerable<SwitchEntity> initSwitches = Enumerable.Range(1, 5)
                .Select(i => new SwitchEntity() 
                { 
                    Id = i, 
                    IpOrName = $"192.168.0.{i}", 
                    Location = $"Loc{i}", 
                    Description = $"Desc{i}", 
                    Login = $"Admin{i}", 
                    EncryptedPassword = $"EP{i}", 
                    EncryptedSuperPassword = $"ESP{i}" 
                });
            */
            //modelBuilder.Entity<SwitchEntity>().HasData(initSwitches);

            modelBuilder.ApplyConfiguration(new SwitchEntityConfiguration());
            modelBuilder.ApplyConfiguration(new InterfaceEntityConfiguration());

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
