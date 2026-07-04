namespace TunNetCom.SilkRoadErp.Sales.UiTests.Infrastructure;

/// <summary>
/// Single collection for all UI tests: they share one running stack (fixture)
/// and run sequentially to avoid interfering with each other's data.
/// </summary>
[CollectionDefinition(Name)]
public sealed class UiTestsCollection : ICollectionFixture<UiTestsFixture>
{
    public const string Name = "UI Tests";
}
