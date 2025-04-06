using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.PrintInvoiceWithDetails;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public static class PrintEngineStartupExtensions
{
    public static void AddPrintEngine(this IServiceCollection services)
    {
        services.AddScoped<PrintRetenuSourceService>();
        services.AddScoped<PrintFullInvoiceService>();
        services.AddScoped(typeof(IPrintPdfService<,>), typeof(PrintPdfPlayWrightService<,>));
    }
}
