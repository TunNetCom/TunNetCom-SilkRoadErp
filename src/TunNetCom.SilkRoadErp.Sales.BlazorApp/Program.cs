using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.BlazorApp.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

var builder = WebApplication.CreateBuilder(args);



// Ajouter les services à votre conteneur
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllersWithViews();

// Register your DbContext
builder.Services.AddDbContext<SalesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register your services
builder.Services.AddScoped<ClientService>();

// Lire la configuration pour l'URL de base
var baseUrl = builder.Configuration.GetValue<string>("BaseUrl");

// Configurer HttpClient avec l'URL de base lue depuis la configuration
builder.Services.AddHttpClient<ClientService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
