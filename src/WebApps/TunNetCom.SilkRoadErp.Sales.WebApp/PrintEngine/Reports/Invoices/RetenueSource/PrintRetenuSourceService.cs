using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Playwright;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;

public class PrintRetenuSourceService
{
    private readonly ILogger<PrintRetenuSourceService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PrintRetenuSourceService(ILogger<PrintRetenuSourceService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(GetInvoiceListWithSummary GetInvoiceListWithSummary)
    {
        // Log the generation of the invoice PDF
        _logger.LogInformation("Generating Retenu source");

        Microsoft.Playwright.Program.Main(["install"]);
        // Render the InvoiceView component to HTML
        var htmlRenderer = new HtmlRenderer(_serviceProvider, _serviceProvider.GetRequiredService<ILoggerFactory>());
        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var dictionary = new Dictionary<string, object?> { { "GetInvoiceListWithSummary", GetInvoiceListWithSummary } };
            var parameters = ParameterView.FromDictionary(dictionary);
            var output = await htmlRenderer.RenderComponentAsync<RetenuSourceView>(parameters);
            return output.ToHtmlString();
        });

        // Generate PDF with Playwright
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);

        var pdfBytes = await page.PdfAsync(new PagePdfOptions
        {
            Format = "A4",
            DisplayHeaderFooter = true,
            HeaderTemplate = "<div style='font-size:12px; text-align:center; width:100%;'>My SaaS Application</div>",
            FooterTemplate = "<div style='font-size:12px; text-align:center; width:100%;'>Page <span class=\"pageNumber\"></span> of <span class=\"totalPages\"></span></div>",
        });

        await browser.CloseAsync();
        return pdfBytes;
    }
}
