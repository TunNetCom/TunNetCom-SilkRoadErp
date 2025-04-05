using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Playwright;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;

public class PrintRetenuSourceService
{
    private readonly ILogger<PrintRetenuSourceService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICustomersApiClient _customerService;
    private readonly IInvoicesApiClient _invoicesService;
    private readonly IAppParametersClient _appParametersClient;

    public PrintRetenuSourceService(ILogger<PrintRetenuSourceService> logger,
                                    IServiceProvider serviceProvider,
                                    IInvoicesApiClient invoicesService,
                                    ICustomersApiClient customerService,
                                    IAppParametersClient appParametersClient)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _invoicesService = invoicesService;
        _customerService = customerService;
        _appParametersClient = appParametersClient;
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(List<int> InvoicesIdList, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating Retenu source");
        Microsoft.Playwright.Program.Main(["install"]);
        var htmlRenderer = new HtmlRenderer(_serviceProvider, _serviceProvider.GetRequiredService<ILoggerFactory>());

        GetAppParametersResponse getAppParametersResponse = new GetAppParametersResponse();
        InvoiceResponse invoiceResponse = new InvoiceResponse();

        getAppParametersResponse = await FetchAppParametersAsync(getAppParametersResponse, cancellationToken);

        var invoicesResponse = await _invoicesService.GetInvoicesByIdsAsync(InvoicesIdList, cancellationToken);
        InvoiceResponse invoice = new InvoiceResponse();
        if (invoicesResponse.IsT0)
        {
            invoice = invoicesResponse.AsT0.FirstOrDefault();
        }
        else if (invoicesResponse.IsT1)
        {
            throw new Exception($"Failed to retrieve invoices");
        }

        var customerResponse = await _customerService.GetCustomerById(invoice.CustomerId, cancellationToken);

        var retenuSourcePrintModel = new RetenuSourcePrintModel
        {
            Invoices = new List<InvoiceResponse>() { invoice },
            CustomerName = customerResponse.Nom,
            CustomerTel = customerResponse.Tel,
            CustomerAdresse = customerResponse.Adresse,
            CustomerMatricule = customerResponse.Matricule,
            CustomerCode = customerResponse.Code,
            CustomerCodeCat = customerResponse.CodeCat,
            CustomerEtbSec = customerResponse.EtbSec,
            CompanyAdress = getAppParametersResponse.Adresse,
            CompanyCodeCat = getAppParametersResponse.CodeCategorie,
            CompanyCodeTVA = getAppParametersResponse.CodeTva,
            CompanyEtbSec = getAppParametersResponse.EtbSecondaire,
            CompanyName = getAppParametersResponse.NomSociete,
            CompanyMatricule = getAppParametersResponse.MatriculeFiscale,
        };

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var dictionary = new Dictionary<string, object?> { { "RetenuSourcePrintModel", retenuSourcePrintModel } };
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
            //HeaderTemplate = "<div style='font-size:12px; text-align:center; width:100%;'>Retenu</div>",
            //FooterTemplate = "<div style='font-size:12px; text-align:center; width:100%;'>Page <span class=\"pageNumber\"></span> of <span class=\"totalPages\"></span></div>",
        });

        await browser.CloseAsync();
        return pdfBytes;
    }

    private async Task<GetAppParametersResponse> FetchAppParametersAsync(GetAppParametersResponse getAppParametersResponse, CancellationToken cancellationToken)
    {
        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);
        if (appParametersResult.IsT0)
        {
            getAppParametersResponse = appParametersResult.AsT0;
            _logger.LogInformation("Successfully retrieved app parameters.");
        }
        else if (appParametersResult.IsT1)
        {
            throw new Exception("Failed to retrieve app parameters.");
        }

        return getAppParametersResponse;
    }
}
