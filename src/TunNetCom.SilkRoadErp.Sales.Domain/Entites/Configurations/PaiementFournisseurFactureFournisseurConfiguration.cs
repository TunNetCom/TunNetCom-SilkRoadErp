using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class PaiementFournisseurFactureFournisseurConfiguration : IEntityTypeConfiguration<PaiementFournisseurFactureFournisseur>
{
    public void Configure(EntityTypeBuilder<PaiementFournisseurFactureFournisseur> entity)
    {
        entity.ToTable("PaiementFournisseurFactureFournisseur");

        entity.HasKey(e => new { e.PaiementFournisseurId, e.FactureFournisseurId });

        entity.Property(e => e.PaiementFournisseurId)
            .HasColumnName("PaiementFournisseurId")
            .IsRequired();

        entity.Property(e => e.FactureFournisseurId)
            .HasColumnName("FactureFournisseurId")
            .IsRequired();

        entity.HasOne(d => d.PaiementFournisseur)
            .WithMany(p => p.FactureFournisseurs)
            .HasForeignKey(d => d.PaiementFournisseurId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PaiementFournisseurFactureFournisseur_PaiementFournisseur");

        entity.HasOne(d => d.FactureFournisseur)
            .WithMany()
            .HasForeignKey(d => d.FactureFournisseurId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementFournisseurFactureFournisseur_FactureFournisseur");
    }
}

