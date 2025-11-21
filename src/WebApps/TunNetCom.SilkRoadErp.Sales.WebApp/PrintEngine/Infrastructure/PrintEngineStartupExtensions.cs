using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.PrintInvoiceWithDetails;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.ProviderInvoices;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSourceFournisseur;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.DeliveryNotes.PrintDeliveryNote;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public static class PrintEngineStartupExtensions
{
    public static void AddPrintEngine(this IServiceCollection services)
    {
        _ = services.AddScoped<PrintRetenuSourceService>();
        _ = services.AddScoped<PrintFullInvoiceService>();
        _ = services.AddScoped<RetenuSourceFournisseurPrintService>();
        _ = services.AddScoped<PrintProviderFullInvoiceService>();
        _ = services.AddScoped<PrintDeliveryNoteService>();
        _ = services.AddScoped(typeof(IPrintPdfService<,>), typeof(PrintPdfPlayWrightService<,>));
    }
}
