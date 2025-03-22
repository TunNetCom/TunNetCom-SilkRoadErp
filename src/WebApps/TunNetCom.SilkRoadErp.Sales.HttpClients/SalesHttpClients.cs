using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Products;
using Microsoft.Extensions.DependencyInjection;

//setting up communication with api backend
//this is a method extension  that adds HTTP clients to your app’s dependency injection (DI) system
//It’s called in Program.cs of your WebApp project to configure how your Blazor app talks to the API.

public static class SalesHttpClients
{
    public static void AddSalesHttpClients(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient<ICustomersApiClient, CustomersApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient<IDeliveryNoteApiClient, DeliveryNoteApiClient>(deliverynote =>
        {
            deliverynote.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient<IInvoicesApiClient, InvoicesApiClient>(invoice =>
        {
            invoice.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient<IProductsApiClient, ProductsApiClient>(product =>
        {
            product.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient<IProvidersApiClient, ProvidersApiClient>(provider =>
        {
            provider.BaseAddress = new Uri($"{baseUrl}/providers/");
        });
    }
}