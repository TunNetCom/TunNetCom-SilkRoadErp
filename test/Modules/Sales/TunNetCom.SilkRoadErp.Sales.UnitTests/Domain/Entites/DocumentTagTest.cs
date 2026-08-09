using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class DocumentTagTest
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var documentTag = new DocumentTag
        {
            TagId = 1,
            DocumentType = "Facture",
            DocumentId = 2
        };

        documentTag.TagId.Should().Be(1);
        documentTag.DocumentType.Should().Be("Facture");
        documentTag.DocumentId.Should().Be(2);
        documentTag.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void DocumentType_Default_ShouldBeEmptyString()
    {
        var documentTag = new DocumentTag();

        documentTag.DocumentType.Should().Be(string.Empty);
    }
}
