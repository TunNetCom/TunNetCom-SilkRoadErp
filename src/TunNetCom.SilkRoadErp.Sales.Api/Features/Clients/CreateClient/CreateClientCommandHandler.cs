namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.CreateClient;

public class CreateClientCommandHandler(SalesContext _context, ILogger<CreateClientCommandHandler> _logger)
    : IRequestHandler<CreateClientCommand,Result<int>>
{
    public async Task<Result<int>> Handle(CreateClientCommand createClientCommand, CancellationToken cancellationToken)
    {
        _logger.LogClientCreated(
            createClientCommand);

        var client = Client.CreateClient
        (
            createClientCommand.Nom,
            createClientCommand.Tel,
            createClientCommand.Adresse,
            createClientCommand.Matricule,
            createClientCommand.Code,
            createClientCommand.CodeCat,
            createClientCommand.EtbSec,
            createClientCommand.Mail
        );

        _context.Client.Add(client);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogClientCreatedSuccessfully(
            client.Id);

        return client.Id;
    }
}
