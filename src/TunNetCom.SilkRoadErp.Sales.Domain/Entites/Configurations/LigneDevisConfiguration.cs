﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations
{
    public partial class LigneDevisConfiguration : IEntityTypeConfiguration<LigneDevis>
    {
        public void Configure(EntityTypeBuilder<LigneDevis> entity)
        {
            entity.HasKey(e => e.IdLi).HasName("PK_dbo.LigneDevis");

            entity.Property(e => e.IdLi).HasColumnName("Id_li");
            entity.Property(e => e.DesignationLi).HasColumnName("Designation_li");
            entity.Property(e => e.NumDevis).HasColumnName("Num_devis");
            entity.Property(e => e.PrixHt)
            .HasColumnType("decimal(18, 2)")
            .HasColumnName("prix_HT");
            entity.Property(e => e.QteLi).HasColumnName("qte_li");
            entity.Property(e => e.RefProduit)
            .HasMaxLength(50)
            .HasColumnName("Ref_produit");
            entity.Property(e => e.Remise).HasColumnName("remise");
            entity.Property(e => e.TotHt)
            .HasColumnType("decimal(18, 2)")
            .HasColumnName("tot_HT");
            entity.Property(e => e.TotTtc)
            .HasColumnType("decimal(18, 2)")
            .HasColumnName("tot_TTC");
            entity.Property(e => e.Tva).HasColumnName("tva");

            entity.HasOne(d => d.NumDevisNavigation).WithMany(p => p.LigneDevis)
            .HasForeignKey(d => d.NumDevis)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_dbo.LigneDevis_dbo.Devis_Num_devis");

            entity.HasOne(d => d.RefProduitNavigation).WithMany(p => p.LigneDevis)
            .HasForeignKey(d => d.RefProduit)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_dbo.LigneDevis_dbo.Produit_Ref_produit");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<LigneDevis> entity);
    }
}
