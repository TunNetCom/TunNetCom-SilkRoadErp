using RadzenBlazorDemos.Services;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var baseUrl = builder.Configuration.GetValue<string>("BaseUrl")
    ?? throw new ArgumentNullException("Sales base url was null!");

builder.Services.AddSalesHttpClients(baseUrl);

// Add OData service
builder.Services.AddHttpClient<ODataService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddPrintEngine(builder.Configuration);
builder.Services.AddLocalization();
builder.Services.AddControllers();
builder.Services.AddScoped<GitHubService>();
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

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();