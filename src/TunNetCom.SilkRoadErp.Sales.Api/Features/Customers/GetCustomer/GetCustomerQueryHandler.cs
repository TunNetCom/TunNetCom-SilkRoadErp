using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;
using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomer;

public class GetCustomerQueryHandler(
    SalesContext _context,
    ILogger<GetCustomerQueryHandler> _logger)
    : IRequestHandler<GetCustomerQuery, PagedList<CustomerResponse>>
{
    public async Task<PagedList<CustomerResponse>> Handle(GetCustomerQuery getCustomerQuery, CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(Client), getCustomerQuery.PageNumber, getCustomerQuery.PageSize);

        var clientsQuery = _context.Client.Select(t =>
            new CustomerResponse
            {
                Nom = t.Nom,
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

        if (!string.IsNullOrEmpty(getCustomerQuery.SearchKeyword))
        {
            clientsQuery = clientsQuery.Where(
                c => c.Id.ToString().Contains(getCustomerQuery.SearchKeyword)
                || c.Nom.Contains(getCustomerQuery.SearchKeyword)
                || c.Tel.Contains(getCustomerQuery.SearchKeyword)
                || c.Adresse.Contains(getCustomerQuery.SearchKeyword)
                || c.Matricule.Contains(getCustomerQuery.SearchKeyword)
                || c.Code.Contains(getCustomerQuery.SearchKeyword)
                || c.CodeCat.Contains(getCustomerQuery.SearchKeyword)
                || c.EtbSec.Contains(getCustomerQuery.SearchKeyword)
                || c.Mail.Contains(getCustomerQuery.SearchKeyword));
        }

        var pagedCustomers = await PagedList<CustomerResponse>.ToPagedListAsync(
            clientsQuery,
            getCustomerQuery.PageNumber,
            getCustomerQuery.PageSize,
            cancellationToken);


        _logger.LogEntitiesFetched(nameof(Client), pagedCustomers.Count);

        return pagedCustomers;
    }
}