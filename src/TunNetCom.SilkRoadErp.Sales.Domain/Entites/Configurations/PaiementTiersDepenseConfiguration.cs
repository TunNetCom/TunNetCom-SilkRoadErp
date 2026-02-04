using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class PaiementTiersDepenseConfiguration : IEntityTypeConfiguration<PaiementTiersDepense>
{
    public void Configure(EntityTypeBuilder<PaiementTiersDepense> entity)
    {
        entity.ToTable("PaiementTiersDepense", t =>
        {
            t.HasCheckConstraint("CHK_PaiementTiersDepense_Montant", "Montant > 0");
            t.HasCheckConstraint("CHK_PaiementTiersDepense_Mois", "Mois IS NULL OR (Mois >= 1 AND Mois <= 12)");
        });

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.NumeroTransactionBancaire)
            .HasMaxLength(50);

        entity.Property(e => e.TiersDepenseFonctionnementId)
            .IsRequired();

        entity.Property(e => e.AccountingYearId)
            .IsRequired();

        entity.Property(e => e.Montant)
            .HasColumnType("decimal(18, 3)")
            .IsRequired();

        entity.Property(e => e.DatePaiement)
            .HasColumnType("datetime")
            .IsRequired();

        entity.Property(e => e.MethodePaiement)
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        entity.Property(e => e.NumeroChequeTraite)
            .HasMaxLength(100);

        entity.Property(e => e.Commentaire)
            .HasMaxLength(500);

        entity.Property(e => e.RibCodeEtab)
            .HasMaxLength(10);

        entity.Property(e => e.RibCodeAgence)
            .HasMaxLength(10);

        entity.Property(e => e.RibNumeroCompte)
            .HasMaxLength(20);

        entity.Property(e => e.RibCle)
            .HasMaxLength(5);

        entity.Property(e => e.DocumentStoragePath)
            .HasColumnName("DocumentStoragePath")
            .HasColumnType("nvarchar(max)");

        entity.Property(e => e.DateEcheance)
            .HasColumnType("datetime");

        entity.Property(e => e.DateModification)
            .HasColumnType("datetime");

        entity.HasIndex(e => e.TiersDepenseFonctionnementId)
            .HasDatabaseName("IX_PaiementTiersDepense_TiersDepenseFonctionnementId");

        entity.HasIndex(e => e.AccountingYearId)
            .HasDatabaseName("IX_PaiementTiersDepense_AccountingYearId");

        entity.HasIndex(e => e.DatePaiement)
            .HasDatabaseName("IX_PaiementTiersDepense_DatePaiement");

        entity.HasOne(d => d.TiersDepenseFonctionnement)
            .WithMany(p => p.PaiementTiersDepense)
            .HasForeignKey(d => d.TiersDepenseFonctionnementId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementTiersDepense_TiersDepenseFonctionnement");

        entity.HasOne(d => d.AccountingYear)
            .WithMany(p => p.PaiementTiersDepense)
            .HasForeignKey(d => d.AccountingYearId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementTiersDepense_AccountingYear");

        entity.HasOne(d => d.Banque)
            .WithMany(p => p.PaiementTiersDepense)
            .HasForeignKey(d => d.BanqueId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_PaiementTiersDepense_Banque");

        entity.HasMany(d => d.FactureDepenses)
            .WithOne(p => p.PaiementTiersDepense)
            .HasForeignKey(p => p.PaiementTiersDepenseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
