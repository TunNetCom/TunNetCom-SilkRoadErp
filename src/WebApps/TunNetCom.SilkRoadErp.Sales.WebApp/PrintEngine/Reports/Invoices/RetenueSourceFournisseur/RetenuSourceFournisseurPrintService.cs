using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSourceFournisseur;

public class RetenuSourceFournisseurPrintService(
    ILogger<RetenuSourceFournisseurPrintService> _logger,
    IProviderInvoiceApiClient _providerInvoiceService,
    IProvidersApiClient _providersService,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<RetenuSourceFournisseurPrintModel, ProviderRetenuView> _printService)
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
        var _providerInvoiceResponse = await _providerInvoiceService.GetProviderInvoicesByIdsAsync(InvoicesIdList, cancellationToken);
        
        ProviderInvoiceResponse? invoiceResponse = _providerInvoiceResponse.FirstOrDefault();

        if (invoiceResponse is null)
        {
            return Result.Fail("failed_to_retrieve_at_least_one_invoice_for_retenue_source");
        }

        var providersResponse = await _providersService.GetAsync(invoiceResponse.ProviderId, cancellationToken);

        if (providersResponse.IsT1)
        {
            return Result.Fail("failed_to_retrieve_customer_form_retenu_source");
        }

         var providerResponse = providersResponse.AsT0;

        RetenuSourceFournisseurPrintModel retenuSourcePrintModel = BuildPrintModel(getAppParametersResponse, invoiceResponse, providerResponse);

        var pdfBytes = await _printService.GeneratePdfAsync(retenuSourcePrintModel, SilkPdfOptions.Default, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static RetenuSourceFournisseurPrintModel BuildPrintModel(
        Result<GetAppParametersResponse> getAppParametersResponse,
        ProviderInvoiceResponse invoiceResponse,
        ProviderResponse providerResponse)
    {
        return new RetenuSourceFournisseurPrintModel
        {
            Nom = providerResponse.Nom,
            Tel = providerResponse.Tel,
            Fax = providerResponse.Fax, // Assuming ProviderResponse has Fax
            Matricule = providerResponse.Matricule,
            Code = providerResponse.Code,
            CodeCat = providerResponse.CodeCat,
            EtbSec = providerResponse.EtbSec,
            Mail = providerResponse.Mail, // Assuming ProviderResponse has Mail
            MailDeux = providerResponse.MailDeux, // Assuming ProviderResponse has MailDeux
            Constructeur = providerResponse.Constructeur, // Assuming ProviderResponse has Constructeur
            Adresse = providerResponse.Adresse,

            // Company-related fields from app parameters
            CompanyName = getAppParametersResponse.Value.NomSociete,
            CompanyAdress = getAppParametersResponse.Value.Adresse,
            CompanyMatricule = getAppParametersResponse.Value.MatriculeFiscale,
            CompanyCodeCat = getAppParametersResponse.Value.CodeCategorie,
            CompanyCodeTVA = getAppParametersResponse.Value.CodeTva,
            CompanyEtbSec = getAppParametersResponse.Value.EtbSecondaire,

            // Invoices (required field)
            Invoices = new List<ProviderInvoiceResponse> { invoiceResponse }
        };
    }

    private async Task<Result<GetAppParametersResponse>> FetchAppParametersAsync(CancellationToken cancellationToken)
    {
        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);

        if (appParametersResult.IsT1)
        {
            Result.Fail("failed_to_retrieve_app_parameters");
        }

        var getAppParametersResponse = appParametersResult.AsT0;
        _logger.LogInformation("Successfully retrieved app parameters.");

        return Result.Ok(getAppParametersResponse);
    }
}
