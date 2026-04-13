using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class PaiementClientBonDeLivraisonConfiguration : IEntityTypeConfiguration<PaiementClientBonDeLivraison>
{
    public void Configure(EntityTypeBuilder<PaiementClientBonDeLivraison> entity)
    {
        entity.ToTable("PaiementClientBonDeLivraison");

        entity.HasKey(e => new { e.PaiementClientId, e.BonDeLivraisonId });

        entity.Property(e => e.PaiementClientId)
            .HasColumnName("PaiementClientId")
            .IsRequired();

        entity.Property(e => e.BonDeLivraisonId)
            .HasColumnName("BonDeLivraisonId")
            .IsRequired();

        entity.HasOne(d => d.PaiementClient)
            .WithMany(p => p.BonDeLivraisons)
            .HasForeignKey(d => d.PaiementClientId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PaiementClientBonDeLivraison_PaiementClient");

        entity.HasOne(d => d.BonDeLivraison)
            .WithMany()
            .HasForeignKey(d => d.BonDeLivraisonId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementClientBonDeLivraison_BonDeLivraison");
    }
}

