using Microsoft.Extensions.Caching.Memory;

namespace TunNetCom.SilkRoadErp.Sales.MvcWebApp.Controllers;

[Authorize]
public class CustomerController : Controller
{
    private readonly ICustomersApiClient _customersApiClient;
    private readonly ILogger<CustomerController> _logger;
    private readonly IMemoryCache _memoryCache;
    private static readonly List<string> _cacheKeys = new List<string>();

    #region Actions
    public CustomerController(
        ICustomersApiClient customersApiClient,
        ILogger<CustomerController> logger,
        IMemoryCache memoryCache)
    {
        _customersApiClient = customersApiClient;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string searchQuery = "", int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        HttpContext.Session.SetString("LastSearchQuery", searchQuery);
        var indexCustomerViewModel = await PopulateIndexVM(page, pageSize, searchQuery, cancellationToken);

        _logger.LogInformation(
            "Result from customer action: {ActionName}, ViewModel: {ViewModelName}: {@ViewModel}",
            nameof(Index),
            nameof(IndexCustomerViewModel),
            indexCustomerViewModel);

        return View(indexCustomerViewModel);
    }

    private async Task<IndexCustomerViewModel> PopulateIndexVM(int page, int pageSize, string searchQuery, CancellationToken cancellationToken)
    {
        var cacheKey = $"CustomerList_{page}_{pageSize}_{searchQuery}";
        if (!_memoryCache.TryGetValue(cacheKey, out List<CustomerResponse> customerList))
        {
            var queryParameters = new QueryStringParameters
            {
                PageNumber = page,
                PageSize = pageSize,
                SearchKeyword = searchQuery
            };

            var pagedList = await _customersApiClient.GetAsync(queryParameters, cancellationToken);
            customerList = pagedList.ToList();

            // Cache aside implementation
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Adjust expiration as needed

            _memoryCache.Set(cacheKey, customerList, cacheEntryOptions);

            // Track the cache key
            _cacheKeys.Add(cacheKey);

            ViewBag.TotalItems = pagedList.TotalCount;
        }
        else
        {
            _logger.LogInformation("Customer list retrieved from cache.");
            ViewBag.TotalItems = customerList.Count;
        }

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.SearchQuery = searchQuery;

        var indexCustomerViewModel = new IndexCustomerViewModel
        {
            CustomerList = customerList,
            CurrentCustomer = new CustomerViewModel()
        };

        _logger.LogInformation(
            "Result from customer action: {ActionName}, ViewModel: {ViewModelName}: {@ViewModel}",
            nameof(PopulateIndexVM),
            nameof(IndexCustomerViewModel),
            indexCustomerViewModel);

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
            await RefreshCache();

            indexVM = await PopulateIndexVM(1, 10, searchQuery, cancellationToken);

            if (!ModelState.IsValid)
            {
                return View(indexVM);
            }

            return RedirectToAction("Index");
        }

        indexVM = await PopulateIndexVM(1, 10, searchQuery, cancellationToken);
        await UpdateCustomer(model, cancellationToken);
        await RefreshCache();

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Error in saving customer, Errors: {@Error}", indexVM);

            return View(indexVM);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCustomer(int id, CancellationToken cancellationToken)
    {
        await _customersApiClient.DeleteAsync(id, cancellationToken);
        await RefreshCache(); // Refresh the cache after deletion
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
    #endregion

    #region Private methods
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
            ModelState.AddModelError(string.Empty, result.AsT1.errors.FirstOrDefault().Value.FirstOrDefault());
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
            ModelState.AddModelError(string.Empty, result.AsT1.errors.FirstOrDefault().Value.FirstOrDefault());
        }
    }

    private Task RefreshCache()
    {
        // Invalidate the cache for all customer lists
        foreach (var cacheKey in _cacheKeys)
        {
            _memoryCache.Remove(cacheKey);
        }

        // Clear the cache keys list
        _cacheKeys.Clear();

        return Task.CompletedTask;
    }
    #endregion
}
