using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class BankTransactionImportTest
{
    [Fact]
    public void CreateBankTransactionImport_ShouldSetProperties()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var import = BankTransactionImport.CreateBankTransactionImport(
            compteBancaireId: 9,
            fileName: "releve_janvier.csv");

        import.CompteBancaireId.Should().Be(9);
        import.FileName.Should().Be("releve_janvier.csv");
        import.ImportedAt.Should().BeAfter(before);
        import.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        import.BankTransaction.Should().BeEmpty();
    }
}
