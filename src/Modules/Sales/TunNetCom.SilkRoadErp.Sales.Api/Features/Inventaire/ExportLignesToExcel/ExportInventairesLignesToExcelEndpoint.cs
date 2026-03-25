using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.ExportLignesToExcel;

public class ExportInventairesLignesToExcelEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/inventaires/export/lignes/excel", HandleExportAsync)
            .WithTags("Inventaire")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportAsync(
        [FromBody] ExportInventairesLignesExcelRequest request,
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] ExcelExportService exportService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<ExportInventairesLignesToExcelEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var ids = request.InventaireIds?.Distinct().ToArray() ?? Array.Empty<int>();

            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var decimalPlaces = await financialParametersService.GetDecimalPlacesAsync(appParams.Value.DecimalPlaces, cancellationToken);

            var columns = new List<ColumnMapping>
            {
                new() { PropertyName = nameof(InventaireLigneExcelRow.NumInventaire), DisplayName = "N° inventaire" },
                new() { PropertyName = nameof(InventaireLigneExcelRow.DateInventaire), DisplayName = "Date inventaire" },
                new() { PropertyName = nameof(InventaireLigneExcelRow.DescriptionInventaire), DisplayName = "Description" },
                new() { PropertyName = nameof(InventaireLigneExcelRow.StatutLibelle), DisplayName = "Statut" },
                new() { PropertyName = nameof(InventaireLigneExcelRow.ExerciceComptable), DisplayName = "Exercice comptable" },
                new() { PropertyName = nameof(InventaireLigneExcelRow.RefProduit), DisplayName = "Réf. produit" },
                new() { PropertyName = nameof(InventaireLigneExcelRow.NomProduit), DisplayName = "Produit" },
                new() { PropertyName = nameof(InventaireLigneExcelRow.QuantiteReelle), DisplayName = "Qté réelle" },
                new() { PropertyName = nameof(InventaireLigneExcelRow.PrixHt), DisplayName = "Prix HT" },
                new() { PropertyName = nameof(InventaireLigneExcelRow.MontantLigne), DisplayName = "Montant ligne HT" }
            };

            if (ids.Length == 0)
            {
                var emptyBytes = exportService.ExportToExcel(
                    Array.Empty<InventaireLigneExcelRow>(),
                    columns,
                    "Lignes inventaire",
                    decimalPlaces);
                var emptyName = $"Inventaires_Lignes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return TypedResults.File(
                    emptyBytes,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: emptyName);
            }

            var rows = await (
                from inv in context.Inventaire.AsNoTracking().FilterByActiveAccountingYear()
                join ay in context.AccountingYear.AsNoTracking() on inv.AccountingYearId equals ay.Id
                where ids.Contains(inv.Id)
                from ligne in context.LigneInventaire.AsNoTracking()
                where ligne.InventaireId == inv.Id
                join p in context.Produit.AsNoTracking() on ligne.RefProduit equals p.Refe into pg
                from p in pg.DefaultIfEmpty()
                orderby inv.Num, ligne.Id
                select new InventaireLigneExcelRow
                {
                    NumInventaire = inv.Num,
                    DateInventaire = inv.DateInventaire,
                    DescriptionInventaire = inv.Description ?? string.Empty,
                    StatutLibelle = inv.Statut == InventaireStatut.Brouillon ? "Brouillon" :
                        inv.Statut == InventaireStatut.Valide ? "Validé" : "Clôturé",
                    ExerciceComptable = ay.Year,
                    RefProduit = ligne.RefProduit,
                    NomProduit = p != null ? p.Nom : string.Empty,
                    QuantiteReelle = ligne.QuantiteReelle,
                    PrixHt = ligne.PrixHt,
                    MontantLigne = ligne.QuantiteReelle * ligne.PrixHt
                }).ToListAsync(cancellationToken);

            logger.LogInformation("Export inventaires lignes Excel: {Count} lignes pour {IdCount} inventaire(s)", rows.Count, ids.Length);

            var fileBytes = exportService.ExportToExcel(
                rows,
                columns,
                "Lignes inventaire",
                decimalPlaces);

            var filename = $"Inventaires_Lignes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return TypedResults.File(
                fileBytes,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting inventaire lines to Excel");
            return TypedResults.StatusCode(500);
        }
    }
}
