
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Playwright;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;
public class PrintPdfPlayWrightService<TModel, TView> : IPrintPdfService<TModel, TView> where TView : IComponent
{
    private readonly ILogger<PrintPdfPlayWrightService<TModel, TView>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private static bool _playwrightInstalled = false;
    private static readonly SemaphoreSlim _installSemaphore = new SemaphoreSlim(1, 1);

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
            // Ensure Playwright browsers are installed (only once)
            await EnsurePlaywrightInstalledAsync(cancellationToken);

            var html = await RenderComponentAsync(printModel);
            _logger.LogDebug("HTML rendered successfully, length: {HtmlLength} characters", html?.Length ?? 0);

            return await GeneratePdfFromHtmlAsync(html, pdfOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF for {ModelType}", typeof(TModel).Name);
            throw;
        }
    }

    private async Task EnsurePlaywrightInstalledAsync(CancellationToken cancellationToken)
    {
        // Double-check locking pattern to ensure installation happens only once
        if (_playwrightInstalled)
        {
            return;
        }

        // Use SemaphoreSlim for async locking
        await _installSemaphore.WaitAsync(cancellationToken);
        
        try
        {
            if (_playwrightInstalled)
            {
                return;
            }

            _logger.LogInformation("Checking Playwright browser installation...");

            // Try to verify if browsers are already installed by checking the executable path
            bool browsersInstalled = false;
            try
            {
                using var playwright = await Playwright.CreateAsync();
                var browserType = playwright.Chromium;
                var executablePath = browserType.ExecutablePath;
                
                if (!string.IsNullOrEmpty(executablePath) && System.IO.File.Exists(executablePath))
                {
                    _logger.LogInformation("Playwright browsers are already installed at: {ExecutablePath}", executablePath);
                    browsersInstalled = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not verify Playwright browser installation, will attempt to install: {Message}", ex.Message);
            }

            if (!browsersInstalled)
            {
                _logger.LogInformation("Installing Playwright browsers (Chromium only)...");
                try
                {
                    var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);
                    
                    if (exitCode != 0)
                    {
                        _logger.LogWarning("Playwright install command returned exit code {ExitCode}. Browsers may already be installed or installation may have failed.", exitCode);
                        // Continue anyway - browsers might be installed but the command failed for other reasons
                    }
                    else
                    {
                        _logger.LogInformation("Playwright browsers installed successfully");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during Playwright installation: {Message}. Will attempt to continue - browsers may already be installed.", ex.Message);
                    // Don't throw - we'll let the actual browser launch attempt fail if browsers are truly missing
                }
            }

            _playwrightInstalled = true;
        }
        finally
        {
            _installSemaphore.Release();
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

    private async Task<byte[]> GeneratePdfFromHtmlAsync(string html, SilkPdfOptions pdfOptions)
    {
        try
        {
            _logger.LogDebug("Creating Playwright instance...");
            using var playwright = await Playwright.CreateAsync();
            
            _logger.LogDebug("Launching Chromium browser...");
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            
            _logger.LogDebug("Creating new page...");
            var page = await browser.NewPageAsync();
            
            _logger.LogDebug("Setting page content...");
            await page.SetContentAsync(html);

            var playwrightOptions = pdfOptions.ToPlaywrightOptions();
            _logger.LogDebug("Generating PDF with options: Format={Format}, Margin={Margin}", 
                playwrightOptions.Format, playwrightOptions.Margin?.Top);
            
            var pdfBytes = await page.PdfAsync(playwrightOptions);
            
            _logger.LogInformation("PDF generated successfully, size: {PdfSize} bytes", pdfBytes?.Length ?? 0);
            return pdfBytes;
        }
        catch (PlaywrightException ex)
        {
            _logger.LogError(ex, "Playwright error while generating PDF. Message: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to generate PDF using Playwright: {ex.Message}. " +
                "Please ensure Playwright browsers are properly installed. " +
                "If running in Docker, verify that browsers were installed during the build process.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while generating PDF: {Message}", ex.Message);
            throw;
        }
    }
}