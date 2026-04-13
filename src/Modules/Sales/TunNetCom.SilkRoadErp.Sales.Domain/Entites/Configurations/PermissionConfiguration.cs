using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK_Permission");

        entity.ToTable("Permissions");

        entity.HasIndex(e => e.Name).IsUnique();

        entity.Property(e => e.Id)
            .HasColumnName("Id");

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("Name");

        entity.Property(e => e.Description)
            .HasMaxLength(500)
            .HasColumnName("Description");

        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt");

        entity.Property(e => e.UpdatedAt)
            .HasColumnName("UpdatedAt");

        entity.HasMany(e => e.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

