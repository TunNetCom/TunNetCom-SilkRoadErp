using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class PrintHistoryTest
{
    [Fact]
    public void Create_ShouldSetDefaults()
    {
        var print = PrintHistory.Create(
            documentType: "Facture",
            documentId: 1,
            printMode: PrintModeEnum.Download,
            userId: 5,
            username: "admin");

        print.DocumentType.Should().Be("Facture");
        print.DocumentId.Should().Be(1);
        print.PrintMode.Should().Be(PrintModeEnum.Download);
        print.UserId.Should().Be(5);
        print.Username.Should().Be("admin");
        print.PrinterName.Should().BeNull();
        print.Copies.Should().Be(1);
        print.FileName.Should().BeNull();
        print.IsDuplicata.Should().BeFalse();
        print.PrintedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        print.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void Create_WhenUsernameNull_ShouldDefaultToSystem()
    {
        var print = PrintHistory.Create(
            documentType: "Facture",
            documentId: 2,
            printMode: PrintModeEnum.DirectPrint,
            userId: null,
            username: null);

        print.Username.Should().Be("System");
        print.UserId.Should().BeNull();
        print.PrintMode.Should().Be(PrintModeEnum.DirectPrint);
    }

    [Fact]
    public void Create_WithAllOptions_ShouldSetThem()
    {
        var print = PrintHistory.Create(
            documentType: "Facture",
            documentId: 3,
            printMode: PrintModeEnum.Download,
            userId: 1,
            username: "admin",
            printerName: "Printer A",
            copies: 2,
            fileName: "facture.pdf",
            isDuplicata: true);

        print.PrinterName.Should().Be("Printer A");
        print.Copies.Should().Be(2);
        print.FileName.Should().Be("facture.pdf");
        print.IsDuplicata.Should().BeTrue();
    }

    [Fact]
    public void SetId_ShouldSetId()
    {
        var print = PrintHistory.Create("Facture", 1, PrintModeEnum.Download, null, "admin");

        print.SetId(10);

        print.Id.Should().Be(10);
    }
}
