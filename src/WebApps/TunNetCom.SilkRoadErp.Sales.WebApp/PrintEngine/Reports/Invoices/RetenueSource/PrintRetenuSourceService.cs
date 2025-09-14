using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;

public class PrintRetenuSourceService(
    ILogger<PrintRetenuSourceService> _logger,
    IInvoicesApiClient _invoicesService,
    ICustomersApiClient _customerService,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<RetenuSourcePrintModel, RetenuSourceView> _printService) 
{
    public async Task<Result<byte[]>> GenerateRetenuSourcePdfAsync(
        List<int> InvoicesIdList,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating Retenu source");

        var getAppParametersResponse = await FetchAppParametersAsync(cancellationToken);

        if (getAppParametersResponse.IsFailed)
        {
            return Result.Fail("failed_to_retrieve_app_parameters");
        }

        var invoicesResponse = await _invoicesService.GetInvoicesByIdsAsync(InvoicesIdList, cancellationToken);
        if (invoicesResponse.IsT1)
        {
            return Result.Fail("failed_to_retrieve_invoices_for_retenue_source");
        }

        InvoiceResponse? invoiceResponse = invoicesResponse.AsT0.FirstOrDefault();

        if (invoiceResponse is null)
        {
            return Result.Fail("failed_to_retrieve_at_least_one_invoice_for_retenue_source");
        }

        var customerResponse = await _customerService.GetCustomerByIdAsync(invoiceResponse.CustomerId, cancellationToken);

        if (customerResponse is null)
        {
            return Result.Fail("failed_to_retrieve_customer_form_retenu_source");
        }

        RetenuSourcePrintModel retenuSourcePrintModel = BuildPrintModel(getAppParametersResponse, invoiceResponse, customerResponse);

        var pdfBytes = await _printService.GeneratePdfAsync(retenuSourcePrintModel, SilkPdfOptions.Default, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static RetenuSourcePrintModel BuildPrintModel(
        Result<GetAppParametersResponse> getAppParametersResponse,
        InvoiceResponse invoiceResponse,
        CustomerResponse customerResponse)
    {
        return new RetenuSourcePrintModel
        {
            Invoices = new List<InvoiceResponse>() { invoiceResponse },
            CustomerName = customerResponse.Name,
            CustomerTel = customerResponse.Tel,
            CustomerAdresse = customerResponse.Adresse,
            CustomerMatricule = customerResponse.Matricule,
            CustomerCode = customerResponse.Code,
            CustomerCodeCat = customerResponse.CodeCat,
            CustomerEtbSec = customerResponse.EtbSec,
            CompanyAdress = getAppParametersResponse.Value.Adresse,
            CompanyCodeCat = getAppParametersResponse.Value.CodeCategorie,
            CompanyCodeTVA = getAppParametersResponse.Value.CodeTva,
            CompanyEtbSec = getAppParametersResponse.Value.EtbSecondaire,
            CompanyName = getAppParametersResponse.Value.NomSociete,
            CompanyMatricule = getAppParametersResponse.Value.MatriculeFiscale,
        };
    }

    private async Task<Result<GetAppParametersResponse>> FetchAppParametersAsync(CancellationToken cancellationToken)
    {
        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);

        if (appParametersResult.IsT1)
        {
            _ = Result.Fail("failed_to_retrieve_app_parameters");
        }

        var getAppParametersResponse = appParametersResult.AsT0;
        _logger.LogInformation("Successfully retrieved app parameters.");

        return Result.Ok(getAppParametersResponse);
    }
}
