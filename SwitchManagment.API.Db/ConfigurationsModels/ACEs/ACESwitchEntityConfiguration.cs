using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Db.Entities.ACL.ACEs;

namespace SwitchManagment.API.Db.ConfigurationsModels.ACL
{
    public class ACESwitchEntityConfiguration : IEntityTypeConfiguration<ACESwitchEntity>
    {
        public void Configure(EntityTypeBuilder<ACESwitchEntity> builder)
        {
            builder.HasKey(aceSw => aceSw.Id);

            builder.Property(aceSw => aceSw.GroupSID)
                .IsRequired();

            builder.HasIndex(aceSw => new { aceSw.GroupSID, aceSw.SwitchId })
                .IsUnique();

            builder
                .HasOne(aceSw => aceSw.Switch)
                .WithMany(sw => sw.ACLSwitch)
                .HasForeignKey(aceSw => aceSw.SwitchId)
                .HasPrincipalKey(sw => sw.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
