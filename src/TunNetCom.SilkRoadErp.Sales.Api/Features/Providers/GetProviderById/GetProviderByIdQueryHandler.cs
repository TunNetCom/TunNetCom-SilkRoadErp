using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProviderById;

public class GetProviderByIdQueryHandler : IRequestHandler<GetProviderByIdQuery, Result<ProviderResponse>>
{
    private readonly SalesContext _context;
    private readonly ILogger<GetProviderByIdQueryHandler> _logger;

    public GetProviderByIdQueryHandler(SalesContext context, ILogger<GetProviderByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<ProviderResponse>> Handle(GetProviderByIdQuery query, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(Fournisseur), query.Id);

        var provider = await _context.Fournisseur.FindAsync(query.Id);
        if (provider is null)
        {
            _logger.LogEntityNotFound(nameof(Fournisseur), query.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        _logger.LogEntityFetchedById(nameof(Fournisseur), query.Id);

        return Result.Ok(provider.Adapt<ProviderResponse>());
    }
}
