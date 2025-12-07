using Microsoft.AspNetCore.Components.Authorization;
using RadzenBlazorDemos.Services;
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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var razorBuilder = builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

// Add HttpClient for auth endpoints
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddPrintEngine(builder.Configuration);
builder.Services.AddLocalization();
builder.Services.AddControllers();
builder.Services.AddScoped<GitHubService>();
builder.Services.AddScoped<IDecimalFormatService, DecimalFormatService>();
builder.Services.AddScoped<ISilkRoadNotificationService, TunNetCom.SilkRoadErp.Sales.WebApp.Services.SilkRoadNotificationService>();
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