using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Auth.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly SalesContext _context;

    public LogoutCommandHandler(SalesContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenEntity = await _context.RefreshToken
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshTokenEntity != null && !refreshTokenEntity.IsRevoked)
        {
            refreshTokenEntity.Revoke();
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok();
    }
}

