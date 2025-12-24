using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class PaiementFournisseurBonDeReceptionConfiguration : IEntityTypeConfiguration<PaiementFournisseurBonDeReception>
{
    public void Configure(EntityTypeBuilder<PaiementFournisseurBonDeReception> entity)
    {
        entity.ToTable("PaiementFournisseurBonDeReception");

        entity.HasKey(e => new { e.PaiementFournisseurId, e.BonDeReceptionId });

        entity.Property(e => e.PaiementFournisseurId)
            .HasColumnName("PaiementFournisseurId")
            .IsRequired();

        entity.Property(e => e.BonDeReceptionId)
            .HasColumnName("BonDeReceptionId")
            .IsRequired();

        entity.HasOne(d => d.PaiementFournisseur)
            .WithMany(p => p.BonDeReceptions)
            .HasForeignKey(d => d.PaiementFournisseurId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PaiementFournisseurBonDeReception_PaiementFournisseur");

        entity.HasOne(d => d.BonDeReception)
            .WithMany()
            .HasForeignKey(d => d.BonDeReceptionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementFournisseurBonDeReception_BonDeReception");
    }
}

