using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwitchManagment.API.Repository.Entities;

namespace SwitchManagment.API.Repository.SqlLite.ConfigurationsModels
{
    public class SwitchEntityConfiguration : IEntityTypeConfiguration<SwitchEntity>
    {
        public void Configure(EntityTypeBuilder<SwitchEntity> builder)
        {
            builder.HasKey(sw => sw.Id);

            builder.HasIndex(sw => sw.IpOrName)
                .IsUnique();
        }
    }
}
