using System;
using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AccountingYear;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Avoirs;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AvoirFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AvoirFinancierFournisseurs;
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
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Tags;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.InstallationTechnician;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AuditLogs;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Users;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PrintHistory;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProductFamilies;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProductSubFamilies;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceClient;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Notifications;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryCar;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetourMarchandiseFournisseur;


public static class SalesHttpClients
{
    /// <summary>
    /// setting up communication with api backend
    /// this is a method extension  that adds HTTP clients to your app's dependency injection (DI) system
    /// It's called in Program.cs of your WebApp project to configure how your Blazor app talks to the API.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="baseUrl"></param>
    public static void AddSalesHttpClients(this IServiceCollection services, string baseUrl, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        // Helper to configure client with optional builder configuration
        IHttpClientBuilder AddClient<TInterface, TImplementation>(Action<HttpClient> configureClient) 
            where TInterface : class 
            where TImplementation : class, TInterface
        {
            var builder = services.AddHttpClient<TInterface, TImplementation>(configureClient);
            // Configure timeout for large requests (images, etc.) - 5 minutes
            builder.ConfigureHttpClient(client => client.Timeout = TimeSpan.FromMinutes(5));
            configureBuilder?.Invoke(builder);
            return builder;
        }
        
        _ = AddClient<ICustomersApiClient, CustomersApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        _ = AddClient<IDeliveryNoteApiClient, DeliveryNoteApiClient>(deliverynote =>
        {
            deliverynote.BaseAddress = new Uri(baseUrl);
        });

        _ = AddClient<IInvoicesApiClient, InvoicesApiClient>(invoice =>
        {
            invoice.BaseAddress = new Uri(baseUrl);
        });

        _ = AddClient<IInventaireApiClient, InventaireApiClient>(inventaire =>
        {
            inventaire.BaseAddress = new Uri(baseUrl);
        });

        _ = AddClient<IProductsApiClient, ProductsApiClient>(product =>
        {
            product.BaseAddress = new Uri(baseUrl);
        });

        _ = AddClient<IProvidersApiClient, ProvidersApiClient>(provider =>
        {
            provider.BaseAddress = new Uri($"{baseUrl}/providers/");
        });

        _ = AddClient<IAppParametersClient, AppParametersClient>(provider =>
        {
            provider.BaseAddress = new Uri($"{baseUrl}/");
        });
        _ = AddClient<IReceiptNoteApiClient, ReceiptNoteApiClient>(receipt =>
        {
            receipt.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IProviderInvoiceApiClient, ProviderInvoiceApiClient>(invoice =>
        {
            invoice.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IOrderApiClient, OrderApiClient>(order =>
        {
            order.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IAccountingYearApiClient, AccountingYearApiClient>(accountingYear =>
        {
            accountingYear.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IQuotationApiClient, QuotationApiClient>(quotation =>
        {
            quotation.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IAvoirsApiClient, AvoirsApiClient>(avoir =>
        {
            avoir.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IAvoirFournisseurApiClient, AvoirFournisseurApiClient>(avoirFournisseur =>
        {
            avoirFournisseur.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IFactureAvoirFournisseurApiClient, FactureAvoirFournisseurApiClient>(factureAvoirFournisseur =>
        {
            factureAvoirFournisseur.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IAvoirFinancierFournisseursApiClient, AvoirFinancierFournisseursApiClient>(avoirFinancierFournisseurs =>
        {
            avoirFinancierFournisseurs.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IPaiementClientApiClient, PaiementClientApiClient>(paiementClient =>
        {
            paiementClient.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IPaiementFournisseurApiClient, PaiementFournisseurApiClient>(paiementFournisseur =>
        {
            paiementFournisseur.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IBanqueApiClient, BanqueApiClient>(banque =>
        {
            banque.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<ISoldesApiClient, SoldesApiClient>(soldes =>
        {
            soldes.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<ITagsApiClient, TagsApiClient>(tags =>
        {
            tags.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IInstallationTechnicianApiClient, InstallationTechnicianApiClient>(installationTechnician =>
        {
            installationTechnician.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IAuditLogsClient, AuditLogsClient>(auditLogs =>
        {
            auditLogs.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IUsersClient, UsersClient>(users =>
        {
            users.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IPrintHistoryClient, PrintHistoryClient>(printHistory =>
        {
            printHistory.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IProductFamiliesApiClient, ProductFamiliesApiClient>(productFamilies =>
        {
            productFamilies.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IProductSubFamiliesApiClient, ProductSubFamiliesApiClient>(productSubFamilies =>
        {
            productSubFamilies.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IRetenueSourceClientApiClient, RetenueSourceClientApiClient>(retenueSourceClient =>
        {
            retenueSourceClient.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IRetenueSourceFournisseurApiClient, RetenueSourceFournisseurApiClient>(retenueSourceFournisseur =>
        {
            retenueSourceFournisseur.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<INotificationApiClient, NotificationApiClient>(notifications =>
        {
            notifications.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IDeliveryCarApiClient, DeliveryCarApiClient>(deliveryCar =>
        {
            deliveryCar.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IRetourMarchandiseFournisseurApiClient, RetourMarchandiseFournisseurApiClient>(retourMarchandiseFournisseur =>
        {
            retourMarchandiseFournisseur.BaseAddress = new Uri(baseUrl);
        });
    }
}