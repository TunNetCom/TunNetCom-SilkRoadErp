using Carter;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetClientsAvecProblemesSolde;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.ExportClientsAvecProblemesSoldeToPdf;

public class ExportClientsAvecProblemesSoldeToPdfEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/soldes/clients-avec-problemes/export/pdf", HandleExportToPdfAsync)
            .WithTags(EndpointTags.Soldes)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportToPdfAsync(
        [FromServices] IMediator mediator,
        [FromServices] PdfListExportService exportService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<ExportClientsAvecProblemesSoldeToPdfEndpoint> logger,
        [FromQuery] int? accountingYearId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "ExportClientsAvecProblemesSoldeToPdfEndpoint called with accountingYearId: {AccountingYearId}",
                accountingYearId);

            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var decimalPlaces = await financialParametersService.GetDecimalPlacesAsync(appParams.Value.DecimalPlaces, cancellationToken);

            var result = await mediator.Send(
                new GetClientsAvecProblemesSoldeQuery(1, 100_000, accountingYearId),
                cancellationToken);

            var items = result.Items;

            var columns = new List<ColumnMapping>
            {
                new() { PropertyName = "ClientNom", DisplayName = "Client" },
                new() { PropertyName = "Solde", DisplayName = "Solde" },
                new() { PropertyName = "NombreQuantitesNonLivrees", DisplayName = "Quantités non livrées" },
                new() { PropertyName = "TotalFactures", DisplayName = "Total factures" },
                new() { PropertyName = "TotalPaiements", DisplayName = "Total paiements" },
                new() { PropertyName = "DateDernierDocument", DisplayName = "Date dernier document" }
            };

            logger.LogInformation("Exporting {Count} clients avec problème de solde to PDF", items.Count);

            var fileBytes = await exportService.ExportToPdfAsync(
                items,
                columns,
                "Clients avec problème de solde",
                decimalPlaces,
                cancellationToken: cancellationToken);

            var filename = $"ClientsProblemesSolde_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return TypedResults.File(
                fileBytes,
                contentType: "application/pdf",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting clients avec problème de solde to PDF");
            return TypedResults.StatusCode(500);
        }
    }
}
