using TunNetCom.SilkRoadErp.Sales.WebApp.Components;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services.DeliveryNote;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var baseUrl = builder.Configuration.GetValue<string>("BaseUrl");

builder.Services.AddHttpClient<CustomerService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<DeliveryNoteService>(deliveryNote =>
{
    deliveryNote.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<ICustomersApiClient, CustomersApiClient>(client =>
{
    client.BaseAddress = new Uri($"{baseUrl}/customers/");
});

builder.Services.AddLocalization();
builder.Services.AddControllers();
string[] supportedCultures = ["en", "fr"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

builder.Services.AddBlazorBootstrap();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
