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
