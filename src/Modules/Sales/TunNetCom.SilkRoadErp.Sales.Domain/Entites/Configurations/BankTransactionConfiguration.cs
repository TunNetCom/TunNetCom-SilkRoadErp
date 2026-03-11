using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class BankTransactionConfiguration : IEntityTypeConfiguration<BankTransaction>
{
    public void Configure(EntityTypeBuilder<BankTransaction> entity)
    {
        entity.ToTable("BankTransaction");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.BankTransactionImportId)
            .HasColumnName("BankTransactionImportId")
            .IsRequired();

        entity.Property(e => e.DateOperation)
            .HasColumnName("DateOperation")
            .IsRequired();

        entity.Property(e => e.DateValeur)
            .HasColumnName("DateValeur")
            .IsRequired();

        entity.Property(e => e.Operation)
            .HasColumnName("Operation")
            .HasMaxLength(500)
            .IsRequired();

        entity.Property(e => e.Reference)
            .HasColumnName("Reference")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(e => e.Debit)
            .HasColumnName("Debit")
            .HasPrecision(18, 4)
            .IsRequired();

        entity.Property(e => e.Credit)
            .HasColumnName("Credit")
            .HasPrecision(18, 4)
            .IsRequired();

        entity.Property(e => e.SageCompteGeneral)
            .HasColumnName("SageCompteGeneral")
            .HasMaxLength(20);

        entity.Property(e => e.SageLibelle)
            .HasColumnName("SageLibelle")
            .HasMaxLength(100);

        entity.HasOne(d => d.BankTransactionImport)
            .WithMany(p => p.BankTransaction)
            .HasForeignKey(d => d.BankTransactionImportId)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<BankTransaction> entity);
}
