using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.BlazorApp.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services à votre conteneur
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson();

// Register your services
builder.Services.AddScoped<ClientService>();

// Add Localization
builder.Services.AddLocalization();
builder.Services.AddControllers();

// Configure supported cultures
string[] supportedCultures = ["en", "fr"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);


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


// Use location configuration
app.UseRequestLocalization(localizationOptions);

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
