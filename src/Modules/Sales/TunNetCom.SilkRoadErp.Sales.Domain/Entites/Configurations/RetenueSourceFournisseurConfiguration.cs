using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations
{
    public partial class RetenueSourceFournisseurConfiguration : IEntityTypeConfiguration<RetenueSourceFournisseur>
    {
        public void Configure(EntityTypeBuilder<RetenueSourceFournisseur> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.RetenueSourceFournisseur");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasIndex(e => e.NumFactureFournisseur)
                .IsUnique()
                .HasDatabaseName("IX_RetenueSourceFournisseur_NumFactureFournisseur");

            entity.Property(e => e.NumFactureFournisseur)
                .HasColumnName("NumFactureFournisseur");

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

            entity.HasOne(d => d.NumFactureFournisseurNavigation)
                .WithMany()
                .HasForeignKey(d => d.NumFactureFournisseur)
                .HasPrincipalKey(p => p.Num)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_dbo.RetenueSourceFournisseur_dbo.FactureFournisseur_NumFactureFournisseur");

            entity.HasOne(d => d.AccountingYear)
                .WithMany()
                .HasForeignKey(d => d.AccountingYearId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_dbo.RetenueSourceFournisseur_dbo.AccountingYear_AccountingYearId");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<RetenueSourceFournisseur> entity);
    }
}


