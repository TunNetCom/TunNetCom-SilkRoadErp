using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.ExportToPdf;

public class ExportDeliveryNotesToPdfEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/delivery-notes/export/pdf", HandleExportToPdfAsync)
            .WithTags(EndpointTags.DeliveryNotes)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportToPdfAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] PdfListExportService exportService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<ExportDeliveryNotesToPdfEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null,
        [FromQuery] int? technicianId = null,
        [FromQuery] int[]? tagIds = null,
        [FromQuery] int? status = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] string[]? selectedColumns = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "ExportDeliveryNotesToPdfEndpoint called with startDate: {StartDate}, endDate: {EndDate}, customerId: {CustomerId}, technicianId: {TechnicianId}, tagIds: {TagIds}, status: {Status}",
                startDate, endDate, customerId, technicianId, tagIds != null ? string.Join(",", tagIds) : "null", status);

            // Get financial parameters from service
            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var decimalPlaces = await financialParametersService.GetDecimalPlacesAsync(appParams.Value.DecimalPlaces, cancellationToken);

            // Build base query with filters before projection (on entity properties)
            var baseQuery = context.BonDeLivraison
                .AsNoTracking()
                .FilterByActiveAccountingYear()
                .AsQueryable();

            // Apply filters on entity before projection
            if (customerId.HasValue)
            {
                baseQuery = baseQuery.Where(bdl => bdl.ClientId == customerId.Value);
            }

            if (technicianId.HasValue)
            {
                logger.LogInformation("Applying technician filter: {technicianId}", technicianId);
                baseQuery = baseQuery.Where(bdl => bdl.InstallationTechnicianId == technicianId.Value);
            }

            // Apply tag filter if provided (OR logic: document must have at least one of the selected tags)
            if (tagIds != null && tagIds.Length > 0)
            {
                logger.LogInformation("Applying tag filter: {tagIds}", string.Join(",", tagIds));
                baseQuery = baseQuery.Where(bdl => context.DocumentTag
                    .Any(dt => dt.DocumentType == "BonDeLivraison" 
                        && dt.DocumentId == bdl.Num 
                        && tagIds.Contains(dt.TagId)));
            }

            // Apply Date Range filters
            if (startDate.HasValue)
            {
                logger.LogInformation("Applying start date filter: {startDate}", startDate);
                baseQuery = baseQuery.Where(bdl => bdl.Date >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                logger.LogInformation("Applying end date filter: {endDate}", endDate);
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                baseQuery = baseQuery.Where(bdl => bdl.Date <= endDateInclusive);
            }

            // Load data first to avoid SQL conversion issues with Statut (string -> enum -> int)
            var deliveryNotesData = await (from bdl in baseQuery
                                          join c in context.Client on bdl.ClientId equals c.Id into clientGroup
                                          from c in clientGroup.DefaultIfEmpty()
                                          select new { bdl, c })
                                          .ToListAsync(cancellationToken);

            // Apply Status filter in memory if needed
            if (status.HasValue)
            {
                logger.LogInformation("Applying status filter in memory: {status}", status);
                var statusEnum = (DocumentStatus)status.Value;
                deliveryNotesData = deliveryNotesData
                    .Where(x => x.bdl.Statut == statusEnum)
                    .ToList();
            }

            // Now project to DTO in memory
            var deliveryNotes = deliveryNotesData
                .Select(x => new GetDeliveryNoteBaseInfos
                {
                    Id = x.bdl.Id,
                    Number = x.bdl.Num,
                    Date = new DateTimeOffset(x.bdl.Date, TimeSpan.Zero),
                    NetAmount = x.bdl.NetPayer,
                    CustomerName = x.c != null ? x.c.Nom : null,
                    GrossAmount = x.bdl.TotHTva,
                    VatAmount = x.bdl.TotTva,
                    NumFacture = x.bdl.NumFacture,
                    CustomerId = x.bdl.ClientId,
                    Statut = (int)x.bdl.Statut,
                    StatutLibelle = x.bdl.Statut.ToString()
                })
                .AsQueryable();

            // Apply ordering
            if (!string.IsNullOrEmpty(orderBy))
            {
                var orderParts = orderBy.Split(' ');
                var propertyName = orderParts[0];
                var isDescending = orderParts.Length > 1 && orderParts[1].ToLower() == "desc";
                
                deliveryNotes = isDescending
                    ? deliveryNotes.OrderByDescending(GetOrderByExpression(propertyName))
                    : deliveryNotes.OrderBy(GetOrderByExpression(propertyName));
            }
            else
            {
                deliveryNotes = deliveryNotes.OrderBy(d => d.Date).ThenBy(d => d.Number);
            }

            var deliveryNoteList = deliveryNotes.ToList();

            // Define all available columns
            var allColumns = new List<ColumnMapping>
            {
                new() { PropertyName = "Number", DisplayName = "Numéro" },
                new() { PropertyName = "Date", DisplayName = "Date" },
                new() { PropertyName = "CustomerName", DisplayName = "Client" },
                new() { PropertyName = "NetAmount", DisplayName = "Montant HT" },
                new() { PropertyName = "VatAmount", DisplayName = "Montant TVA" },
                new() { PropertyName = "StatutLibelle", DisplayName = "Statut" }
            };

            // Filter columns based on selection
            var columnsToExport = selectedColumns != null && selectedColumns.Length > 0
                ? allColumns.Where(c => selectedColumns.Contains(c.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList()
                : allColumns;

            if (!columnsToExport.Any())
            {
                columnsToExport = allColumns; // Default to all if none selected
            }

            logger.LogInformation("Exporting {Count} delivery notes to PDF with {ColumnCount} columns", deliveryNoteList.Count, columnsToExport.Count);

            // Calculate totals with VAT details
            var vatRate7 = (int)await financialParametersService.GetVatRate7Async(appParams.Value.VatRate7, cancellationToken);
            var vatRate13 = (int)await financialParametersService.GetVatRate13Async(appParams.Value.VatRate13, cancellationToken);
            var vatRate19 = (int)await financialParametersService.GetVatRate19Async(appParams.Value.VatRate19, cancellationToken);

            var deliveryNoteIds = deliveryNoteList.Select(d => d.Id).ToList();
            var linesQuery = from bdl in context.BonDeLivraison
                            where deliveryNoteIds.Contains(bdl.Id)
                            join line in context.LigneBl on bdl.Id equals line.BonDeLivraisonId
                            select new { line.TotHt, line.TotTtc, Tva = (int)line.Tva };

            var lines = await linesQuery.ToListAsync(cancellationToken);

            var totalNetAmount = lines.Sum(l => l.TotHt);
            
            // Calculate VAT bases (using TotHt)
            var totalBase7 = lines.Where(l => l.Tva == vatRate7).Sum(l => l.TotHt);
            var totalBase13 = lines.Where(l => l.Tva == vatRate13).Sum(l => l.TotHt);
            var totalBase19 = lines.Where(l => l.Tva == vatRate19).Sum(l => l.TotHt);
            
            // Calculate VAT amounts (using TotTtc - TotHt)
            var totalVat7 = lines.Where(l => l.Tva == vatRate7).Sum(l => l.TotTtc - l.TotHt);
            var totalVat13 = lines.Where(l => l.Tva == vatRate13).Sum(l => l.TotTtc - l.TotHt);
            var totalVat19 = lines.Where(l => l.Tva == vatRate19).Sum(l => l.TotTtc - l.TotHt);
            var totalVatAmount = totalVat7 + totalVat13 + totalVat19;
            var totalTtcAmount = totalNetAmount + totalVatAmount;

            var fileBytes = await exportService.ExportToPdfAsync(
                deliveryNoteList, 
                columnsToExport, 
                "Liste des Bons de Livraison", 
                decimalPlaces, 
                totalNetAmount, 
                totalVatAmount, 
                totalTtcAmount, 
                totalVat7, 
                totalVat13, 
                totalVat19, 
                totalBase7, 
                totalBase13, 
                totalBase19, 
                cancellationToken);

            var filename = $"BonsLivraison_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return TypedResults.File(
                fileBytes,
                contentType: "application/pdf",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting delivery notes to PDF format");
            return TypedResults.StatusCode(500);
        }
    }

    private static System.Linq.Expressions.Expression<System.Func<GetDeliveryNoteBaseInfos, object>> GetOrderByExpression(string propertyName)
    {
        return propertyName.ToLower() switch
        {
            "number" => d => d.Number,
            "date" => d => d.Date,
            "customername" => d => d.CustomerName ?? string.Empty,
            "netamount" => d => d.NetAmount,
            "vatamount" => d => d.VatAmount,
            "grossamount" => d => d.GrossAmount,
            "statut" => d => d.Statut,
            _ => d => d.Number
        };
    }
}

