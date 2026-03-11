using Radzen;
using TunNetCom.SilkRoadErp.Administration.HttpClients;
using TunNetCom.SilkRoadErp.TenantSetup.WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();
builder.Services.AddLocalization();
builder.Services.AddControllers();

var adminApiUrl = builder.Configuration["AdminApi:BaseUrl"]
    ?? "https://localhost:5001";

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

app.Run();
