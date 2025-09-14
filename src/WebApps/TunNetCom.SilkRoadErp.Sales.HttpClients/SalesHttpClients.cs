using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Orders;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Products;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ReceiptNote;


public static class SalesHttpClients
{
    /// <summary>
    /// setting up communication with api backend
    /// this is a method extension  that adds HTTP clients to your app’s dependency injection (DI) system
    /// It’s called in Program.cs of your WebApp project to configure how your Blazor app talks to the API.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="baseUrl"></param>
    public static void AddSalesHttpClients(this IServiceCollection services, string baseUrl)
    {
        _ = services.AddHttpClient<ICustomersApiClient, CustomersApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        _ = services.AddHttpClient<IDeliveryNoteApiClient, DeliveryNoteApiClient>(deliverynote =>
        {
            deliverynote.BaseAddress = new Uri(baseUrl);
        });

        _ = services.AddHttpClient<IInvoicesApiClient, InvoicesApiClient>(invoice =>
        {
            invoice.BaseAddress = new Uri(baseUrl);
        });

        _ = services.AddHttpClient<IProductsApiClient, ProductsApiClient>(product =>
        {
            product.BaseAddress = new Uri(baseUrl);
        });

        _ = services.AddHttpClient<IProvidersApiClient, ProvidersApiClient>(provider =>
        {
            provider.BaseAddress = new Uri($"{baseUrl}/providers/");
        });

        _ = services.AddHttpClient<IAppParametersClient, AppParametersClient>(provider =>
        {
            provider.BaseAddress = new Uri($"{baseUrl}/");
        });
        _ = services.AddHttpClient<IReceiptNoteApiClient, ReceiptNoteApiClient>(receipt =>
        {
            receipt.BaseAddress = new Uri(baseUrl);
        });
        _ = services.AddHttpClient<IProviderInvoiceApiClient, ProviderInvoiceApiClient>(invoice =>
        {
            invoice.BaseAddress = new Uri(baseUrl);
        });
        _ = services.AddHttpClient<IOrderApiClient, OrderApiClient>(order =>
        {
            order.BaseAddress = new Uri(baseUrl);
        });
    }
}