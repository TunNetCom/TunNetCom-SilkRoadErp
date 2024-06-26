﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations
{
    public partial class BonDeLivraisonConfiguration : IEntityTypeConfiguration<BonDeLivraison>
    {
        public void Configure(EntityTypeBuilder<BonDeLivraison> entity)
        {
            entity.HasKey(e => e.Num).HasName("PK_dbo.BonDeLivraison");

            entity.Property(e => e.ClientId).HasColumnName("clientId");
            entity.Property(e => e.Date)
            .HasColumnType("datetime")
            .HasColumnName("date");
            entity.Property(e => e.NetPayer)
            .HasColumnType("decimal(18, 2)")
            .HasColumnName("net_payer");
            entity.Property(e => e.NumFacture).HasColumnName("Num_Facture");
            entity.Property(e => e.TempBl).HasColumnName("temp_bl");
            entity.Property(e => e.TotHTva)
            .HasColumnType("decimal(18, 2)")
            .HasColumnName("tot_H_tva");
            entity.Property(e => e.TotTva)
            .HasColumnType("decimal(18, 2)")
            .HasColumnName("tot_tva");

            entity.HasOne(d => d.Client).WithMany(p => p.BonDeLivraison)
            .HasForeignKey(d => d.ClientId)
            .HasConstraintName("FK_dbo.BonDeLivraison_dbo.Client_clientId");

            entity.HasOne(d => d.NumFactureNavigation).WithMany(p => p.BonDeLivraison)
            .HasForeignKey(d => d.NumFacture)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_dbo.BonDeLivraison_dbo.Facture_Num_Facture");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<BonDeLivraison> entity);
    }
}
