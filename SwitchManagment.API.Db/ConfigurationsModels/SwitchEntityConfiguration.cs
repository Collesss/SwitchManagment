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
        }
    }
}
