using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;

namespace TunNetCom.SilkRoadErp.Sales.MvcWebApp.Components;

[ViewComponent(Name = "CustomerDeleteConfirmationViewComponent")]
public class CustomerDeleteConfirmationViewComponent : ViewComponent
{
    private readonly ICustomersApiClient _customersApiClient;

    public CustomerDeleteConfirmationViewComponent(ICustomersApiClient customersApiClient)
    {
        _customersApiClient = customersApiClient;
    }

    public async Task<IViewComponentResult> InvokeAsync(int customerId)
    {
        var cts = new CancellationTokenSource();

        var customerResponse = await _customersApiClient.GetAsync(customerId, cts.Token);

        if (!customerResponse.IsT0)
        {
            return Content("Error loading customer data.");
        }

        var customer = customerResponse.AsT0;

        return View("Default", customer);
    }
}