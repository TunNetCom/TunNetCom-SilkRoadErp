using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> entity)
    {
        entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK_UserRole");

        entity.ToTable("UserRoles");

        entity.Property(e => e.UserId)
            .HasColumnName("UserId");

        entity.Property(e => e.RoleId)
            .HasColumnName("RoleId");

        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt");

        entity.HasOne(e => e.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

