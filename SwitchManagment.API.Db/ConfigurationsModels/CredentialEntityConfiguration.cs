using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwitchManagment.API.Db.Entities;

namespace SwitchManagment.API.Db.ConfigurationsModels
{
    internal class CredentialEntityConfiguration : IEntityTypeConfiguration<CredentialEntity>
    {
        public void Configure(EntityTypeBuilder<CredentialEntity> builder)
        {
            builder.HasKey(credential => credential.Id);

            builder.HasIndex(credential => credential.Name).IsUnique();
            builder.Property(credential => credential.Login).IsRequired();
            builder.Property(credential => credential.Password).IsRequired();
            //builder.Property(credential => credential.SuperPassword).IsRequired();
        }
    }
}
