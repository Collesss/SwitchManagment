using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwitchManagment.API.Db.Entities;

namespace SwitchManagment.API.Db.ConfigurationsModels
{
    public class InterfaceEntityConfiguration : IEntityTypeConfiguration<InterfaceEntity>
    {
        public void Configure(EntityTypeBuilder<InterfaceEntity> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasAlternateKey(i => new { i.SwitchId , i.IdOnSwitch });

            builder
                .HasOne<SwitchEntity>()
                .WithMany()
                .HasForeignKey(i => i.SwitchId)
                .HasPrincipalKey(sw => sw.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
