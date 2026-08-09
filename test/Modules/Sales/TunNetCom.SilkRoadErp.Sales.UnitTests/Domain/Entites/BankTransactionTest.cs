using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class BankTransactionTest
{
    [Fact]
    public void CreateBankTransaction_ShouldSetProperties()
    {
        var dateOperation = new DateTime(2024, 1, 15, 9, 30, 0);
        var dateValeur = new DateTime(2024, 1, 16, 0, 0, 0);

        var transaction = BankTransaction.CreateBankTransaction(
            bankTransactionImportId: 5,
            dateOperation: dateOperation,
            dateValeur: dateValeur,
            operation: "Retrait",
            reference: "REF-123",
            debit: 100.50m,
            credit: 0m);

        transaction.BankTransactionImportId.Should().Be(5);
        transaction.DateOperation.Should().Be(dateOperation);
        transaction.DateValeur.Should().Be(dateValeur);
        transaction.Operation.Should().Be("Retrait");
        transaction.Reference.Should().Be("REF-123");
        transaction.Debit.Should().Be(100.50m);
        transaction.Credit.Should().Be(0m);
        transaction.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void CreateBankTransaction_WithCredit_ShouldSetCredit()
    {
        var transaction = BankTransaction.CreateBankTransaction(
            bankTransactionImportId: 1,
            dateOperation: DateTime.Now,
            dateValeur: DateTime.Now,
            operation: "Versement",
            reference: "REF-456",
            debit: 0m,
            credit: 250.75m);

        transaction.Debit.Should().Be(0m);
        transaction.Credit.Should().Be(250.75m);
        transaction.SageCompteGeneral.Should().BeNull();
        transaction.SageLibelle.Should().BeNull();
    }
}
