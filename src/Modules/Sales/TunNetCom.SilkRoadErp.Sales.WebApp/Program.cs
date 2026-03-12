using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Inventaire;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Products;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ReceiptNote;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Orders;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AccountingYear;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Quotations;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Avoirs;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AvoirFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.FactureAvoirFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementClient;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Banque;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Soldes;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Tags;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services.Recap;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services.Dashboard;

// Prefer Aspire-assigned URLs over config so we don't force 5005/5006 when running under AppHost (avoids port conflict / slow startup then shutdown)
var aspireUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (string.IsNullOrEmpty(aspireUrls) || aspireUrls.Contains("5001"))
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://localhost:5005;http://localhost:5006");

var builder = WebApplication.CreateBuilder(args);

var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? builder.Configuration["Urls"] ?? "";
if (string.IsNullOrWhiteSpace(urls) || urls.Contains("5001"))
    urls = "https://localhost:5005;http://localhost:5006";
builder.WebHost.UseUrls(urls);

builder.AddServiceDefaults();

// Add services to the container.
var razorBuilder = builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Blazor Server/Interactive Server Components use SignalR under the hood.
// Default max incoming message size is 32KB, which can cancel JS interop calls
// when returning base64 payloads (camera capture).
builder.Services.Configure<Microsoft.AspNetCore.SignalR.HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 8 * 1024 * 1024; // 8MB
    // SignalR defaults: KeepAliveInterval 15s, ClientTimeoutInterval 30s.
    // If GET /accountingYear/active is called every 1-2s, circuits may be reconnecting.
    // Ensure load balancer/reverse proxy idle timeout > ClientTimeoutInterval (e.g. 60s+).
});

// Configure circuit options (all environments: retention; development: detailed errors)
// DisconnectedCircuitRetentionPeriod: how long disconnected circuit state is retained (default 3 min). Allows reconnect without full re-init.
razorBuilder.AddCircuitOptions(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
    options.DisconnectedCircuitMaxRetained = 100;
    if (builder.Environment.IsDevelopment())
        options.DetailedErrors = true;
});

var baseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl")
    ?? throw new ArgumentNullException("Sales base url was null!");

// Add HttpContextAccessor for circuit tracking
builder.Services.AddHttpContextAccessor();

// Add Session support (required for stable circuit IDs)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4); // Hard-coded to 4 hours
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add token storage services
builder.Services.AddSingleton<ITokenStore, TokenStore>();
builder.Services.AddScoped<ICircuitIdService, CircuitIdService>();

// Add Auth Service FIRST (before HttpClients that depend on it)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddSingleton<ITokenExpirationNotifier, TokenExpirationNotifier>();
builder.Services.AddScoped<IAutoLogoutService, AutoLogoutService>();

// Add AuthenticationStateProvider for Blazor authorization
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddCascadingAuthenticationState();

// Register AuthHttpClientHandler as scoped (required for IJSRuntime)
builder.Services.AddScoped<AuthHttpClientHandler>();

builder.Services.Configure<DeploymentOptions>(builder.Configuration.GetSection(DeploymentOptions.SectionName));
var deploymentMode = builder.Configuration.GetValue<DeploymentMode>("Deployment:Mode");

if (deploymentMode == DeploymentMode.MultiTenant)
{
    builder.Services.AddScoped<ITenantContext, BlazorTenantContext>();
    builder.Services.AddScoped<TenantDelegatingHandler>();
}
else
{
    builder.Services.AddSingleton<ITenantContext>(sp =>
    {
        var options = sp.GetRequiredService<IOptions<DeploymentOptions>>();
        return new BlazorTenantContext(
            sp.GetRequiredService<IHttpContextAccessor>(),
            options);
    });
}

// Add Sales HttpClients (this registers all clients with their implementations)
// Then configure them with the auth handler
builder.Services.AddSalesHttpClients(baseUrl, httpClientBuilder =>
{
    httpClientBuilder.AddHttpMessageHandler<AuthHttpClientHandler>();
    if (deploymentMode == DeploymentMode.MultiTenant)
    {
        httpClientBuilder.AddHttpMessageHandler<TenantDelegatingHandler>();
    }
});

// Add OData service with auth handler
builder.Services.AddHttpClient<ODataService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromMinutes(5); // Increase timeout for large requests
})
.AddHttpMessageHandler<AuthHttpClientHandler>();

// Recap Ventes/Achats service (calls /api/invoices/totals, /api/provider-invoices/totals + API clients)
builder.Services.AddHttpClient<IRecapVentesAchatsService, RecapVentesAchatsService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
})
.AddHttpMessageHandler<AuthHttpClientHandler>();

// Dashboard evolution (ventes/achats by month for chart)
builder.Services.AddHttpClient<IDashboardEvolutionService, DashboardEvolutionService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
})
.AddHttpMessageHandler<AuthHttpClientHandler>();

// Add HttpClient for auth endpoints
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

// Add named HttpClient for raw API calls (totals, export TEJ, etc.) with auth
builder.Services.AddHttpClient("SalesApi", client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromMinutes(2);
}).AddHttpMessageHandler<AuthHttpClientHandler>();

builder.Services.AddPrintEngine(builder.Configuration);
builder.Services.AddLocalization();
builder.Services.AddControllers();
builder.Services.AddScoped<IDecimalFormatService, DecimalFormatService>();
builder.Services.AddScoped<ISilkRoadNotificationService, TunNetCom.SilkRoadErp.Sales.WebApp.Services.SilkRoadNotificationService>();
builder.Services.AddScoped<ICurrentProductStateService, CurrentProductStateService>();
builder.Services.AddScoped<ICurrentProductCalculationService, CurrentProductCalculationService>();
string[] supportedCultures = ["en", "fr", "ar"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("fr")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

builder.Services.AddBlazorBootstrap();
builder.Services.AddRadzenComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error", createScopeForErrors: true);
    _ = app.UseHsts();
}

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

// Enable session middleware (must be before MapRazorComponents)
app.UseSession();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
