namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure;

[CollectionDefinition("SqlServerIntegration", DisableParallelization = true)]
public sealed class SqlServerIntegrationCollection : ICollectionFixture<SqlServerTestcontainerFixture>;
