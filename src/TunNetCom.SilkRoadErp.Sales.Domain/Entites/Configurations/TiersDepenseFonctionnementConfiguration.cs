using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class TiersDepenseFonctionnementConfiguration : IEntityTypeConfiguration<TiersDepenseFonctionnement>
{
    public void Configure(EntityTypeBuilder<TiersDepenseFonctionnement> entity)
    {
        entity.ToTable("TiersDepenseFonctionnement");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Nom)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.Tel)
            .HasMaxLength(50);

        entity.Property(e => e.Adresse)
            .HasMaxLength(500);

        entity.Property(e => e.Matricule)
            .HasMaxLength(50);

        entity.Property(e => e.Code)
            .HasMaxLength(50);

        entity.Property(e => e.CodeCat)
            .HasMaxLength(50);

        entity.Property(e => e.EtbSec)
            .HasMaxLength(50);

        entity.Property(e => e.Mail)
            .HasMaxLength(200);

        entity.HasMany(d => d.FactureDepense)
            .WithOne(p => p.IdTiersDepenseFonctionnementNavigation)
            .HasForeignKey(d => d.IdTiersDepenseFonctionnement)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(d => d.PaiementTiersDepense)
            .WithOne(p => p.TiersDepenseFonctionnement)
            .HasForeignKey(d => d.TiersDepenseFonctionnementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
