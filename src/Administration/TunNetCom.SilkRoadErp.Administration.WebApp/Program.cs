using Radzen;
using TunNetCom.SilkRoadErp.Administration.HttpClients;
using TunNetCom.SilkRoadErp.Administration.WebApp.Components;

// Prefer Aspire-assigned URLs over config so we don't force 5002/5004 when running under AppHost (avoids "address already in use")
var aspireUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (string.IsNullOrEmpty(aspireUrls) || aspireUrls.Contains("5001"))
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://localhost:5002;http://localhost:5004");

var builder = WebApplication.CreateBuilder(args);

var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? builder.Configuration["Urls"] ?? "";
if (string.IsNullOrWhiteSpace(urls) || urls.Contains("5001"))
    urls = "https://localhost:5002;http://localhost:5004";
builder.WebHost.UseUrls(urls);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();
builder.Services.AddLocalization();
builder.Services.AddControllers();

var adminApiUrl = builder.Configuration["AdminApi:BaseUrl"]
    ?? "https://localhost:5011";

builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>(client =>
{
    client.BaseAddress = new Uri(adminApiUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

string[] supportedCultures = ["en", "fr", "ar"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("fr")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
