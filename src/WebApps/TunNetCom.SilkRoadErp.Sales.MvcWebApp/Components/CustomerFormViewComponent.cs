namespace TunNetCom.SilkRoadErp.Sales.MvcWebApp.Components;

public class CustomerFormViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(CustomerViewModel customer)
    {
        return View(customer);
    }
}
