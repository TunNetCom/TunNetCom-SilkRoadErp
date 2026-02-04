using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class FactureDepenseConfiguration : IEntityTypeConfiguration<FactureDepense>
{
    public void Configure(EntityTypeBuilder<FactureDepense> entity)
    {
        entity.ToTable("FactureDepense");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.HasIndex(e => new { e.Num, e.AccountingYearId })
            .IsUnique()
            .HasDatabaseName("IX_FactureDepense_Num_AccountingYearId");

        entity.Property(e => e.Num)
            .IsRequired();

        entity.Property(e => e.IdTiersDepenseFonctionnement)
            .HasColumnName("IdTiersDepenseFonctionnement")
            .IsRequired();

        entity.Property(e => e.Date)
            .HasColumnType("datetime")
            .IsRequired();

        entity.Property(e => e.Description)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        entity.Property(e => e.MontantTotal)
            .HasColumnType("decimal(18, 3)")
            .IsRequired();

        entity.Property(e => e.AccountingYearId)
            .IsRequired();

        entity.Property(e => e.Statut)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(e => e.DocumentStoragePath)
            .HasColumnName("DocumentStoragePath")
            .HasColumnType("nvarchar(max)");

        entity.HasOne(d => d.IdTiersDepenseFonctionnementNavigation)
            .WithMany(p => p.FactureDepense)
            .HasForeignKey(d => d.IdTiersDepenseFonctionnement)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_FactureDepense_TiersDepenseFonctionnement");

        entity.HasOne(d => d.AccountingYear)
            .WithMany(p => p.FactureDepense)
            .HasForeignKey(d => d.AccountingYearId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_FactureDepense_AccountingYear");

        entity.HasMany(d => d.PaiementTiersDepenseFactureDepense)
            .WithOne(p => p.FactureDepense)
            .HasForeignKey(p => p.FactureDepenseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
