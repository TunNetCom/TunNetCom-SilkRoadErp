using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.ValidateOrders;

public class ValidateOrdersCommandHandler(
    SalesContext _context,
    ILogger<ValidateOrdersCommandHandler> _logger)
    : IRequestHandler<ValidateOrdersCommand, Result>
{
    public async Task<Result> Handle(ValidateOrdersCommand command, CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var orders = await _context.Commandes
            .Where(c => command.Ids.Contains(c.Num))
            .ToListAsync(cancellationToken);

        if (orders.Count != command.Ids.Count)
        {
            var foundIds = orders.Select(c => c.Num).ToList();
            var missingIds = command.Ids.Except(foundIds).ToList();
            _logger.LogWarning("Orders not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"orders_not_found: {string.Join(", ", missingIds)}");
        }

        var errors = new List<string>();

        foreach (var order in orders)
        {
            try
            {
                if (order.Statut == DocumentStatus.Brouillon)
                {
                    order.Valider();
                    // Marquer explicitement la propriété comme modifiée car le setter est privé
                    _context.Entry(order).Property(x => x.Statut).IsModified = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot validate order {Id}", order.Num);
                errors.Add($"Id {order.Num}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Validated {Count} orders", orders.Count);

        return Result.Ok();
    }
}


