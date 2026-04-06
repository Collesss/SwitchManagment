using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Db.Entities.ACL.ACEs;

namespace SwitchManagment.API.Db.ConfigurationsModels.ACEs
{
    public class ACEVlanOnInterfaceEntityConfiguration : IEntityTypeConfiguration<ACEVlanOnInterfaceEntity>
    {
        public void Configure(EntityTypeBuilder<ACEVlanOnInterfaceEntity> builder)
        {
            builder.HasKey(aceVlOnSw => aceVlOnSw.Id);

            builder.Property(aceVlOnSw => aceVlOnSw.GroupSID)
                .IsRequired();

            builder.HasIndex(aceVlOnSw => new { aceVlOnSw.GroupSID, aceVlOnSw.SwitchId, aceVlOnSw.IdOnSwitch, aceVlOnSw.Vlan })
                .IsUnique();

            builder
                .HasOne(aceVlOnSw => aceVlOnSw.Interface)
                .WithMany(i => i.ACLVlans)
                .HasForeignKey(aceVlOnSw => new { aceVlOnSw.SwitchId, aceVlOnSw.IdOnSwitch })
                .HasPrincipalKey(i => new { i.SwitchId, i.IdOnSwitch })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
