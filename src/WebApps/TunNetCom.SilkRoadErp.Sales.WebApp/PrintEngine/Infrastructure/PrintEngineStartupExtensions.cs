using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.PrintInvoiceWithDetails;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSourceFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public static class PrintEngineStartupExtensions
{
    public static void AddPrintEngine(this IServiceCollection services)
    {
        services.AddScoped<PrintRetenuSourceService>();
        services.AddScoped<PrintFullInvoiceService>();
        services.AddScoped<RetenuSourceFournisseurPrintService>();
        services.AddScoped(typeof(IPrintPdfService<,>), typeof(PrintPdfPlayWrightService<,>));
    }
}
