using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;
using Testcontainers.MsSql;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.DataSeeder;
using TunNetCom.SilkRoadErp.Sales.UiTests.Pages;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services;

namespace TunNetCom.SilkRoadErp.Sales.UiTests.Infrastructure;

/// <summary>
/// Boots the full stack once per test run:
/// SQL Server (Testcontainers) → Sales API (Kestrel :5801) → Sales WebApp (Kestrel :5802) → Playwright Chromium.
/// The API applies EF migrations and runs <see cref="DatabaseSeeder"/> on startup,
/// which seeds the default admin user used by the tests.
/// </summary>
public sealed class UiTestsFixture : IAsyncLifetime
{
    // Seeded by DatabaseSeeder in Sales.Api — keep in sync if it changes there.
    public const string AdminUsername = "admin";
    public const string AdminPassword = "Admin123!";

    // Fixed ports, chosen to avoid the WebApp's 5005/5006 fallback in Program.cs.
    private const int ApiPort = 5801;
    private const int WebAppPort = 5802;

    public string WebAppBaseUrl => $"http://localhost:{WebAppPort}";
    private static string ApiBaseUrl => $"http://localhost:{ApiPort}";

    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong@Pass1")
        .Build();

    private ApiFactory? _apiFactory;
    private WebAppFactory? _webAppFactory;
    private IPlaywright? _playwright;

    public IBrowser Browser { get; private set; } = default!;

    /// <summary>Connection string of the throwaway SQL container — use it to seed test data via SalesContext.</summary>
    public string ConnectionString => _sqlContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        // 1. API first: its startup applies migrations and seeds the database (roles, permissions, admin user).
        _apiFactory = new ApiFactory(ConnectionString);
        _apiFactory.UseKestrel(ApiPort);
        _apiFactory.StartServer();

        // 2. WebApp, pointed at the API. Program.cs falls back to https://5005 when
        //    ASPNETCORE_URLS is empty, so pin the URL for the test host explicitly.
        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", $"http://localhost:{WebAppPort}");
        _webAppFactory = new WebAppFactory(ApiBaseUrl);
        _webAppFactory.UseKestrel(WebAppPort);
        _webAppFactory.StartServer();

        // 3. Playwright. Installs Chromium if missing (no-op when already cached).
        var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);
        if (exitCode != 0)
        {
            throw new InvalidOperationException($"'playwright install chromium' failed with exit code {exitCode}.");
        }

        _playwright = await Playwright.CreateAsync();

        // Set UI_TESTS_HEADED=1 to watch the browser locally.
        // When headed, actions are slowed down so a human can follow them;
        // override the delay (ms) with UI_TESTS_SLOWMO. Headless runs stay at full speed.
        var headed = Environment.GetEnvironmentVariable("UI_TESTS_HEADED") == "1";
        var slowMo = float.TryParse(Environment.GetEnvironmentVariable("UI_TESTS_SLOWMO"), out var ms)
            ? ms
            : headed ? 500f : 0f;

        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = !headed,
            SlowMo = slowMo
        });
    }

    /// <summary>Creates an isolated browser context (own cookies/session) and returns a page rooted at the WebApp.</summary>
    public async Task<IPage> NewPageAsync()
    {
        var context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = WebAppBaseUrl,
            IgnoreHTTPSErrors = true
        });

        return await context.NewPageAsync();
    }

    /// <summary>Creates an isolated context and logs in as the seeded admin. The returned page is on the home page.</summary>
    public async Task<IPage> NewAuthenticatedPageAsync()
    {
        var page = await NewPageAsync();
        var loginPage = new LoginPage(page);

        await loginPage.GotoAsync();
        await loginPage.LoginAsync(AdminUsername, AdminPassword);
        await page.WaitForURLAsync(
            url => !url.Contains("/login"),
            new PageWaitForURLOptions { Timeout = 30_000 });

        return page;
    }

    public async Task DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.DisposeAsync();
        }

        _playwright?.Dispose();

        if (_webAppFactory is not null)
        {
            await _webAppFactory.DisposeAsync();
        }

        if (_apiFactory is not null)
        {
            await _apiFactory.DisposeAsync();
        }

        await _sqlContainer.DisposeAsync();
    }

    // Both app assemblies have an internal top-level Program class, and referencing both would make
    // 'Program' ambiguous anyway — so each factory uses a public type from its assembly as marker.

    private sealed class ApiFactory(string connectionString) : WebApplicationFactory<DatabaseSeeder>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("ConnectionStrings:DefaultConnection", connectionString);
        }
    }

    private sealed class WebAppFactory(string apiBaseUrl) : WebApplicationFactory<AuthService>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("ApiSettings:BaseUrl", apiBaseUrl);
            builder.UseSetting("Deployment:Mode", "Standalone");
        }
    }
}
