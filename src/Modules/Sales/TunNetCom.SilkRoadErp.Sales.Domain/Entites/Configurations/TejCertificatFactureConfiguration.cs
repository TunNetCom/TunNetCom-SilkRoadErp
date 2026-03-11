using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class TejCertificatFactureConfiguration : IEntityTypeConfiguration<TejCertificatFacture>
{
    public void Configure(EntityTypeBuilder<TejCertificatFacture> entity)
    {
        entity.HasKey(e => e.FactureFournisseurId)
            .HasName("PK_TejCertificatFacture");

        entity.ToTable("TejCertificatFacture");

        entity.Property(e => e.FactureFournisseurId)
            .HasColumnName("FactureFournisseurId");
        entity.Property(e => e.RefCertif)
            .HasMaxLength(50)
            .HasColumnName("RefCertif");

        entity.HasOne(e => e.FactureFournisseur)
            .WithMany()
            .HasForeignKey(e => e.FactureFournisseurId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
