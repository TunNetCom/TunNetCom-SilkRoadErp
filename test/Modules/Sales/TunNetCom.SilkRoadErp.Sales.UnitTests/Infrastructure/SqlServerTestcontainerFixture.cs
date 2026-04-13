using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using Testcontainers.MsSql;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure;

/// <summary>
/// xUnit class fixture that starts a SQL Server Testcontainer and runs EF Core migrations.
/// Use with IClassFixture&lt;SqlServerTestcontainerFixture&gt; and IAsyncLifetime on the test class.
/// </summary>
public sealed class SqlServerTestcontainerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong@Pass1")
        .Build();

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();
        await ApplyMigrationsAsync();
    }

    public SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseSqlServer(
                ConnectionString,
                sqlServerOptions => sqlServerOptions.MigrationsAssembly("TunNetCom.SilkRoadErp.Sales.Domain"))
            .Options;
        return new SalesContext(options);
    }

    private async Task ApplyMigrationsAsync()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();
}
