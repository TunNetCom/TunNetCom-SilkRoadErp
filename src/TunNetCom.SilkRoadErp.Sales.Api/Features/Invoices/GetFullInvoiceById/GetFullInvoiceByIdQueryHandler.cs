namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetFullInvoiceById;


public class GetFullInvoiceByIdQueryHandler(
    SalesContext _context,
    ILogger<GetFullInvoiceByIdQueryHandler> _logger)
    : IRequestHandler<GetFullInvoiceByIdQuery, Result<FullInvoiceResponse>>
{
    public async Task<Result<FullInvoiceResponse>> Handle(
        GetFullInvoiceByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(
            $"{nameof(Facture)} including {nameof(BonDeLivraison)}s, {nameof(LigneBl)}s and {nameof(Client)}",
            query.Id);
        FullInvoiceResponse? invoice = await GetFullInvoiceByIdAsync(query, cancellationToken);

        if (invoice is null)
        {
            _logger.LogEntityNotFound(
                $"{nameof(Facture)} including {nameof(BonDeLivraison)}s, {nameof(LigneBl)}s and {nameof(Client)}",
                query.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        _logger.LogEntityFetchedById(
            $"{nameof(Facture)} including {nameof(BonDeLivraison)}s, {nameof(LigneBl)}s and {nameof(Client)}",
            query.Id);

        return invoice;
    }

    private async Task<FullInvoiceResponse?> GetFullInvoiceByIdAsync(
        GetFullInvoiceByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await _context.Facture
               .AsNoTracking()
               .Where(f => f.Num == query.Id)
               .Include(f => f.IdClientNavigation)
               .Include(f => f.BonDeLivraison)
                   .ThenInclude(bl => bl.LigneBl)
               .Select(f => new FullInvoiceResponse
               {
                   Num = f.Num,
                   Date = f.Date,
                   IdClient = f.IdClient,
                   Client = f.IdClientNavigation != null ? new FullInvoiceCustomerResponse
                   {
                       Id = f.IdClientNavigation.Id,
                       Nom = f.IdClientNavigation.Nom,
                       Tel = f.IdClientNavigation.Tel,
                       Adresse = f.IdClientNavigation.Adresse,
                       Matricule = f.IdClientNavigation.Matricule,
                       Code = f.IdClientNavigation.Code,
                       CodeCat = f.IdClientNavigation.CodeCat,
                       EtbSec = f.IdClientNavigation.EtbSec,
                       Mail = f.IdClientNavigation.Mail
                   } : null!,
                   DeliveryNotes = f.BonDeLivraison.Select(bl => new FullInvoiceCustomerResponseDeliveryNoteResponse
                   {
                       Num = bl.Num,
                       Date = bl.Date,
                       TotHTva = bl.TotHTva,
                       TotTva = bl.TotTva,
                       NetPayer = bl.NetPayer,
                       TempBl = bl.TempBl,
                       ClientId = bl.ClientId,
                       Lines = bl.LigneBl.Select(li => new DeliveryNoteLineResponse
                       {
                           IdLi = li.IdLi,
                           RefProduit = li.RefProduit,
                           DesignationLi = li.DesignationLi,
                           QteLi = li.QteLi,
                           PrixHt = li.PrixHt,
                           Remise = li.Remise,
                           TotHt = li.TotHt,
                           Tva = li.Tva,
                           TotTtc = li.TotTtc
                       }).ToList()
                   }).ToList()
               })
               .FirstOrDefaultAsync(cancellationToken);
    }
}