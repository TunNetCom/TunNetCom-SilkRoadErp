using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Testcontainers.MsSql;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Authorization;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.FunctionalTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _container;

    public string ConnectionString => _container.GetConnectionString();

    public CustomWebApplicationFactory()
    {
        _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Strong@Pass1")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await ApplyMigrationsAsync();
        SeedData();
    }

    private async Task ApplyMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseSqlServer(ConnectionString,
                sqlServerOptions => sqlServerOptions.MigrationsAssembly("TunNetCom.SilkRoadErp.Sales.Domain"))
            .Options;
        await using var context = new SalesContext(options);
        await context.Database.MigrateAsync();
    }

    private void SeedData()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseSqlServer(ConnectionString,
                sqlServerOptions => sqlServerOptions.MigrationsAssembly("TunNetCom.SilkRoadErp.Sales.Domain"))
            .Options;
        using var context = new SalesContext(options);

        if (!context.Systeme.Any())
        {
            _ = context.Systeme.Add(new Systeme
            {
                NomSociete = "Test",
                Adresse = "Test",
                Tel = "123",
                CodeTva = "TVA",
                BloquerVenteStockInsuffisant = false,
                BloquerBlSansFacture = false
            });
        }

        if (!context.AccountingYear.Any())
        {
            var year = AccountingYear.CreateAccountingYear(2024, isActive: true);
            _ = context.AccountingYear.Add(year);
        }

        if (!context.Produit.Any())
        {
            context.Produit.AddRange(new[] {
                new Produit(
                    refe: "REF1",
                    nom: "Product 1",
                    qteLimite: 0,
                    remise: 0,
                    remiseAchat: 0,
                    tva: 19,
                    prix: 50m,
                    prixAchat: 0,
                    visibilite: true),
                new Produit(
                    refe: "REF2",
                    nom: "Product 2",
                    qteLimite: 0,
                    remise: 0,
                    remiseAchat: 0,
                    tva: 19,
                    prix: 25m,
                    prixAchat: 0,
                    visibilite: true),
                new Produit(
                    refe: "REF3",
                    nom: "Product 3",
                    qteLimite: 0,
                    remise: 0,
                    remiseAchat: 0,
                    tva: 19,
                    prix: 20m,
                    prixAchat: 0,
                    visibilite: true)
            });
        }

        _ = context.SaveChanges();
    }
  

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder.UseSetting("ConnectionStrings:DefaultConnection", ConnectionString);

        _ = builder.ConfigureServices(services =>
        {
            _ = services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, null);

            _ = services.RemoveAll<IAuthorizationHandler>();
            _ = services.AddSingleton<IAuthorizationHandler, TestPermissionHandler>();

            _ = services.RemoveAll<INumberGeneratorService>();
            var numberGeneratorMock = new Mock<INumberGeneratorService>();
            _ = numberGeneratorMock
                .Setup(x => x.GenerateBonDeLivraisonNumberAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _ = services.AddScoped<INumberGeneratorService>(_ => numberGeneratorMock.Object);
        });
    }

    public new async Task DisposeAsync() => await _container.DisposeAsync();
}
