using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public class CreateDeliveryNoteCommandHandler(
    SalesContext _context,
    ILogger<CreateDeliveryNoteCommandHandler> _logger,
    INumberGeneratorService _numberGeneratorService,
    IStockCalculationService _stockCalculationService,
    IActiveAccountingYearService _activeAccountingYearService)
    : IRequestHandler<CreateDeliveryNoteCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateDeliveryNoteCommand createDeliveryNoteCommand,
        CancellationToken cancellationToken) 
    {
        _logger.LogEntityCreated(nameof(BonDeLivraison), createDeliveryNoteCommand);

        //TODO add checks
        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        var num = await _numberGeneratorService.GenerateBonDeLivraisonNumberAsync(activeAccountingYear.Id, cancellationToken);

        var deliveryNote = BonDeLivraison.CreateBonDeLivraison
            (
                createDeliveryNoteCommand.Date,
                createDeliveryNoteCommand.TotHTva,
                createDeliveryNoteCommand.TotTva,
                createDeliveryNoteCommand.NetPayer,
                createDeliveryNoteCommand.TempBl,
                createDeliveryNoteCommand.NumFacture,
                createDeliveryNoteCommand.ClientId,
                activeAccountingYear.Id
            );
        deliveryNote.Num = num;

        var deliveryNoteDetailsList = createDeliveryNoteCommand.DeliveryNoteDetails?.ToList() ?? new List<LigneBlSubCommand>();
        _logger.LogInformation($"Creating delivery note with {deliveryNoteDetailsList.Count} items");

        // Récupérer les paramètres d'application pour vérifier si on doit bloquer la vente
        var systeme = await _context.Systeme.FirstOrDefaultAsync(cancellationToken);
        var bloquerVenteStockInsuffisant = systeme?.BloquerVenteStockInsuffisant ?? true;

        // Valider le stock pour chaque ligne
        var activeYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
        if (activeYearId.HasValue)
        {
            var stockErrors = new List<string>();
            foreach (var deliveryNoteDetail in deliveryNoteDetailsList)
            {
                var stockResult = await _stockCalculationService.CalculateStockAsync(
                    deliveryNoteDetail.RefProduit, 
                    activeYearId.Value, 
                    cancellationToken);
                
                if (stockResult.StockDisponible < deliveryNoteDetail.QteLi)
                {
                    var errorMessage = $"Produit {deliveryNoteDetail.RefProduit}: Stock disponible ({stockResult.StockDisponible}) insuffisant pour la quantité demandée ({deliveryNoteDetail.QteLi})";
                    stockErrors.Add(errorMessage);
                    
                    // Logger un avertissement dans tous les cas
                    _logger.LogWarning("Stock insuffisant détecté: {ErrorMessage}", errorMessage);
                }
            }

            // Bloquer uniquement si le paramètre est activé
            if (stockErrors.Any() && bloquerVenteStockInsuffisant)
            {
                _logger.LogWarning("Stock validation failed for delivery note (blocage activé): {Errors}", string.Join("; ", stockErrors));
                return Result.Fail($"stock_insuffisant: {string.Join("; ", stockErrors)}");
            }
            
            // Si le paramètre est désactivé, on log un avertissement mais on continue
            if (stockErrors.Any() && !bloquerVenteStockInsuffisant)
            {
                _logger.LogWarning("Stock insuffisant détecté mais vente autorisée (blocage désactivé): {Errors}", string.Join("; ", stockErrors));
            }
        }

        foreach(var deliveryNoteDetail in deliveryNoteDetailsList) 
        {
            var lignesBl = new LigneBl
            {
                RefProduit = deliveryNoteDetail.RefProduit,
                DesignationLi = deliveryNoteDetail.DesignationLi,
                QteLi = deliveryNoteDetail.QteLi,
                QteLivree = deliveryNoteDetail.QteLivree,
                PrixHt = deliveryNoteDetail.PrixHt,
                Remise = deliveryNoteDetail.Remise,
                TotHt = deliveryNoteDetail.TotHt,
                Tva = deliveryNoteDetail.Tva,
                TotTtc = deliveryNoteDetail.TotTtc,
                BonDeLivraisonId = deliveryNote.Id,
                NumBlNavigation = deliveryNote
            };

            // TODO make method to add lignesBl
            deliveryNote.LigneBl.Add( lignesBl );
        }

        _logger.LogInformation($"Added {deliveryNote.LigneBl.Count} lines to delivery note before saving");

        _ = _context.BonDeLivraison.Add(deliveryNote);
        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully(nameof(BonDeLivraison), deliveryNote.Num);

        return deliveryNote.Num;
    }
}
