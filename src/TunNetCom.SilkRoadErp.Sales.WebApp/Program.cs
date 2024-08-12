using TunNetCom.SilkRoadErp.Sales.WebApp.Components;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services.DeliveryNote;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services.Invoice;
using TunNetCom.SilkRoadErp.Sales.WebApp.Services.Product;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var baseUrl = builder.Configuration.GetValue<string>("BaseUrl");
builder.Services.AddHttpClient<CustomersApiClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<DeliveryNoteApiClient>(deliverynote =>
{
    deliverynote.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<InvoicesApiClient>(invoice =>
{
    invoice.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<ProductsApiClient>(product =>
{
    product.BaseAddress = new Uri(baseUrl);
});



builder.Services.AddHttpClient<IProvidersApiClient, ProvidersApiClient>(provider =>
{
    provider.BaseAddress = new Uri($"{baseUrl}/providers/");
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
