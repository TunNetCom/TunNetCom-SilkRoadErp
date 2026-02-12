using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.UpdateFactureDepense;

public class UpdateFactureDepenseCommandHandler(
    SalesContext _context,
    ILogger<UpdateFactureDepenseCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<UpdateFactureDepenseCommand, Result>
{
    public async Task<Result> Handle(UpdateFactureDepenseCommand command, CancellationToken cancellationToken)
    {
        var entity = await _context.FactureDepense.FindAsync(new object[] { command.Id }, cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("FactureDepense not found: Id {Id}", command.Id);
            return Result.Fail(EntityNotFound.Error());
        }
        if (entity.Statut != DocumentStatus.Draft)
        {
            _logger.LogWarning("FactureDepense cannot be updated when not Draft: Id {Id}", command.Id);
            return Result.Fail("facture_depense_not_draft");
        }

        string? documentStoragePath = entity.DocumentStoragePath;
        if (!string.IsNullOrWhiteSpace(command.DocumentBase64))
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(entity.DocumentStoragePath))
                {
                    try
                    {
                        await _documentStorageService.DeleteAsync(entity.DocumentStoragePath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error deleting old document, continuing with new document");
                    }
                }
                var documentBytes = Convert.FromBase64String(command.DocumentBase64);
                var fileName = $"facture_depense_{entity.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}";
                documentStoragePath = await _documentStorageService.SaveAsync(documentBytes, fileName, cancellationToken);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Base64 format for document");
                return Result.Fail("invalid_document_format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing document");
                return Result.Fail("error_storing_document");
            }
        }

        entity.Update(command.Date, command.Description ?? string.Empty, command.MontantTotal, documentStoragePath);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("FactureDepense updated: Id {Id}", command.Id);
        return Result.Ok();
    }
}
