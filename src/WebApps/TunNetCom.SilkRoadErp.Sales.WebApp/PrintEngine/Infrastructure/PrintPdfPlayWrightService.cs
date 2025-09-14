namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Playwright;

public class PrintPdfPlayWrightService<TModel, TView> : IPrintPdfService<TModel, TView> where TView : IComponent
{
    private readonly ILogger<PrintPdfPlayWrightService<TModel, TView>> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PrintPdfPlayWrightService(
        ILogger<PrintPdfPlayWrightService<TModel, TView>> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<byte[]> GeneratePdfAsync(TModel printModel, CancellationToken cancellationToken)
    {
        return await GeneratePdfAsync(printModel, SilkPdfOptions.Default, cancellationToken);
    }

    public async Task<byte[]> GeneratePdfAsync(TModel printModel, SilkPdfOptions pdfOptions, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating PDF for {ModelType}", typeof(TModel).Name);

        try
        {
            _ = Microsoft.Playwright.Program.Main(["install"]);

            var html = await RenderComponentAsync(printModel);

            return await GeneratePdfFromHtmlAsync(html, pdfOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF for {ModelType}", typeof(TModel).Name);
            throw;
        }
    }

    private async Task<string> RenderComponentAsync(TModel printModel)
    {
        var htmlRenderer = new HtmlRenderer(_serviceProvider, _serviceProvider.GetRequiredService<ILoggerFactory>());

        return await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var dictionary = new Dictionary<string, object?> { { "Model", printModel } };
            var parameters = ParameterView.FromDictionary(dictionary);
            var output = await htmlRenderer.RenderComponentAsync<TView>(parameters);
            return output.ToHtmlString();
        });
    }

    private static async Task<byte[]> GeneratePdfFromHtmlAsync(string html, SilkPdfOptions pdfOptions)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);

        var playwrightOptions = pdfOptions.ToPlaywrightOptions();
        var pdfBytes = await page.PdfAsync(playwrightOptions);

        return pdfBytes;
    }
}