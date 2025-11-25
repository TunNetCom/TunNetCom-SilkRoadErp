using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.UpdateDeliveryNote;

public class UpdateDeliveryNoteCommandHandler(
    SalesContext _context,
    ILogger<UpdateDeliveryNoteCommandHandler> _logger,
    IStockCalculationService _stockCalculationService,
    IActiveAccountingYearService _activeAccountingYearService)
    : IRequestHandler<UpdateDeliveryNoteCommand, Result>
{
    public async Task<Result> Handle(
        UpdateDeliveryNoteCommand updateDeliveryNoteCommand,
        CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(BonDeLivraison), updateDeliveryNoteCommand.Num);

        var deliveryNote = await _context.BonDeLivraison
            .Include(b => b.LigneBl)
            .FirstOrDefaultAsync(b => b.Num == updateDeliveryNoteCommand.Num, cancellationToken);

        if (deliveryNote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), updateDeliveryNoteCommand.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        // Update the delivery note properties
        deliveryNote.UpdateBonDeLivraison(
            updateDeliveryNoteCommand.Date,
            updateDeliveryNoteCommand.TotHTva,
            updateDeliveryNoteCommand.TotTva,
            updateDeliveryNoteCommand.NetPayer,
            updateDeliveryNoteCommand.TempBl,
            updateDeliveryNoteCommand.NumFacture,
            updateDeliveryNoteCommand.ClientId,
            activeAccountingYear.Id
        );

        // Récupérer les paramètres d'application pour vérifier si on doit bloquer la vente
        var systeme = await _context.Systeme.FirstOrDefaultAsync(cancellationToken);
        var bloquerVenteStockInsuffisant = systeme?.BloquerVenteStockInsuffisant ?? true;

        // Valider le stock pour chaque ligne
        var activeYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
        if (activeYearId.HasValue)
        {
            var stockErrors = new List<string>();
            foreach (var deliveryNoteDetail in updateDeliveryNoteCommand.DeliveryNoteDetails)
            {
                var stockResult = await _stockCalculationService.CalculateStockAsync(
                    deliveryNoteDetail.RefProduit, 
                    activeYearId.Value, 
                    cancellationToken);
                
                // Pour la mise à jour, on doit tenir compte des quantités déjà vendues dans ce BL
                // On récupère la quantité actuelle du BL pour ce produit
                var quantiteActuelleBl = deliveryNote.LigneBl
                    .Where(l => l.RefProduit == deliveryNoteDetail.RefProduit)
                    .Sum(l => l.QteLi);
                
                // Le stock disponible doit être suffisant pour la nouvelle quantité
                // On ajoute la quantité actuelle du BL au stock disponible pour avoir le stock réellement disponible
                var stockDisponibleReel = stockResult.StockDisponible + quantiteActuelleBl;
                
                if (stockDisponibleReel < deliveryNoteDetail.QteLi)
                {
                    var errorMessage = $"Produit {deliveryNoteDetail.RefProduit}: Stock disponible ({stockDisponibleReel}) insuffisant pour la quantité demandée ({deliveryNoteDetail.QteLi})";
                    stockErrors.Add(errorMessage);
                    
                    // Logger un avertissement dans tous les cas
                    _logger.LogWarning("Stock insuffisant détecté: {ErrorMessage}", errorMessage);
                }
            }

            // Bloquer uniquement si le paramètre est activé
            if (stockErrors.Any() && bloquerVenteStockInsuffisant)
            {
                _logger.LogWarning("Stock validation failed for delivery note update (blocage activé): {Errors}", string.Join("; ", stockErrors));
                return Result.Fail($"stock_insuffisant: {string.Join("; ", stockErrors)}");
            }
            
            // Si le paramètre est désactivé, on log un avertissement mais on continue
            if (stockErrors.Any() && !bloquerVenteStockInsuffisant)
            {
                _logger.LogWarning("Stock insuffisant détecté mais vente autorisée (blocage désactivé): {Errors}", string.Join("; ", stockErrors));
            }
        }

        // Remove all existing lines
        _context.LigneBl.RemoveRange(deliveryNote.LigneBl);

        // Add new lines
        foreach (var deliveryNoteDetail in updateDeliveryNoteCommand.DeliveryNoteDetails)
        {
            var ligneBl = new LigneBl
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

            deliveryNote.LigneBl.Add(ligneBl);
        }

        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(BonDeLivraison), updateDeliveryNoteCommand.Num);

        return Result.Ok();
    }
}

