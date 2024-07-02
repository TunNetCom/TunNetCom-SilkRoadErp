namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.CreateClient;

public class CreateClientCommandHandler(SalesContext _context, ILogger<CreateClientCommandHandler> logger)
    : IRequestHandler<CreateClientCommand, ClientResponse>
{
    public async Task<ClientResponse> Handle(CreateClientCommand createClientCommand, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating client with values: {@Model}", createClientCommand);

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

        logger.LogInformation("Client created successfully with ID: {Id}", client.Id);
        return client.Adapt<ClientResponse>();
    }
}
