using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK_RefreshToken");

        entity.ToTable("RefreshTokens");

        entity.HasIndex(e => e.Token).IsUnique();
        entity.HasIndex(e => e.UserId);

        entity.Property(e => e.Id)
            .HasColumnName("Id");

        entity.Property(e => e.UserId)
            .IsRequired()
            .HasColumnName("UserId");

        entity.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("Token");

        entity.Property(e => e.ExpiresAt)
            .IsRequired()
            .HasColumnName("ExpiresAt");

        entity.Property(e => e.IsRevoked)
            .IsRequired()
            .HasColumnName("IsRevoked")
            .HasDefaultValue(false);

        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt");

        entity.Property(e => e.RevokedAt)
            .HasColumnName("RevokedAt");

        entity.HasOne(e => e.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

