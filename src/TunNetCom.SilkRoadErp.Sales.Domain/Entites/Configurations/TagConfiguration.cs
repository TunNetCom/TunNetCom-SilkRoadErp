using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> entity)
    {
        entity.ToTable("Tag");

        entity.HasKey(e => e.Id).HasName("PK_dbo.Tag");

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name)
            .HasColumnName("Name")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(e => e.Color)
            .HasColumnName("Color")
            .HasMaxLength(50);

        entity.Property(e => e.Description)
            .HasColumnName("Description")
            .HasMaxLength(500);

        entity.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_Tag_Name");

        entity.HasMany(d => d.DocumentTags)
            .WithOne(p => p.Tag)
            .HasForeignKey(d => d.TagId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_dbo.DocumentTag_dbo.Tag_TagId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Tag> entity);
}

