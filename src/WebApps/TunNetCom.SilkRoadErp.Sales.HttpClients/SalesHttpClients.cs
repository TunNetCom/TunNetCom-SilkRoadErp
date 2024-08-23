using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Products;
using Microsoft.Extensions.DependencyInjection;

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