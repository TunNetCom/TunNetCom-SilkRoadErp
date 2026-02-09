using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class BankTransactionImportConfiguration : IEntityTypeConfiguration<BankTransactionImport>
{
    public void Configure(EntityTypeBuilder<BankTransactionImport> entity)
    {
        entity.ToTable("BankTransactionImport");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.CompteBancaireId)
            .HasColumnName("CompteBancaireId")
            .IsRequired();

        entity.Property(e => e.FileName)
            .HasColumnName("FileName")
            .HasMaxLength(500)
            .IsRequired();

        entity.Property(e => e.ImportedAt)
            .HasColumnName("ImportedAt")
            .IsRequired();

        entity.HasOne(d => d.CompteBancaire)
            .WithMany(p => p.BankTransactionImport)
            .HasForeignKey(d => d.CompteBancaireId)
            .OnDelete(DeleteBehavior.Restrict);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<BankTransactionImport> entity);
}
