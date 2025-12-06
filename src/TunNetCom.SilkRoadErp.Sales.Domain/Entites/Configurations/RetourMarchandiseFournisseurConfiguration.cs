using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations
{
    public partial class RetourMarchandiseFournisseurConfiguration : IEntityTypeConfiguration<RetourMarchandiseFournisseur>
    {
        public void Configure(EntityTypeBuilder<RetourMarchandiseFournisseur> entity)
        {
            entity.ToTable("RetourMarchandiseFournisseur");

            entity.HasKey(e => e.Id).HasName("PK_dbo.RetourMarchandiseFournisseur");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasIndex(e => e.Num)
                .IsUnique()
                .HasDatabaseName("IX_RetourMarchandiseFournisseur_Num");

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.IdFournisseur).HasColumnName("id_fournisseur");
            entity.Property(e => e.TotHTva)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("tot_H_tva");
            entity.Property(e => e.TotTva)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("tot_tva");
            entity.Property(e => e.NetPayer)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("net_payer");
            entity.Property(e => e.Statut)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(d => d.IdFournisseurNavigation).WithMany()
                .HasForeignKey(d => d.IdFournisseur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.RetourMarchandiseFournisseur_dbo.Fournisseur_id_fournisseur");

            entity.HasOne(d => d.AccountingYear).WithMany()
                .HasForeignKey(d => d.AccountingYearId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_dbo.RetourMarchandiseFournisseur_dbo.AccountingYear_AccountingYearId");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<RetourMarchandiseFournisseur> entity);
    }
}

