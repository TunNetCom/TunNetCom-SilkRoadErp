using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class PaiementFournisseurConfiguration : IEntityTypeConfiguration<PaiementFournisseur>
{
    public void Configure(EntityTypeBuilder<PaiementFournisseur> entity)
    {
        entity.ToTable("PaiementFournisseur", t =>
        {
            t.HasCheckConstraint("CHK_PaiementFournisseur_Montant", "Montant > 0");
            t.HasCheckConstraint("CHK_PaiementFournisseur_Document", 
                "(FactureFournisseurId IS NULL AND BonDeReceptionId IS NULL) OR " +
                "(FactureFournisseurId IS NOT NULL AND BonDeReceptionId IS NULL) OR " +
                "(FactureFournisseurId IS NULL AND BonDeReceptionId IS NOT NULL)");
        });

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Numero)
            .HasColumnName("Numero")
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(e => e.FournisseurId)
            .HasColumnName("FournisseurId")
            .IsRequired();

        entity.Property(e => e.AccountingYearId)
            .HasColumnName("AccountingYearId")
            .IsRequired();

        entity.Property(e => e.Montant)
            .HasColumnName("Montant")
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        entity.Property(e => e.DatePaiement)
            .HasColumnName("DatePaiement")
            .HasColumnType("datetime")
            .IsRequired();

        entity.Property(e => e.MethodePaiement)
            .HasColumnName("MethodePaiement")
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        entity.Property(e => e.FactureFournisseurId)
            .HasColumnName("FactureFournisseurId");

        entity.Property(e => e.BonDeReceptionId)
            .HasColumnName("BonDeReceptionId");

        entity.Property(e => e.NumeroChequeTraite)
            .HasColumnName("NumeroChequeTraite")
            .HasMaxLength(100);

        entity.Property(e => e.BanqueId)
            .HasColumnName("BanqueId");

        entity.Property(e => e.DateEcheance)
            .HasColumnName("DateEcheance")
            .HasColumnType("datetime");

        entity.Property(e => e.Commentaire)
            .HasColumnName("Commentaire")
            .HasMaxLength(500);

        entity.Property(e => e.DateModification)
            .HasColumnName("DateModification")
            .HasColumnType("datetime");

        entity.HasIndex(e => e.Numero)
            .IsUnique()
            .HasDatabaseName("IX_PaiementFournisseur_Numero");

        entity.HasIndex(e => e.FournisseurId)
            .HasDatabaseName("IX_PaiementFournisseur_FournisseurId");

        entity.HasIndex(e => e.AccountingYearId)
            .HasDatabaseName("IX_PaiementFournisseur_AccountingYearId");

        entity.HasIndex(e => e.DatePaiement)
            .HasDatabaseName("IX_PaiementFournisseur_DatePaiement");

        entity.HasOne(d => d.Fournisseur)
            .WithMany(p => p.PaiementFournisseur)
            .HasForeignKey(d => d.FournisseurId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementFournisseur_Fournisseur");

        entity.HasOne(d => d.AccountingYear)
            .WithMany()
            .HasForeignKey(d => d.AccountingYearId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementFournisseur_AccountingYear");

        entity.HasOne(d => d.Banque)
            .WithMany(p => p.PaiementFournisseur)
            .HasForeignKey(d => d.BanqueId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_PaiementFournisseur_Banque");

        entity.HasOne(d => d.FactureFournisseur)
            .WithMany()
            .HasForeignKey(d => d.FactureFournisseurId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_PaiementFournisseur_FactureFournisseur");

        entity.HasOne(d => d.BonDeReception)
            .WithMany()
            .HasForeignKey(d => d.BonDeReceptionId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_PaiementFournisseur_BonDeReception");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PaiementFournisseur> entity);
}

