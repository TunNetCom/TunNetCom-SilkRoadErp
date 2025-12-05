using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> entity)
    {
        entity.ToTable("Notification");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Type)
            .HasColumnName("Type")
            .HasConversion<int>()
            .IsRequired();

        entity.Property(e => e.Title)
            .HasColumnName("Title")
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.Message)
            .HasColumnName("Message")
            .HasMaxLength(1000)
            .IsRequired();

        entity.Property(e => e.RelatedEntityId)
            .HasColumnName("RelatedEntityId");

        entity.Property(e => e.RelatedEntityType)
            .HasColumnName("RelatedEntityType")
            .HasMaxLength(50);

        entity.Property(e => e.IsRead)
            .HasColumnName("IsRead")
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.UserId)
            .HasColumnName("UserId");

        entity.Property(e => e.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime")
            .IsRequired();

        entity.Property(e => e.ReadAt)
            .HasColumnName("ReadAt")
            .HasColumnType("datetime");

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_Notification_UserId");

        entity.HasIndex(e => e.IsRead)
            .HasDatabaseName("IX_Notification_IsRead");

        entity.HasIndex(e => e.Type)
            .HasDatabaseName("IX_Notification_Type");

        entity.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_Notification_CreatedAt");

        entity.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Notification_User");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Notification> entity);
}

