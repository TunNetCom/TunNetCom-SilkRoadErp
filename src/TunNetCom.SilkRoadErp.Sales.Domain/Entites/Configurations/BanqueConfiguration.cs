using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class BanqueConfiguration : IEntityTypeConfiguration<Banque>
{
    public void Configure(EntityTypeBuilder<Banque> entity)
    {
        entity.ToTable("Banque");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Nom)
            .HasColumnName("Nom")
            .HasMaxLength(200)
            .IsRequired();

        entity.HasIndex(e => e.Nom)
            .IsUnique()
            .HasDatabaseName("IX_Banque_Nom");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Banque> entity);
}

