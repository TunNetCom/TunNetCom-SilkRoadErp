namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.CreateClient;

public class CreateClientCommandHandler(SalesContext _context, IValidator<CreateClientRequest> validator,
    ILogger<CreateClientCommandHandler> logger) : IRequestHandler<CreateClientCommand, ClientResponse>
{
    public async Task<ClientResponse> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Create Client with Values : {@Model}", request);
        ValidationResult validationResult = await validator.ValidateAsync(request.Request, cancellationToken);
        if (!validationResult.IsValid) 
        {
            logger.LogWarning("Validation failed for client creation: {@ValidationResult}", validationResult);
            throw new ValidationException(validationResult.Errors);
        }

        var client = request.Request.Adapt<Client>();
        _context.Client.Add(client);
        await _context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Client created successfully with ID: {Id}", client.Id);
        return client.Adapt<ClientResponse>();
    }
}
