using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations
{
    public partial class LigneRetourMarchandiseFournisseurConfiguration : IEntityTypeConfiguration<LigneRetourMarchandiseFournisseur>
    {
        public void Configure(EntityTypeBuilder<LigneRetourMarchandiseFournisseur> entity)
        {
            entity.ToTable("LigneRetourMarchandiseFournisseur");

            entity.HasKey(e => e.IdLigne).HasName("PK_dbo.LigneRetourMarchandiseFournisseur");

            entity.Property(e => e.IdLigne).HasColumnName("Id_ligne");
            entity.Property(e => e.DesignationLi).HasColumnName("designation_li");
            entity.Property(e => e.RetourMarchandiseFournisseurId).HasColumnName("RetourMarchandiseFournisseurId");
            entity.Property(e => e.PrixHt)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("prix_HT");
            entity.Property(e => e.QteLi).HasColumnName("qte_li");
            entity.Property(e => e.RefProduit)
                .HasMaxLength(50)
                .HasColumnName("Ref_Produit");
            entity.Property(e => e.Remise).HasColumnName("remise");
            entity.Property(e => e.TotHt)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("tot_HT");
            entity.Property(e => e.TotTtc)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("tot_TTC");
            entity.Property(e => e.Tva).HasColumnName("tva");

            // Nouveaux champs pour la réception après réparation
            entity.Property(e => e.QteRecue)
                .HasColumnName("qte_recue")
                .HasDefaultValue(0);
            
            entity.Property(e => e.DateReception)
                .HasColumnType("datetime")
                .HasColumnName("date_reception")
                .IsRequired(false);
            
            entity.Property(e => e.UtilisateurReception)
                .HasMaxLength(256)
                .HasColumnName("utilisateur_reception")
                .IsRequired(false);

            entity.HasOne(d => d.RetourMarchandiseFournisseurNavigation).WithMany(p => p.LigneRetourMarchandiseFournisseur)
                .HasForeignKey(d => d.RetourMarchandiseFournisseurId)
                .HasConstraintName("FK_dbo.LigneRetourMarchandiseFournisseur_dbo.RetourMarchandiseFournisseur_RetourMarchandiseFournisseurId");

            entity.HasOne(d => d.RefProduitNavigation).WithMany()
                .HasForeignKey(d => d.RefProduit)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.LigneRetourMarchandiseFournisseur_dbo.Produit_Ref_Produit");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<LigneRetourMarchandiseFournisseur> entity);
    }
}
