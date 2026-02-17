using Microsoft.AspNetCore.Components.Authorization;
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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var razorBuilder = builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Blazor Server/Interactive Server Components use SignalR under the hood.
// Default max incoming message size is 32KB, which can cancel JS interop calls
// when returning base64 payloads (camera capture).
builder.Services.Configure<Microsoft.AspNetCore.SignalR.HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 8 * 1024 * 1024; // 8MB
});

// Configure circuit options
if (builder.Environment.IsDevelopment())
{
    razorBuilder.AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });
}

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

// Add Sales HttpClients (this registers all clients with their implementations)
// Then configure them with the auth handler
builder.Services.AddSalesHttpClients(baseUrl, builder => builder.AddHttpMessageHandler<AuthHttpClientHandler>());

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

app.Run();
