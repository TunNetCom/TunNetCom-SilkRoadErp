using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class LigneInventaireConfiguration : IEntityTypeConfiguration<LigneInventaire>
{
    public void Configure(EntityTypeBuilder<LigneInventaire> entity)
    {
        entity.ToTable("LigneInventaire");

        entity.HasKey(e => e.Id).HasName("PK_dbo.LigneInventaire");

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.InventaireId)
            .HasColumnName("InventaireId")
            .IsRequired();

        entity.Property(e => e.RefProduit)
            .HasColumnName("RefProduit")
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(e => e.QuantiteTheorique)
            .HasColumnName("QuantiteTheorique")
            .IsRequired();

        entity.Property(e => e.QuantiteReelle)
            .HasColumnName("QuantiteReelle")
            .IsRequired();

        entity.Property(e => e.PrixHt)
            .HasColumnName("PrixHt")
            .HasColumnType("decimal(18, 3)")
            .IsRequired();

        entity.Property(e => e.DernierPrixAchat)
            .HasColumnName("DernierPrixAchat")
            .HasColumnType("decimal(18, 3)")
            .IsRequired();

        entity.HasIndex(e => e.InventaireId)
            .HasDatabaseName("IX_LigneInventaire_InventaireId");

        entity.HasIndex(e => e.RefProduit)
            .HasDatabaseName("IX_LigneInventaire_RefProduit");

        entity.HasOne(d => d.Inventaire)
            .WithMany(p => p.LigneInventaire)
            .HasForeignKey(d => d.InventaireId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_dbo.LigneInventaire_dbo.Inventaire_InventaireId");

        entity.HasOne(d => d.RefProduitNavigation)
            .WithMany()
            .HasForeignKey(d => d.RefProduit)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_dbo.LigneInventaire_dbo.Produit_RefProduit");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<LigneInventaire> entity);
}

