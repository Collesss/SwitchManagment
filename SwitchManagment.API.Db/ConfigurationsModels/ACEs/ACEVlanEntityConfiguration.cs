using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Db.Entities.ACL.ACEs;

namespace SwitchManagment.API.Db.ConfigurationsModels.ACEs
{
    public class ACEVlanEntityConfiguration : IEntityTypeConfiguration<ACEVlanEntity>
    {
        public void Configure(EntityTypeBuilder<ACEVlanEntity> builder)
        {
            builder.HasKey(aceVl => aceVl.Id);

            builder
                .HasOne<SwitchEntity>()
                .WithMany()
                .HasForeignKey(aceVl => aceVl.SwitchId)
                .HasPrincipalKey(sw => sw.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
