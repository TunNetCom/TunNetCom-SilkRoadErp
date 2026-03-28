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

public class DynamicAuthHandler : System.Net.Http.DelegatingHandler
{
    private readonly Action<System.Net.Http.HttpRequestMessage> _configureRequest;
    
    public DynamicAuthHandler(Action<System.Net.Http.HttpRequestMessage> configureRequest, System.Net.Http.HttpMessageHandler innerHandler) 
        : base(innerHandler)
    {
        _configureRequest = configureRequest;
    }

    protected override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
        _configureRequest(request);
        return base.SendAsync(request, cancellationToken);
    }
    
    protected override System.Net.Http.HttpResponseMessage Send(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
        _configureRequest(request);
        return base.Send(request, cancellationToken);
    }
}

public static class SalesHttpClients
{

    public static void AddSalesHttpClients(this IServiceCollection services, string baseUrl, Action<IHttpClientBuilder>? configureBuilder = null, Action<IServiceProvider, System.Net.Http.HttpRequestMessage>? configureClientAuth = null)
    {
        // Register a singleton SocketsHttpHandler for optimal connection pooling
        Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAddSingleton<System.Net.Http.SocketsHttpHandler>(services, sp => new System.Net.Http.SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2)
        });

        // Helper to configure client with optional builder configuration
        IServiceCollection AddClient<TInterface, TImplementation>(Action<HttpClient> configureClient) 
            where TInterface : class 
            where TImplementation : class, TInterface
        {
            // Map the Interface resolution directly to the Blazor Circuit Scope explicitly
            services.AddScoped<TInterface>(sp => {
                var socketsHandler = sp.GetRequiredService<System.Net.Http.SocketsHttpHandler>();
                
                // Construct a dynamic handler that evaluates the token ON EVERY REQUEST safely inside this exact scope
                Action<System.Net.Http.HttpRequestMessage> dynamicConfigurator = request => {
                    configureClientAuth?.Invoke(sp, request);
                };
                
                var dynamicHandler = new DynamicAuthHandler(dynamicConfigurator, socketsHandler);
                var client = new HttpClient(dynamicHandler, disposeHandler: false);
                client.Timeout = TimeSpan.FromMinutes(5);
                configureClient(client);
                
                // Construct the generated Refit/NSwag API client safely inside our Circuit Scope
                return Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<TImplementation>(sp, client);
            });
            return services;
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
        _ = AddClient<ICompteBancaireApiClient, CompteBancaireApiClient>(compteBancaire =>
        {
            compteBancaire.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IBankTransactionsApiClient, BankTransactionsApiClient>(bankTransactions =>
        {
            bankTransactions.BaseAddress = new Uri(baseUrl);
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
        _ = AddClient<ITiersDepenseFonctionnementApiClient, TiersDepenseFonctionnementApiClient>(tiersDepense =>
        {
            tiersDepense.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IFactureDepenseApiClient, FactureDepenseApiClient>(factureDepense =>
        {
            factureDepense.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IRetenueSourceFactureDepenseApiClient, RetenueSourceFactureDepenseApiClient>(retenueSourceFactureDepense =>
        {
            retenueSourceFactureDepense.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<IPaiementTiersDepenseApiClient, PaiementTiersDepenseApiClient>(paiementTiersDepense =>
        {
            paiementTiersDepense.BaseAddress = new Uri(baseUrl);
        });
        _ = AddClient<ITiersApiClient, TiersApiClient>(tiers =>
        {
            tiers.BaseAddress = new Uri(baseUrl);
        });
    }
}