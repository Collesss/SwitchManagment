using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwitchManagment.API.Db.Entities;

namespace SwitchManagment.API.Db.ConfigurationsModels
{
    public class SwitchEntityConfiguration : IEntityTypeConfiguration<SwitchEntity>
    {
        public void Configure(EntityTypeBuilder<SwitchEntity> builder)
        {
            builder.HasKey(sw => sw.Id);

            builder.HasIndex(sw => sw.IpOrName)
                .IsUnique();

            builder.HasOne(sw => sw.Credential)
                .WithMany()
                .HasForeignKey(sw => sw.CredentialId)
                .HasPrincipalKey(credential => credential.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
