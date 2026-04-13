using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.UpdateInventaire;

public class UpdateInventaireCommandHandler(
    SalesContext _context,
    ILogger<UpdateInventaireCommandHandler> _logger)
    : IRequestHandler<UpdateInventaireCommand, Result>
{
    public async Task<Result> Handle(UpdateInventaireCommand command, CancellationToken cancellationToken)
    {
        var inventaire = await _context.Inventaire
            .Include(i => i.LigneInventaire)
            .FirstOrDefaultAsync(i => i.Id == command.Id, cancellationToken);

        if (inventaire == null)
        {
            _logger.LogWarning("Inventaire {Id} not found", command.Id);
            return Result.Fail("inventaire_not_found");
        }

        // Vérifier que l'inventaire n'est pas clôturé
        if (inventaire.Statut == Domain.Entites.InventaireStatut.Cloture)
        {
            _logger.LogWarning("Cannot update clôturé inventaire {Id}", command.Id);
            return Result.Fail("cannot_update_cloture_inventaire");
        }

        // Mettre à jour l'inventaire
        inventaire.UpdateInventaire(command.DateInventaire, command.Description);

        // Supprimer les lignes qui ne sont plus dans la commande
        var lignesIdsToKeep = command.Lignes
            .Where(l => l.Id.HasValue)
            .Select(l => l.Id!.Value)
            .ToList();

        var lignesToDelete = inventaire.LigneInventaire
            .Where(l => !lignesIdsToKeep.Contains(l.Id))
            .ToList();

        _context.LigneInventaire.RemoveRange(lignesToDelete);

        // Mettre à jour ou créer les lignes
        foreach (var ligneCmd in command.Lignes)
        {
            if (ligneCmd.Id.HasValue)
            {
                // Mettre à jour une ligne existante
                var ligneExistante = inventaire.LigneInventaire
                    .FirstOrDefault(l => l.Id == ligneCmd.Id.Value);

                if (ligneExistante != null)
                {
                    // Récupérer le produit pour mettre à jour la quantité théorique
                    var produit = await _context.Produit
                        .FirstOrDefaultAsync(p => p.Refe == ligneCmd.RefProduit, cancellationToken);

                    if (produit != null)
                    {
                        ligneExistante.UpdateLigneInventaire(
                            quantiteReelle: ligneCmd.QuantiteReelle,
                            prixHt: ligneCmd.PrixHt,
                            dernierPrixAchat: ligneCmd.DernierPrixAchat
                        );
                    }
                }
            }
            else
            {
                // Créer une nouvelle ligne
                var produit = await _context.Produit
                    .FirstOrDefaultAsync(p => p.Refe == ligneCmd.RefProduit, cancellationToken);

                if (produit != null)
                {
                    // Calculer la quantité théorique depuis le stock calculé
                    // Pour l'inventaire, on utilise le stock calculé comme quantité théorique
                    // Si pas de stock calculé, on utilise 0
                    var quantiteTheorique = 0; // Sera calculée depuis le stock si nécessaire
                    
                    var nouvelleLigne = Domain.Entites.LigneInventaire.CreateLigneInventaire(
                        inventaireId: inventaire.Id,
                        refProduit: ligneCmd.RefProduit,
                        quantiteTheorique: quantiteTheorique,
                        quantiteReelle: ligneCmd.QuantiteReelle,
                        prixHt: ligneCmd.PrixHt,
                        dernierPrixAchat: ligneCmd.DernierPrixAchat
                    );

                    await _context.LigneInventaire.AddAsync(nouvelleLigne, cancellationToken);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Inventaire {Id} updated successfully", command.Id);
        return Result.Ok();
    }
}

