using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.VerifyReception;

public class VerifyReceptionCommandHandler(
    SalesContext _context,
    ILogger<VerifyReceptionCommandHandler> _logger)
    : IRequestHandler<VerifyReceptionCommand, Result<VerifyReceptionResponse>>
{
    public async Task<Result<VerifyReceptionResponse>> Handle(
        VerifyReceptionCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verifying reception for RetourMarchandiseFournisseur {Num}", command.Num);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Récupérer le retour avec ses lignes
            var retour = await _context.RetourMarchandiseFournisseur
                .Include(r => r.LigneRetourMarchandiseFournisseur)
                .FirstOrDefaultAsync(r => r.Num == command.Num, cancellationToken);

            if (retour == null)
            {
                _logger.LogWarning("RetourMarchandiseFournisseur {Num} not found", command.Num);
                return Result.Fail<VerifyReceptionResponse>("retour_not_found");
            }

            // Vérifier que le retour est dans un statut permettant la réception
            if (retour.StatutRetour != RetourFournisseurStatus.Valid &&
                retour.StatutRetour != RetourFournisseurStatus.EnReparation &&
                retour.StatutRetour != RetourFournisseurStatus.ReceptionPartielle)
            {
                _logger.LogWarning("RetourMarchandiseFournisseur {Num} has invalid status {Status} for reception",
                    command.Num, retour.StatutRetour);
                return Result.Fail<VerifyReceptionResponse>("invalid_status_for_reception");
            }

            // Créer un dictionnaire des lignes pour accès rapide
            var lignesDict = retour.LigneRetourMarchandiseFournisseur.ToDictionary(l => l.IdLigne);
            var lignesMisesAJour = new List<VerifyReceptionLineResponse>();

            // Mettre à jour chaque ligne
            foreach (var lineRequest in command.Lines)
            {
                if (!lignesDict.TryGetValue(lineRequest.IdLigne, out var ligne))
                {
                    _logger.LogWarning("Ligne {IdLigne} not found in retour {Num}", lineRequest.IdLigne, command.Num);
                    continue;
                }

                // Valider la quantité reçue
                if (lineRequest.QteRecue < 0)
                {
                    return Result.Fail<VerifyReceptionResponse>($"La quantité reçue pour la ligne {lineRequest.IdLigne} ne peut pas être négative");
                }

                if (lineRequest.QteRecue > ligne.QteLi)
                {
                    return Result.Fail<VerifyReceptionResponse>(
                        $"La quantité reçue ({lineRequest.QteRecue}) pour la ligne {lineRequest.IdLigne} ne peut pas dépasser la quantité retournée ({ligne.QteLi})");
                }

                // Enregistrer la réception
                ligne.EnregistrerReception(lineRequest.QteRecue, command.Utilisateur);

                lignesMisesAJour.Add(new VerifyReceptionLineResponse
                {
                    IdLigne = ligne.IdLigne,
                    RefProduit = ligne.RefProduit,
                    Designation = ligne.DesignationLi,
                    QteRetournee = ligne.QteLi,
                    QteRecue = ligne.QteRecue,
                    QteEnAttente = ligne.GetQuantiteEnAttente()
                });
            }

            // Mettre à jour le statut du retour
            retour.ValiderReception();

            // Créer l'entrée de traçabilité
            var receptionTrace = ReceptionRetourFournisseur.Create(
                retourMarchandiseFournisseurId: retour.Id,
                dateReception: DateTime.Now,
                utilisateur: command.Utilisateur,
                commentaire: command.Commentaire);

            await _context.ReceptionRetourFournisseur.AddAsync(receptionTrace, cancellationToken);

            // Sauvegarder les modifications
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var quantiteTotaleRecue = retour.GetQuantiteTotaleRecue();
            var quantiteEnAttente = retour.GetQuantiteEnAttenteReception();

            var response = new VerifyReceptionResponse
            {
                Success = true,
                Num = retour.Num,
                NouveauStatut = retour.StatutRetour.ToString(),
                QuantiteTotaleRecue = quantiteTotaleRecue,
                QuantiteEnAttente = quantiteEnAttente,
                Message = GetStatusMessage(retour.StatutRetour, quantiteTotaleRecue, quantiteEnAttente),
                LignesMisesAJour = lignesMisesAJour
            };

            _logger.LogInformation(
                "Reception verified for RetourMarchandiseFournisseur {Num}. New status: {Status}, Received: {Received}, Pending: {Pending}",
                command.Num, retour.StatutRetour, quantiteTotaleRecue, quantiteEnAttente);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error verifying reception for RetourMarchandiseFournisseur {Num}", command.Num);
            return Result.Fail<VerifyReceptionResponse>($"Erreur lors de la vérification de la réception: {ex.Message}");
        }
    }

    private static string GetStatusMessage(RetourFournisseurStatus status, int received, int pending)
    {
        return status switch
        {
            RetourFournisseurStatus.Cloture => $"Réception complète. Tous les articles ({received}) ont été reçus.",
            RetourFournisseurStatus.ReceptionPartielle => $"Réception partielle. {received} article(s) reçu(s), {pending} en attente.",
            RetourFournisseurStatus.EnReparation => "Les articles sont toujours en réparation chez le fournisseur.",
            _ => "Statut mis à jour."
        };
    }
}
