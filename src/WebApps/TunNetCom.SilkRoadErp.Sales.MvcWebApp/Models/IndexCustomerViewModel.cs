using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

namespace TunNetCom.SilkRoadErp.Sales.MvcWebApp.Models;

public class IndexCustomerViewModel
{
    public List<CustomerResponse> CustomerList { get; set; }

    public CustomerViewModel CurrentCustomer { get; set; }
}
