using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.PrintInvoiceWithDetails;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.ProviderInvoices;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSourceFournisseur;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.DeliveryNotes.PrintDeliveryNote;

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
        _ = services.AddScoped(typeof(IPrintPdfService<,>), typeof(PrintPdfPlayWrightService<,>));

        // Register printing services
        _ = services.AddHttpClient<IPrinterService, RemotePrinterService>((serviceProvider, client) =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var serviceUrl = config["Printing:ServiceUrl"];
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                client.BaseAddress = new Uri(serviceUrl);
            }
        });

        // Register print service - use DirectPrintService if service URL is configured, otherwise use PrintToPdfService
        services.AddScoped<IPrintService>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var serviceUrl = config["Printing:ServiceUrl"];
            var jsRuntime = serviceProvider.GetRequiredService<IJSRuntime>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            if (!string.IsNullOrEmpty(serviceUrl))
            {
                var printerService = serviceProvider.GetRequiredService<IPrinterService>();
                return new DirectPrintService(
                    printerService,
                    jsRuntime,
                    loggerFactory.CreateLogger<DirectPrintService>(),
                    config,
                    loggerFactory);
            }
            else
            {
                return new PrintToPdfService(
                    jsRuntime,
                    loggerFactory.CreateLogger<PrintToPdfService>());
            }
        });
    }
}
