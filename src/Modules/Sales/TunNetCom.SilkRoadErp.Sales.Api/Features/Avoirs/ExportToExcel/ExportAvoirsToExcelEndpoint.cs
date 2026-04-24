using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.ExportToExcel;

public class ExportAvoirsToExcelEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/avoirs/export/excel", HandleExportToExcelAsync)
            .WithTags(EndpointTags.Avoirs)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> HandleExportToExcelAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] ExcelExportService exportService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<ExportAvoirsToExcelEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? clientId = null,
        [FromQuery] int? status = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] string[]? selectedColumns = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "ExportAvoirsToExcelEndpoint called with startDate: {StartDate}, endDate: {EndDate}, clientId: {ClientId}, status: {Status}",
                startDate, endDate, clientId, status);

            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var decimalPlaces = await financialParametersService.GetDecimalPlacesAsync(
                appParams.Value.DecimalPlaces,
                cancellationToken);

            // Build base query with filters (keep it translatable to SQL)
            var baseQuery = context.Avoirs
                .AsNoTracking()
                .FilterByActiveAccountingYear()
                .AsQueryable();

            if (clientId.HasValue)
            {
                baseQuery = baseQuery.Where(a => a.ClientId == clientId.Value);
            }

            // Statut is stored as nvarchar (enum as string); do not cast to int in SQL.
            if (status.HasValue)
            {
                var statusEnum = (DocumentStatus)status.Value;
                baseQuery = baseQuery.Where(a => a.Statut == statusEnum);
            }

            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(a => a.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                baseQuery = baseQuery.Where(a => a.Date <= endDateInclusive);
            }

            // Project with totals in SQL (keep it EF-translatable: avoid enum ToString() here)
            var query = from a in baseQuery
                        join c in context.Client on a.ClientId equals c.Id into clientGroup
                        from c in clientGroup.DefaultIfEmpty()
                        select new
                        {
                            a.Num,
                            a.Date,
                            a.ClientId,
                            ClientName = c != null ? c.Nom : null,
                            TotalExcludingTaxAmount = a.LigneAvoirs.Sum(l => l.TotHt),
                            TotalVATAmount = a.LigneAvoirs.Sum(l => l.TotTtc - l.TotHt),
                            TotalIncludingTaxAmount = a.LigneAvoirs.Sum(l => l.TotTtc),
                            DocumentStatut = a.Statut
                        };

            // Apply ordering
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var orderParts = orderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var propertyName = orderParts[0];
                var isDescending = orderParts.Length > 1 && orderParts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

                query = (propertyName.ToLowerInvariant(), isDescending) switch
                {
                    ("num", false) => query.OrderBy(x => x.Num),
                    ("num", true) => query.OrderByDescending(x => x.Num),
                    ("date", false) => query.OrderBy(x => x.Date),
                    ("date", true) => query.OrderByDescending(x => x.Date),
                    ("clientname", false) => query.OrderBy(x => x.ClientName),
                    ("clientname", true) => query.OrderByDescending(x => x.ClientName),
                    ("totalexcludingtaxamount", false) => query.OrderBy(x => x.TotalExcludingTaxAmount),
                    ("totalexcludingtaxamount", true) => query.OrderByDescending(x => x.TotalExcludingTaxAmount),
                    ("totalvatamount", false) => query.OrderBy(x => x.TotalVATAmount),
                    ("totalvatamount", true) => query.OrderByDescending(x => x.TotalVATAmount),
                    ("totalincludingtaxamount", false) => query.OrderBy(x => x.TotalIncludingTaxAmount),
                    ("totalincludingtaxamount", true) => query.OrderByDescending(x => x.TotalIncludingTaxAmount),
                    ("statut", false) => query.OrderBy(x => x.DocumentStatut),
                    ("statut", true) => query.OrderByDescending(x => x.DocumentStatut),
                    _ => query.OrderByDescending(x => x.Num)
                };
            }
            else
            {
                query = query.OrderByDescending(x => x.Num);
            }

            var rawAvoirs = await query.ToListAsync(cancellationToken);

            var avoirs = rawAvoirs.Select(x => new AvoirBaseInfo
            {
                Num = x.Num,
                Date = new DateTimeOffset(x.Date),
                ClientId = x.ClientId,
                ClientName = x.ClientName,
                TotalExcludingTaxAmount = x.TotalExcludingTaxAmount,
                TotalVATAmount = x.TotalVATAmount,
                TotalIncludingTaxAmount = x.TotalIncludingTaxAmount,
                Statut = (int)x.DocumentStatut,
                StatutLibelle = x.DocumentStatut.ToString()
            }).ToList();

            // Totaux + récap TVA par taux (7 / 13 / 19), même logique que les BLs
            var vatRate7 = (int)await financialParametersService.GetVatRate7Async(appParams.Value.VatRate7, cancellationToken);
            var vatRate13 = (int)await financialParametersService.GetVatRate13Async(appParams.Value.VatRate13, cancellationToken);
            var vatRate19 = (int)await financialParametersService.GetVatRate19Async(appParams.Value.VatRate19, cancellationToken);

            var avoirIds = await baseQuery.Select(a => a.Id).ToListAsync(cancellationToken);
            var lines = await (
                from line in context.LigneAvoirs.AsNoTracking()
                where avoirIds.Contains(line.AvoirsId)
                select new { line.TotHt, line.TotTtc, Tva = (int)line.Tva }
            ).ToListAsync(cancellationToken);

            var totalNetAmount = lines.Sum(l => l.TotHt);
            var totalBase7 = lines.Where(l => l.Tva == vatRate7).Sum(l => l.TotHt);
            var totalBase13 = lines.Where(l => l.Tva == vatRate13).Sum(l => l.TotHt);
            var totalBase19 = lines.Where(l => l.Tva == vatRate19).Sum(l => l.TotHt);
            var totalVat7 = lines.Where(l => l.Tva == vatRate7).Sum(l => l.TotTtc - l.TotHt);
            var totalVat13 = lines.Where(l => l.Tva == vatRate13).Sum(l => l.TotTtc - l.TotHt);
            var totalVat19 = lines.Where(l => l.Tva == vatRate19).Sum(l => l.TotTtc - l.TotHt);
            var totalVatAmount = totalVat7 + totalVat13 + totalVat19;
            var totalTtcAmount = totalNetAmount + totalVatAmount;

            logger.LogInformation(
                "Avoirs export VAT recap — Base7: {B7}, Base13: {B13}, Base19: {B19}, Vat7: {V7}, Vat13: {V13}, Vat19: {V19}",
                totalBase7, totalBase13, totalBase19, totalVat7, totalVat13, totalVat19);

            var allColumns = new List<ColumnMapping>
            {
                new() { PropertyName = nameof(AvoirBaseInfo.Num), DisplayName = "Numéro" },
                new() { PropertyName = nameof(AvoirBaseInfo.Date), DisplayName = "Date" },
                new() { PropertyName = nameof(AvoirBaseInfo.ClientName), DisplayName = "Client" },
                new() { PropertyName = nameof(AvoirBaseInfo.TotalExcludingTaxAmount), DisplayName = "Montant HT" },
                new() { PropertyName = nameof(AvoirBaseInfo.TotalVATAmount), DisplayName = "Montant TVA" },
                new() { PropertyName = nameof(AvoirBaseInfo.TotalIncludingTaxAmount), DisplayName = "Total TTC" },
                new() { PropertyName = nameof(AvoirBaseInfo.StatutLibelle), DisplayName = "Statut" }
            };

            var columnsToExport = selectedColumns != null && selectedColumns.Length > 0
                ? allColumns.Where(c => selectedColumns.Contains(c.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList()
                : allColumns;

            if (!columnsToExport.Any())
            {
                columnsToExport = allColumns;
            }

            var fileBytes = exportService.ExportToExcel(
                avoirs,
                columnsToExport,
                sheetName: "Avoirs client",
                decimalPlaces: decimalPlaces,
                totalNetAmount: totalNetAmount,
                totalVatAmount: totalVatAmount,
                totalTtcAmount: totalTtcAmount,
                totalVat7: totalVat7,
                totalVat13: totalVat13,
                totalVat19: totalVat19,
                totalBase7: totalBase7,
                totalBase13: totalBase13,
                totalBase19: totalBase19,
                appendVatRecapSection: true);

            var filename = $"AvoirsClient_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return TypedResults.File(
                fileBytes,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting avoirs to Excel format");

            // Return a readable error to help diagnose issues from the WebApp.
            // (The WebApp already shows ex.Message in its notification.)
            return TypedResults.Problem(
                title: "Export avoirs to Excel failed",
                detail: ex.InnerException != null ? $"{ex.Message} | Inner: {ex.InnerException.Message}" : ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}

