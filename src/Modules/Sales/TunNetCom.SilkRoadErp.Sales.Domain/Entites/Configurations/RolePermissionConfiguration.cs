using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> entity)
    {
        entity.HasKey(e => new { e.RoleId, e.PermissionId }).HasName("PK_RolePermission");

        entity.ToTable("RolePermissions");

        entity.Property(e => e.RoleId)
            .HasColumnName("RoleId");

        entity.Property(e => e.PermissionId)
            .HasColumnName("PermissionId");

        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt");

        entity.HasOne(e => e.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(e => e.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

