using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class TagTest
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var tag = new Tag
        {
            Name = "Important",
            Color = "#ff0000",
            Description = "Tag description"
        };

        tag.Name.Should().Be("Important");
        tag.Color.Should().Be("#ff0000");
        tag.Description.Should().Be("Tag description");
        tag.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        tag.DocumentTags.Should().BeEmpty();
    }

    [Fact]
    public void Name_Default_ShouldBeEmptyString()
    {
        var tag = new Tag();

        tag.Name.Should().Be(string.Empty);
    }
}
