using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class RetenueSourceFactureDepenseConfiguration : IEntityTypeConfiguration<RetenueSourceFactureDepense>
{
    public void Configure(EntityTypeBuilder<RetenueSourceFactureDepense> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK_RetenueSourceFactureDepense");
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.HasIndex(e => e.FactureDepenseId)
            .IsUnique()
            .HasDatabaseName("IX_RetenueSourceFactureDepense_FactureDepenseId");

        entity.Property(e => e.FactureDepenseId)
            .HasColumnName("FactureDepenseId");

        entity.Property(e => e.NumTej)
            .HasMaxLength(100)
            .HasColumnName("NumTej");

        entity.Property(e => e.MontantAvantRetenu)
            .HasColumnType("decimal(18, 3)")
            .HasColumnName("MontantAvantRetenu");

        entity.Property(e => e.TauxRetenu)
            .HasColumnName("TauxRetenu");

        entity.Property(e => e.MontantApresRetenu)
            .HasColumnType("decimal(18, 3)")
            .HasColumnName("MontantApresRetenu");

        entity.Property(e => e.PdfStoragePath)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("PdfStoragePath");

        entity.Property(e => e.DateCreation)
            .HasColumnType("datetime")
            .HasColumnName("DateCreation");

        entity.HasOne(d => d.FactureDepense)
            .WithMany()
            .HasForeignKey(d => d.FactureDepenseId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RetenueSourceFactureDepense_FactureDepense");

        entity.HasOne(d => d.AccountingYear)
            .WithMany()
            .HasForeignKey(d => d.AccountingYearId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RetenueSourceFactureDepense_AccountingYear");
    }
}
