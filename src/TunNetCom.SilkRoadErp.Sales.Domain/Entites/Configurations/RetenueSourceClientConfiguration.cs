using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations
{
    public partial class RetenueSourceClientConfiguration : IEntityTypeConfiguration<RetenueSourceClient>
    {
        public void Configure(EntityTypeBuilder<RetenueSourceClient> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.RetenueSourceClient");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasIndex(e => e.NumFacture)
                .IsUnique()
                .HasDatabaseName("IX_RetenueSourceClient_NumFacture");

            entity.Property(e => e.NumFacture)
                .HasColumnName("NumFacture");

            entity.Property(e => e.NumTej)
                .HasMaxLength(100)
                .HasColumnName("NumTej");

            entity.Property(e => e.MontantAvantRetenu)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("MontantAvantRetenu");

            entity.Property(e => e.TauxRetenu)
                .HasColumnName("TauxRetenu");

            entity.Property(e => e.MontantApresRetenu)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("MontantApresRetenu");

            entity.Property(e => e.PdfStoragePath)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("PdfStoragePath");

            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("DateCreation");

            entity.HasOne(d => d.NumFactureNavigation)
                .WithMany()
                .HasForeignKey(d => d.NumFacture)
                .HasPrincipalKey(p => p.Num)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_dbo.RetenueSourceClient_dbo.Facture_NumFacture");

            entity.HasOne(d => d.AccountingYear)
                .WithMany()
                .HasForeignKey(d => d.AccountingYearId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_dbo.RetenueSourceClient_dbo.AccountingYear_AccountingYearId");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<RetenueSourceClient> entity);
    }
}


