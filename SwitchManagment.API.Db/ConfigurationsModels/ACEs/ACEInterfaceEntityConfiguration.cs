using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Db.Entities.ACL.ACEs;

namespace SwitchManagment.API.Db.ConfigurationsModels.ACEs
{
    public class ACEInterfaceEntityConfiguration : IEntityTypeConfiguration<ACEInterfaceEntity>
    {
        public void Configure(EntityTypeBuilder<ACEInterfaceEntity> builder)
        {
            builder.HasKey(i => i.Id);

            builder
                .HasOne<InterfaceEntity>()
                .WithMany()
                .HasForeignKey(aceI => new { aceI.SwitchId, aceI.IdOnSwitch })
                .HasPrincipalKey(i => new { i.SwitchId, i.IdOnSwitch })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
