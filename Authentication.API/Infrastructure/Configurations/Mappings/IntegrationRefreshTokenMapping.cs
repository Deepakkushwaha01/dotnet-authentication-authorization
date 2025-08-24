using Authentication.API.Infrastructure.Configurations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.API.Infrastructure.Configurations.Mappings
{
    public class IntegrationRefreshTokenMapping : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable(nameof(RefreshToken), "Integration");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Uid).HasColumnName("Uid").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("CreatedOn").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.Expires).HasColumnName("Expires").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.DeletedOn).HasColumnName("DeletedOn").HasColumnType("datetime").HasDefaultValue(null);
            builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").HasColumnType("nvarchar").HasMaxLength(255).HasDefaultValue(null);
            builder.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy").HasColumnType("nvarchar").HasMaxLength(255).HasDefaultValue(null);
            builder.Property(x => x.Token).HasColumnName("Token").HasColumnType("nvarchar").HasMaxLength(255).IsRequired();
            builder.Property(x => x.UserId).HasColumnName("UserId").HasColumnType("nvarchar").IsRequired();

            builder.HasIndex(x => x.Uid).IsUnique();
            builder.HasIndex(x => x.Token).IsUnique();

        }
    }
}