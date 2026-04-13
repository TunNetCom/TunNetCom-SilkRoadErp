using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class PaiementClientFactureConfiguration : IEntityTypeConfiguration<PaiementClientFacture>
{
    public void Configure(EntityTypeBuilder<PaiementClientFacture> entity)
    {
        entity.ToTable("PaiementClientFacture");

        entity.HasKey(e => new { e.PaiementClientId, e.FactureId });

        entity.Property(e => e.PaiementClientId)
            .HasColumnName("PaiementClientId")
            .IsRequired();

        entity.Property(e => e.FactureId)
            .HasColumnName("FactureId")
            .IsRequired();

        entity.HasOne(d => d.PaiementClient)
            .WithMany(p => p.Factures)
            .HasForeignKey(d => d.PaiementClientId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PaiementClientFacture_PaiementClient");

        entity.HasOne(d => d.Facture)
            .WithMany()
            .HasForeignKey(d => d.FactureId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementClientFacture_Facture");
    }
}

