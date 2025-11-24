using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AccountingYear;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Avoirs;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AvoirFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.FactureAvoirFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Inventaire;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Orders;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Products;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Quotations;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ReceiptNote;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementClient;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Banque;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Soldes;


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

        _ = services.AddHttpClient<IInventaireApiClient, InventaireApiClient>(inventaire =>
        {
            inventaire.BaseAddress = new Uri(baseUrl);
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
        _ = services.AddHttpClient<IAccountingYearApiClient, AccountingYearApiClient>(accountingYear =>
        {
            accountingYear.BaseAddress = new Uri(baseUrl);
        });
        _ = services.AddHttpClient<IQuotationApiClient, QuotationApiClient>(quotation =>
        {
            quotation.BaseAddress = new Uri(baseUrl);
        });
        _ = services.AddHttpClient<IAvoirsApiClient, AvoirsApiClient>(avoir =>
        {
            avoir.BaseAddress = new Uri(baseUrl);
        });
        _ = services.AddHttpClient<IAvoirFournisseurApiClient, AvoirFournisseurApiClient>(avoirFournisseur =>
        {
            avoirFournisseur.BaseAddress = new Uri(baseUrl);
        });
        _ = services.AddHttpClient<IFactureAvoirFournisseurApiClient, FactureAvoirFournisseurApiClient>(factureAvoirFournisseur =>
        {
            factureAvoirFournisseur.BaseAddress = new Uri(baseUrl);
        });
        _ = services.AddHttpClient<IPaiementClientApiClient, PaiementClientApiClient>(paiementClient =>
        {
            paiementClient.BaseAddress = new Uri(baseUrl);
        });
        _ = services.AddHttpClient<IPaiementFournisseurApiClient, PaiementFournisseurApiClient>(paiementFournisseur =>
        {
            paiementFournisseur.BaseAddress = new Uri(baseUrl);
        });
        _ = services.AddHttpClient<IBanqueApiClient, BanqueApiClient>(banque =>
        {
            banque.BaseAddress = new Uri(baseUrl);
        });
        _ = services.AddHttpClient<ISoldesApiClient, SoldesApiClient>(soldes =>
        {
            soldes.BaseAddress = new Uri(baseUrl);
        });
    }
}