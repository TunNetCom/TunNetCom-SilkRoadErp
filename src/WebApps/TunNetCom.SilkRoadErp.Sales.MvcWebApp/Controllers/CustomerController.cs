using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.MvcWebApp.Models;

namespace TunNetCom.SilkRoadErp.Sales.MvcWebApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomersApiClient _customersApiClient;

        public CustomerController(ICustomersApiClient customersApiClient)
        {
            _customersApiClient = customersApiClient;
        }

        // GET: Customer/Index
        [HttpGet]
        public async Task<IActionResult> Index(string searchQuery = "", int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var indexVM = await PopulateIndexVM(page, pageSize, searchQuery, cancellationToken);
            return View(indexVM);
        }

        private async Task<IndexCustomerViewModel> PopulateIndexVM(int page, int pageSize, string searchQuery, CancellationToken cancellationToken)
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

            var indexVM = new IndexCustomerViewModel
            {
                CustomerList = pagedList,
                CurrentCustomer = new CustomerViewModel()
            };

            return indexVM;
        }

        [HttpPost]
        public async Task<IActionResult> Index(CustomerViewModel model, CancellationToken cancellationToken)
        {
            var indexVM = await PopulateIndexVM(1, 10, string.Empty, cancellationToken);
            if (!ModelState.IsValid)
            {
                indexVM.CurrentCustomer = model;

                return View(indexVM);
            }

            if (model.Id == 0)
            {
                indexVM.CurrentCustomer = await CreateCustomer(model, cancellationToken);

                return View(indexVM);
            }

            indexVM.CurrentCustomer = await UpdateCustomer(model, cancellationToken);

            return View(indexVM);
        }

        private async Task<CustomerViewModel> UpdateCustomer(CustomerViewModel model, CancellationToken cancellationToken)
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
                    "",
                    result.AsT1.errors.FirstOrDefault().Value.FirstOrDefault());

                return model;
            }

            return new CustomerViewModel();
        }

        private async Task<CustomerViewModel> CreateCustomer(CustomerViewModel model, CancellationToken cancellationToken)
        {
            // Create new customer
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
                    "",
                    result.AsT1.errors.FirstOrDefault().Value.FirstOrDefault());

                return model;
            }

            return new CustomerViewModel();
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
    }
}
