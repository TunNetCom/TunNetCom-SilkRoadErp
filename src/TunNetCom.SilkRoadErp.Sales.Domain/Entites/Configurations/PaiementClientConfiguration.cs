using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class PaiementClientConfiguration : IEntityTypeConfiguration<PaiementClient>
{
    public void Configure(EntityTypeBuilder<PaiementClient> entity)
    {
        entity.ToTable("PaiementClient", t =>
        {
            t.HasCheckConstraint("CHK_PaiementClient_Montant", "Montant > 0");
            t.HasCheckConstraint("CHK_PaiementClient_Document", 
                "(FactureId IS NULL AND BonDeLivraisonId IS NULL) OR " +
                "(FactureId IS NOT NULL AND BonDeLivraisonId IS NULL) OR " +
                "(FactureId IS NULL AND BonDeLivraisonId IS NOT NULL)");
        });

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Numero)
            .HasColumnName("Numero")
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(e => e.ClientId)
            .HasColumnName("ClientId")
            .IsRequired();

        entity.Property(e => e.AccountingYearId)
            .HasColumnName("AccountingYearId")
            .IsRequired();

        entity.Property(e => e.Montant)
            .HasColumnName("Montant")
            .HasColumnType("decimal(18, 3)")
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

        entity.Property(e => e.FactureId)
            .HasColumnName("FactureId");

        entity.Property(e => e.BonDeLivraisonId)
            .HasColumnName("BonDeLivraisonId");

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

        entity.Property(e => e.DocumentStoragePath)
            .HasColumnName("DocumentStoragePath")
            .HasColumnType("nvarchar(max)");

        entity.Property(e => e.DateModification)
            .HasColumnName("DateModification")
            .HasColumnType("datetime");

        entity.HasIndex(e => e.Numero)
            .IsUnique()
            .HasDatabaseName("IX_PaiementClient_Numero");

        entity.HasIndex(e => e.ClientId)
            .HasDatabaseName("IX_PaiementClient_ClientId");

        entity.HasIndex(e => e.AccountingYearId)
            .HasDatabaseName("IX_PaiementClient_AccountingYearId");

        entity.HasIndex(e => e.DatePaiement)
            .HasDatabaseName("IX_PaiementClient_DatePaiement");

        entity.HasOne(d => d.Client)
            .WithMany(p => p.PaiementClient)
            .HasForeignKey(d => d.ClientId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementClient_Client");

        entity.HasOne(d => d.AccountingYear)
            .WithMany()
            .HasForeignKey(d => d.AccountingYearId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementClient_AccountingYear");

        entity.HasOne(d => d.Banque)
            .WithMany(p => p.PaiementClient)
            .HasForeignKey(d => d.BanqueId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_PaiementClient_Banque");

        entity.HasOne(d => d.Facture)
            .WithMany()
            .HasForeignKey(d => d.FactureId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_PaiementClient_Facture");

        entity.HasOne(d => d.BonDeLivraison)
            .WithMany()
            .HasForeignKey(d => d.BonDeLivraisonId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_PaiementClient_BonDeLivraison");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PaiementClient> entity);
}

