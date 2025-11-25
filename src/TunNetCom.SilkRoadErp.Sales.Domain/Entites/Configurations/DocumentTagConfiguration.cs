using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class DocumentTagConfiguration : IEntityTypeConfiguration<DocumentTag>
{
    public void Configure(EntityTypeBuilder<DocumentTag> entity)
    {
        entity.ToTable("DocumentTag");

        entity.HasKey(e => e.Id).HasName("PK_dbo.DocumentTag");

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.TagId)
            .HasColumnName("TagId")
            .IsRequired();

        entity.Property(e => e.DocumentType)
            .HasColumnName("DocumentType")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(e => e.DocumentId)
            .HasColumnName("DocumentId")
            .IsRequired();

        entity.HasIndex(e => new { e.DocumentType, e.DocumentId, e.TagId })
            .IsUnique()
            .HasDatabaseName("IX_DocumentTag_DocumentType_DocumentId_TagId");

        entity.HasIndex(e => new { e.DocumentType, e.DocumentId })
            .HasDatabaseName("IX_DocumentTag_DocumentType_DocumentId");

        entity.HasIndex(e => e.TagId)
            .HasDatabaseName("IX_DocumentTag_TagId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<DocumentTag> entity);
}

