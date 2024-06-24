namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public class UpdateClientCommandHandler(SalesContext _context ,IValidator<UpdateClientRequest> validator) : IRequestHandler<UpdateClientCommand, ClientResponse>
{
    public async Task<ClientResponse> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Client.FindAsync(request.Id);
        ValidationResult validationResult = await validator.ValidateAsync(request.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
            
        if (client == null)
        {
            
            throw new KeyNotFoundException($"Client with Id {request.Id} not found.");
        }

        request.Request.Adapt(client);
        await _context.SaveChangesAsync(cancellationToken);
        return client.Adapt<ClientResponse>();
    }
}
