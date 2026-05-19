using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeliverRemaining;

public class DeliverRemainingQuantitiesCommandHandler(
    SalesContext _context,
    ILogger<DeliverRemainingQuantitiesCommandHandler> _logger)
    : IRequestHandler<DeliverRemainingQuantitiesCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        DeliverRemainingQuantitiesCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing DeliverRemainingQuantitiesCommand for Invoice {InvoiceId}", command.InvoiceId);

        var facture = await _context.Facture
            .Include(f => f.BonDeLivraison)
                .ThenInclude(bl => bl.LigneBl)
            .FirstOrDefaultAsync(f => f.Id == command.InvoiceId, cancellationToken);

        if (facture == null)
        {
            return Result.Fail("invoice_not_found");
        }

        if (facture.Statut != DocumentStatus.Draft)
        {
            return Result.Fail("invoice_must_be_draft");
        }

        foreach (var itemSub in command.Items)
        {
            var line = facture.BonDeLivraison
                .SelectMany(bl => bl.LigneBl)
                .FirstOrDefault(l => l.RefProduit == itemSub.RefProduit);

            if (line == null)
            {
                _logger.LogWarning("Product {RefProduit} not found in invoice {InvoiceId}", itemSub.RefProduit, command.InvoiceId);
                continue;
            }

            line.QteLivree = itemSub.QuantityToDeliver;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return facture.Id;
    }
}
