using TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.CreateOrder;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Result>
{
    private readonly SalesContext _context;
    private readonly ILogger<UpdateOrderCommandHandler> _logger;

    public UpdateOrderCommandHandler(
        SalesContext context,
        ILogger<UpdateOrderCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateOrderCommand updateOrderCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(Commandes), updateOrderCommand.Num);

        // Chercher la commande à mettre à jour avec ses lignes
        var orderToUpdate = await _context.Commandes
            .Include(c => c.LigneCommandes)
            .FirstOrDefaultAsync(c => c.Num == updateOrderCommand.Num, cancellationToken);

        if (orderToUpdate is null)
        {
            _logger.LogEntityNotFound(nameof(Commandes), updateOrderCommand.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        if (orderToUpdate.Statut == DocumentStatus.Valid)
        {
            return Result.Fail("Le document est validé et ne peut plus être modifié.");
        }

        // Mettre à jour les propriétés
        orderToUpdate.UpdateCommandes(
            num: updateOrderCommand.Num,
            fournisseurId: updateOrderCommand.FournisseurId,
            date: updateOrderCommand.Date);

        // Supprimer les anciennes lignes
        _context.LigneCommandes.RemoveRange(orderToUpdate.LigneCommandes);

        // Ajouter les nouvelles lignes
        foreach (var orderLine in updateOrderCommand.OrderLines)
        {
            var ligneCommande = new LigneCommandes
            {
                RefProduit = orderLine.RefProduit,
                DesignationLi = orderLine.DesignationLi,
                QteLi = orderLine.QteLi,
                PrixHt = orderLine.PrixHt,
                Remise = orderLine.Remise,
                TotHt = orderLine.TotHt,
                Tva = orderLine.Tva,
                TotTtc = orderLine.TotTtc,
                NumCommande = orderToUpdate.Num,
                NumCommandeNavigation = orderToUpdate
            };

            orderToUpdate.LigneCommandes.Add(ligneCommande);
        }

        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(Commandes), updateOrderCommand.Num);

        return Result.Ok();
    }
}

