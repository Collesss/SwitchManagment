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

            builder
                .HasOne<InterfaceEntity>()
                .WithMany()
                .HasForeignKey(aceVlOnSw => new { aceVlOnSw.SwitchId, aceVlOnSw.IdOnSwitch })
                .HasPrincipalKey(i => new { i.SwitchId, i.IdOnSwitch })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
