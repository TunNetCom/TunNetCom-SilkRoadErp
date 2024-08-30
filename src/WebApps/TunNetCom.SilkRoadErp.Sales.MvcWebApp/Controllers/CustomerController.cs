namespace TunNetCom.SilkRoadErp.Sales.MvcWebApp.Controllers;

[Authorize]
public class CustomerController(ICustomersApiClient _customersApiClient, ILogger<CustomerController> _logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        string searchQuery = "",
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        HttpContext.Session.SetString("LastSearchQuery", searchQuery);
        var indexCustomerViewModel = await PopulateIndexVM(page, pageSize, searchQuery, cancellationToken);

        _logger.LogInformation(
            "Result from customer action: {ActionName}, ViewModel: {ViewModelName}: {@ViewModel}",
            nameof(Index),
            nameof(IndexCustomerViewModel),
            @indexCustomerViewModel);

        return View(indexCustomerViewModel);
    }

    private async Task<IndexCustomerViewModel> PopulateIndexVM(
        int page,
        int pageSize,
        string searchQuery,
        CancellationToken cancellationToken)
    {
        var queryParameters = new QueryStringParameters
        {
            PageNumber = page,
            PageSize = pageSize,
            SearchKeyword = searchQuery
        };

        var pagedList = await _customersApiClient.GetAsync(queryParameters, cancellationToken);

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = pagedList.TotalCount;
        ViewBag.SearchQuery = searchQuery;

        var indexCustomerViewModel = new IndexCustomerViewModel
        {
            CustomerList = pagedList,
            CurrentCustomer = new CustomerViewModel()
        };

        _logger.LogInformation(
            "Result from customer action: {ActionName}, ViewModel: {ViewModelName}: {@ViewModel}",
            nameof(PopulateIndexVM),
            nameof(IndexCustomerViewModel),
            @indexCustomerViewModel);

        return indexCustomerViewModel;
    }

    [HttpPost]
    public async Task<IActionResult> Index(CustomerViewModel model, CancellationToken cancellationToken)
    {
        var searchQuery = HttpContext.Session.GetString("LastSearchQuery") ?? string.Empty;
        ViewBag.SearchQuery = searchQuery;

        var indexVM = await PopulateIndexVM(1, 10, searchQuery, cancellationToken);
        if (!ModelState.IsValid)
        {
            indexVM.CurrentCustomer = model;

            return View(indexVM);
        }

        if (model.Id == 0)
        {
            await CreateCustomer(model, cancellationToken);

            indexVM = await PopulateIndexVM(1, 10, searchQuery, cancellationToken);

            if (!ModelState.IsValid)
            {
                return View(indexVM);
            }

            return RedirectToAction("Index");
        }

        indexVM = await PopulateIndexVM(1, 10, searchQuery, cancellationToken);
        await UpdateCustomer(model, cancellationToken);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Error in saving customer, Errors: {@Error}", indexVM);

            return View(indexVM);
        }

        return RedirectToAction("Index");
    }

    private async Task UpdateCustomer(CustomerViewModel model, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateCustomerRequest
        {
            Nom = model.Nom,
            Tel = model.Tel,
            Adresse = model.Adresse,
            Matricule = model.Matricule,
            Code = model.Code,
            CodeCat = model.CodeCat,
            EtbSec = model.EtbSec,
            Mail = model.Mail
        };

        var result = await _customersApiClient.UpdateAsync(updateRequest, model.Id, cancellationToken);

        if (result.IsT1)
        {
            ModelState.AddModelError(
                string.Empty,
                result.AsT1.errors.FirstOrDefault().Value.FirstOrDefault());
        }
    }

    private async Task CreateCustomer(CustomerViewModel model, CancellationToken cancellationToken)
    {
        var createRequest = new CreateCustomerRequest
        {
            Nom = model.Nom,
            Tel = model.Tel,
            Adresse = model.Adresse,
            Matricule = model.Matricule,
            Code = model.Code,
            CodeCat = model.CodeCat,
            EtbSec = model.EtbSec,
            Mail = model.Mail
        };

        var result = await _customersApiClient.CreateAsync(createRequest, cancellationToken);

        if (result.IsT1)
        {
            ModelState.AddModelError(
                string.Empty,
                result.AsT1.errors.FirstOrDefault().Value.FirstOrDefault());

        }

    }

    [HttpPost]
    public async Task<IActionResult> DeleteCustomer(int id, CancellationToken cancellationToken)
    {
        await _customersApiClient.DeleteAsync(id, cancellationToken);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> EditCustomer(int id, CancellationToken cancellationToken)
    {
        var customerResponse = await _customersApiClient.GetAsync(id, cancellationToken);

        if (!customerResponse.IsT0)
        {
            return NotFound();
        }

        var customer = customerResponse.AsT0;

        var model = new CustomerViewModel
        {
            Id = customer.Id,
            Nom = customer.Nom,
            Tel = customer.Tel,
            Adresse = customer.Adresse,
            Matricule = customer.Matricule,
            Code = customer.Code,
            CodeCat = customer.CodeCat,
            EtbSec = customer.EtbSec,
            Mail = customer.Mail
        };

        return PartialView("_CustomerForm", model);
    }

    public IActionResult GetDeleteConfirmationViewComponent(int id)
    {
        return ViewComponent("CustomerDeleteConfirmationViewComponent", new { customerId = id });
    }
}