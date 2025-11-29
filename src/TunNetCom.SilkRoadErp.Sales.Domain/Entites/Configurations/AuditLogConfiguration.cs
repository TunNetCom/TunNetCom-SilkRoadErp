using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK_AuditLog");

        entity.ToTable("AuditLog");

        // Indexes for common queries
        entity.HasIndex(e => e.EntityName).HasDatabaseName("IX_AuditLog_EntityName");
        entity.HasIndex(e => e.EntityId).HasDatabaseName("IX_AuditLog_EntityId");
        entity.HasIndex(e => e.UserId).HasDatabaseName("IX_AuditLog_UserId");
        entity.HasIndex(e => e.Timestamp).HasDatabaseName("IX_AuditLog_Timestamp");
        entity.HasIndex(e => new { e.EntityName, e.EntityId }).HasDatabaseName("IX_AuditLog_EntityName_EntityId");
        entity.HasIndex(e => new { e.EntityName, e.EntityId, e.Timestamp }).HasDatabaseName("IX_AuditLog_EntityName_EntityId_Timestamp");

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.EntityName)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("EntityName");

        entity.Property(e => e.EntityId)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("EntityId");

        entity.Property(e => e.Action)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("Action");

        entity.Property(e => e.UserId)
            .HasColumnName("UserId");

        entity.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("Username");

        entity.Property(e => e.Timestamp)
            .IsRequired()
            .HasColumnName("Timestamp");

        // JSON columns stored as NVARCHAR(MAX) for SQL Server
        entity.Property(e => e.OldValues)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("OldValues");

        entity.Property(e => e.NewValues)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("NewValues");

        entity.Property(e => e.ChangedProperties)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("ChangedProperties");
    }
}





