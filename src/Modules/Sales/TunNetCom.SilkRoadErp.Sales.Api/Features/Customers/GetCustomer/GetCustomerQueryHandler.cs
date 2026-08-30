namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomer;

public class GetCustomerQueryHandler(
    SalesContext _context,
    ILogger<GetCustomerQueryHandler> _logger)
    : IRequestHandler<GetCustomerQuery, PagedList<CustomerResponse>>
{
    public async Task<PagedList<CustomerResponse>> Handle(GetCustomerQuery getCustomerQuery, CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(Client), getCustomerQuery.PageNumber, getCustomerQuery.PageSize);

        var clientsQuery = _context.Client.AsNoTracking().Select(t =>
            new CustomerResponse
            {
                Name = t.Nom,
                Adresse = t.Adresse,
                Code = t.Code,
                CodeCat = t.CodeCat,
                EtbSec = t.EtbSec,
                Id = t.Id,
                Mail = t.Mail,
                Matricule = t.Matricule,
                Tel = t.Tel
            })
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(getCustomerQuery.SearchKeyword))
        {
            var keyword = getCustomerQuery.SearchKeyword.Trim();
            clientsQuery = clientsQuery.Where(
                c => c.Id.ToString().Contains(keyword)
                || (c.Name != null && c.Name.Contains(keyword))
                || (c.Tel != null && c.Tel.Contains(keyword))
                || (c.Adresse != null && c.Adresse.Contains(keyword))
                || (c.Matricule != null && c.Matricule.Contains(keyword))
                || (c.Code != null && c.Code.Contains(keyword))
                || (c.CodeCat != null && c.CodeCat.Contains(keyword))
                || (c.EtbSec != null && c.EtbSec.Contains(keyword))
                || (c.Mail != null && c.Mail.Contains(keyword)));
        }

        var pagedCustomers = await PagedList<CustomerResponse>.ToPagedListAsync(
            clientsQuery,
            getCustomerQuery.PageNumber,
            getCustomerQuery.PageSize,
            cancellationToken);


        _logger.LogEntitiesFetched(nameof(Client), pagedCustomers.Items.Count);

        return pagedCustomers;
    }
}