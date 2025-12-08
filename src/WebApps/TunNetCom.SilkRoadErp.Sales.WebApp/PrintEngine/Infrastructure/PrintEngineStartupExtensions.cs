using Microsoft.JSInterop;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.PrintInvoiceWithDetails;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.ProviderInvoices;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSourceFournisseur;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.DeliveryNotes.PrintDeliveryNote;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Quotations.PrintQuotation;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Orders.PrintOrder;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes.PrintSoldeClient;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes.PrintSoldeFournisseur;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.PaiementFournisseur.PrintTraite;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PrintHistory;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public static class PrintEngineStartupExtensions
{
    public static void AddPrintEngine(this IServiceCollection services, IConfiguration? configuration = null)
    {
        _ = services.AddScoped<PrintRetenuSourceService>();
        _ = services.AddScoped<PrintFullInvoiceService>();
        _ = services.AddScoped<RetenuSourceFournisseurPrintService>();
        _ = services.AddScoped<PrintProviderFullInvoiceService>();
        _ = services.AddScoped<PrintDeliveryNoteService>();
        _ = services.AddScoped<PrintQuotationService>();
        _ = services.AddScoped<PrintOrderService>();
        _ = services.AddScoped<PrintSoldeClientService>();
        _ = services.AddScoped<PrintSoldeFournisseurService>();
        _ = services.AddScoped<PrintTraiteService>();
        _ = services.AddScoped(typeof(IPrintPdfService<,>), typeof(PrintPdfPlayWrightService<,>));

        // Register printing services
        // Register RemotePrinterService for HTTP client-based printing (when remote service URL is configured)
        _ = services.AddHttpClient<RemotePrinterService>((serviceProvider, client) =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var serviceUrl = config["Printing:ServiceUrl"];
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                client.BaseAddress = new Uri(serviceUrl);
            }
        })
        .ConfigureHttpClient((serviceProvider, client) =>
        {
            // Additional configuration if needed
        });

        // Register BrowserPrinterService as an alternative implementation
        // This will be used by DirectPrintService when remote service is not available
        services.AddScoped<BrowserPrinterService>(serviceProvider =>
        {
            var jsRuntime = serviceProvider.GetRequiredService<IJSRuntime>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return new BrowserPrinterService(jsRuntime, loggerFactory.CreateLogger<BrowserPrinterService>());
        });

        // Register HybridPrinterService as the main IPrinterService
        // This will automatically use RemotePrinterService when available, otherwise BrowserPrinterService
        services.AddScoped<IPrinterService, HybridPrinterService>(serviceProvider =>
        {
            var remoteService = serviceProvider.GetRequiredService<RemotePrinterService>();
            var browserService = serviceProvider.GetRequiredService<BrowserPrinterService>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return new HybridPrinterService(
                remoteService,
                browserService,
                config,
                loggerFactory.CreateLogger<HybridPrinterService>());
        });

        // Register inner print service - always use DirectPrintService which handles fallback to browser printing
        services.AddScoped<IPrintService>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var jsRuntime = serviceProvider.GetRequiredService<IJSRuntime>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            // DirectPrintService will try remote service first, then fall back to browser printing
            // This allows the system to work with or without a remote printing service
            var printerService = serviceProvider.GetRequiredService<IPrinterService>();
            var innerService = new DirectPrintService(
                printerService,
                jsRuntime,
                loggerFactory.CreateLogger<DirectPrintService>(),
                config,
                loggerFactory);

            // Wrap with CentralizedPrintService for logging
            var printHistoryClient = serviceProvider.GetRequiredService<IPrintHistoryClient>();
            return new CentralizedPrintService(
                innerService,
                printHistoryClient,
                loggerFactory.CreateLogger<CentralizedPrintService>());
        });
    }
}
