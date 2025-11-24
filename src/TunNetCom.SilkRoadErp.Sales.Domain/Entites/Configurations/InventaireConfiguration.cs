using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class InventaireConfiguration : IEntityTypeConfiguration<Inventaire>
{
    public void Configure(EntityTypeBuilder<Inventaire> entity)
    {
        entity.ToTable("Inventaire");

        entity.HasKey(e => e.Id).HasName("PK_dbo.Inventaire");

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Num)
            .HasColumnName("Num")
            .IsRequired();

        entity.Property(e => e.AccountingYearId)
            .HasColumnName("AccountingYearId")
            .IsRequired();

        entity.Property(e => e.DateInventaire)
            .HasColumnName("DateInventaire")
            .HasColumnType("datetime")
            .IsRequired();

        entity.Property(e => e.Description)
            .HasColumnName("Description")
            .HasMaxLength(500);

        entity.Property(e => e.Statut)
            .HasColumnName("Statut")
            .HasConversion<int>()
            .IsRequired();

        entity.HasIndex(e => new { e.AccountingYearId, e.Num })
            .IsUnique()
            .HasDatabaseName("IX_Inventaire_AccountingYearId_Num");

        entity.HasIndex(e => e.AccountingYearId)
            .HasDatabaseName("IX_Inventaire_AccountingYearId");

        entity.HasOne(d => d.AccountingYear)
            .WithMany(p => p.Inventaires)
            .HasForeignKey(d => d.AccountingYearId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_dbo.Inventaire_dbo.AccountingYear_AccountingYearId");

        entity.HasMany(d => d.LigneInventaire)
            .WithOne(p => p.Inventaire)
            .HasForeignKey(d => d.InventaireId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_dbo.LigneInventaire_dbo.Inventaire_InventaireId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Inventaire> entity);
}

