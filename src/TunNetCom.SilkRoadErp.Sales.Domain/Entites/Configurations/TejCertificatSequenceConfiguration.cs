using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class TejCertificatSequenceConfiguration : IEntityTypeConfiguration<TejCertificatSequence>
{
    public void Configure(EntityTypeBuilder<TejCertificatSequence> entity)
    {
        entity.HasKey(e => new { e.Annee, e.Mois })
            .HasName("PK_TejCertificatSequence");

        entity.ToTable("TejCertificatSequence");

        entity.Property(e => e.Annee)
            .HasColumnName("Annee");
        entity.Property(e => e.Mois)
            .HasColumnName("Mois");
        entity.Property(e => e.DerniereSequence)
            .HasColumnName("DerniereSequence");
        entity.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasColumnName("RowVersion");
    }
}
