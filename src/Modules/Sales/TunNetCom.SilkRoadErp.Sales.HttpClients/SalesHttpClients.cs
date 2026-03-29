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
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.CompteBancaire;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.BankTransactions;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Soldes;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Tags;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.InstallationTechnician;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AuditLogs;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Users;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PrintHistory;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProductFamilies;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProductSubFamilies;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceClient;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceFactureDepense;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Notifications;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryCar;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetourMarchandiseFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.TiersDepenseFonctionnement;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.FactureDepense;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementTiersDepense;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Tiers;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients;

public static class SalesHttpClients
{
    /// <summary>
    /// Registers all Sales API typed clients using IHttpClientFactory best practices.
    /// Each client is registered via AddHttpClient so that DelegatingHandlers
    /// (e.g. AuthHttpClientHandler, TenantDelegatingHandler) attached via
    /// <paramref name="configureBuilder"/> are properly resolved from the circuit scope.
    /// </summary>
    public static void AddSalesHttpClients(
        this IServiceCollection services,
        string baseUrl,
        Action<IHttpClientBuilder>? configureBuilder = null)
    {
        // Local helper: registers one typed client and applies the shared builder config
        void AddClient<TInterface, TImplementation>(string? baseAddress = null)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            var builder = services.AddHttpClient<TInterface, TImplementation>(client =>
            {
                client.BaseAddress = new Uri(baseAddress ?? baseUrl);
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            configureBuilder?.Invoke(builder);
        }

        AddClient<ICustomersApiClient, CustomersApiClient>();
        AddClient<IDeliveryNoteApiClient, DeliveryNoteApiClient>();
        AddClient<IInvoicesApiClient, InvoicesApiClient>();
        AddClient<IInventaireApiClient, InventaireApiClient>();
        AddClient<IProductsApiClient, ProductsApiClient>();
        AddClient<IReceiptNoteApiClient, ReceiptNoteApiClient>();
        AddClient<IProviderInvoiceApiClient, ProviderInvoiceApiClient>();
        AddClient<IOrderApiClient, OrderApiClient>();
        AddClient<IAccountingYearApiClient, AccountingYearApiClient>();
        AddClient<IQuotationApiClient, QuotationApiClient>();
        AddClient<IAvoirsApiClient, AvoirsApiClient>();
        AddClient<IAvoirFournisseurApiClient, AvoirFournisseurApiClient>();
        AddClient<IFactureAvoirFournisseurApiClient, FactureAvoirFournisseurApiClient>();
        AddClient<IAvoirFinancierFournisseursApiClient, AvoirFinancierFournisseursApiClient>();
        AddClient<IPaiementClientApiClient, PaiementClientApiClient>();
        AddClient<IPaiementFournisseurApiClient, PaiementFournisseurApiClient>();
        AddClient<IBanqueApiClient, BanqueApiClient>();
        AddClient<ICompteBancaireApiClient, CompteBancaireApiClient>();
        AddClient<IBankTransactionsApiClient, BankTransactionsApiClient>();
        AddClient<ISoldesApiClient, SoldesApiClient>();
        AddClient<ITagsApiClient, TagsApiClient>();
        AddClient<IInstallationTechnicianApiClient, InstallationTechnicianApiClient>();
        AddClient<IAuditLogsClient, AuditLogsClient>();
        AddClient<IUsersClient, UsersClient>();
        AddClient<IPrintHistoryClient, PrintHistoryClient>();
        AddClient<IProductFamiliesApiClient, ProductFamiliesApiClient>();
        AddClient<IProductSubFamiliesApiClient, ProductSubFamiliesApiClient>();
        AddClient<IRetenueSourceClientApiClient, RetenueSourceClientApiClient>();
        AddClient<IRetenueSourceFournisseurApiClient, RetenueSourceFournisseurApiClient>();
        AddClient<IRetenueSourceFactureDepenseApiClient, RetenueSourceFactureDepenseApiClient>();
        AddClient<INotificationApiClient, NotificationApiClient>();
        AddClient<IDeliveryCarApiClient, DeliveryCarApiClient>();
        AddClient<IRetourMarchandiseFournisseurApiClient, RetourMarchandiseFournisseurApiClient>();
        AddClient<ITiersDepenseFonctionnementApiClient, TiersDepenseFonctionnementApiClient>();
        AddClient<IFactureDepenseApiClient, FactureDepenseApiClient>();
        AddClient<IPaiementTiersDepenseApiClient, PaiementTiersDepenseApiClient>();
        AddClient<ITiersApiClient, TiersApiClient>();

        // ProvidersApiClient uses a slightly different base path
        AddClient<IProvidersApiClient, ProvidersApiClient>($"{baseUrl}/providers/");

        // AppParametersClient uses baseUrl + trailing slash
        AddClient<IAppParametersClient, AppParametersClient>($"{baseUrl}/");
    }
}