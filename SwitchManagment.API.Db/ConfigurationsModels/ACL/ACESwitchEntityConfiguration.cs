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

            builder
                .HasOne<SwitchEntity>()
                .WithMany()
                .HasForeignKey(aceSw => aceSw.SwitchId)
                .HasPrincipalKey(sw => sw.Id)
                .OnDelete(DeleteBehavior.Cascade);

            /*
            builder.HasOne(sw => sw.Credential)
                .WithMany()
                .HasForeignKey(sw => sw.CredentialId)
                .HasPrincipalKey(credential => credential.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            */
        }
    }
}
