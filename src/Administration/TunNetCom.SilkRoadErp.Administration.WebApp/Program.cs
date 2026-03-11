using Radzen;
using TunNetCom.SilkRoadErp.Administration.HttpClients;
using TunNetCom.SilkRoadErp.Administration.WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
