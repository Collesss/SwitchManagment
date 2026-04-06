using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwitchManagment.API.Db.Entities.ACL.ACEs;

namespace SwitchManagment.API.Db.ConfigurationsModels.ACEs
{
    public class ACEInterfaceEntityConfiguration : IEntityTypeConfiguration<ACEInterfaceEntity>
    {
        public void Configure(EntityTypeBuilder<ACEInterfaceEntity> builder)
        {
            builder.HasKey(aceI => aceI.Id);

            builder.Property(aceI => aceI.GroupSID)
                .IsRequired();

            builder.HasIndex(aceI => new { aceI.GroupSID, aceI.SwitchId, aceI.IdOnSwitch })
                .IsUnique();

            builder
                .HasOne(aceI => aceI.Interface)
                .WithMany(i => i.ACLInterface)
                .HasForeignKey(aceI => new { aceI.SwitchId, aceI.IdOnSwitch })
                .HasPrincipalKey(i => new { i.SwitchId, i.IdOnSwitch })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
