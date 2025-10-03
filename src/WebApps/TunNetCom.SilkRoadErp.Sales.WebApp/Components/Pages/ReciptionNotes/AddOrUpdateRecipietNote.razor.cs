using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Radzen.Blazor;
using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
using TunNetCom.SilkRoadErp.Sales.Contracts.Sorting;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Products;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ReceiptNote;
using TunNetCom.SilkRoadErp.Sales.WebApp.Locales;


namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.ReciptionNotes;

public partial class AddOrUpdateRecipietNote : ComponentBase
{
    [Parameter] public string? num { get; set; }

    [Inject] public NavigationManager NavigationManager { get; set; } = default!;
    [Inject] public IStringLocalizer<SharedResource> Localizer { get; set; } = default!;
    [Inject] public ToastService toastService { get; set; } = default!;
    [Inject] public NotificationService NotificationService { get; set; } = default!;
    [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] public IReceiptNoteApiClient receiptNoteApiClient { get; set; } = default!;
    [Inject] public IProductsApiClient productsApiClient { get; set; } = default!;
    [Inject] public IProviderInvoiceApiClient invoicesApiClient { get; set; } = default!;
    [Inject] public IProvidersApiClient providerApiClient { get; set; } = default!;
    [Inject] public IAppParametersClient appParametersClient { get; set; } = default!;
    [Inject] public ILogger<AddOrUpdateRecipietNote> logger { get; set; } = default!;
    [Inject] public DialogService DialogService { get; set; } = default!;

    RadzenDataGrid<DeliveryNoteDetailResponse> ordersGrid;
    List<DeliveryNoteDetailResponse> orders;
    List<DeliveryNoteDetailResponse> searchList;
    int count;
    private const int DefaultPageSize = 7;
    private CancellationTokenSource _cancellationTokenSource = new();
    private List<ProviderResponse> _filteredProviders { get; set; } = new();
    private List<ProviderInvoiceResponse> _filteredInvoices { get; set; } = new();
    List<KeyValuePair<int, string>> editedFields = new();
    DataGridEditMode editMode = DataGridEditMode.Single;
    List<DeliveryNoteDetailResponse> ordersToInsert = new();
    List<DeliveryNoteDetailResponse> ordersToUpdate = new();
    private bool isLoading = true;
    bool isLoadingCustomers = false;
    bool isLoadingInvoices = false;
    decimal totalHt;
    decimal totalVat;
    decimal totalTtc;
    string deliveryNoteNumber;
    DateTime deliveryNoteDate = DateTime.Now;
    private GetAppParametersResponse getAppParametersResponse;
    private int? selectedProviderId;
    int? selectedCustomerId
    {
        get => selectedProviderId;
        set
        {
            if (selectedProviderId != value)
            {
                selectedProviderId = value;
                selectedInvoiceId = null;
                _filteredInvoices = new List<ProviderInvoiceResponse>();
                _ = LoadCustomerInvoices(new LoadDataArgs());
                StateHasChanged();
            }
        }
    }

    private int? _selectedInvoiceId;
    int? selectedInvoiceId
    {
        get => _selectedInvoiceId;
        set
        {
            if (_selectedInvoiceId != value)
            {
                _selectedInvoiceId = value;
                StateHasChanged();
            }
        }
    }

    void Reset()
    {
        ordersToInsert.Clear();
        ordersToUpdate.Clear();
    }

    void Reset(DeliveryNoteDetailResponse order)
    {
        _ = ordersToInsert.Remove(order);
        _ = ordersToUpdate.Remove(order);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        searchList = new List<DeliveryNoteDetailResponse>();
        var param = await appParametersClient.GetAppParametersAsync(_cancellationTokenSource.Token);
        getAppParametersResponse = param.AsT0;
        await LoadCustomers(null);
        await FetchReciptionNote();
        await LoadCustomerInvoices(null);
        isLoading = false;
    }

    private async Task SaveDeliveryNote()
    {
        try
        {
            if (!selectedProviderId.HasValue)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = Localizer["error"],
                    Detail = $"{Localizer["select_customer"]}"
                });
                return;
            }
            if (!selectedProviderId.HasValue)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = Localizer["error"],
                    Detail = $"{Localizer["select_customer"]}"
                });
                return;
            }
            isLoading = true;
            StateHasChanged();

            var request = new CreateReceiptNoteRequest
            {
                Date = deliveryNoteDate,
                NumBonFournisseur = 1,
                DateLivraison = DateTime.Now,
                IdFournisseur = selectedProviderId ?? 0,
                NumFactureFournisseur = null,
            };

            var response = await receiptNoteApiClient.CreateReceiptNote(request, _cancellationTokenSource.Token);

            if (response.IsSuccess)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = Localizer["success"],
                    Detail = Localizer["delivery_note_saved_successfully"]
                });

                if (string.IsNullOrEmpty(deliveryNoteNumber))
                {
                    deliveryNoteNumber = response.ValueOrDefault.ToString();
                }
            }
            else
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = Localizer["error"],
                    Detail = $"{Localizer["failed_to_save_delivery_note"]}: {response.Errors.First().Message}"
                });
            }
        }
        catch (Exception ex)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = Localizer["error"],
                Detail = $"{Localizer["failed_to_save_delivery_note"]}: {ex.Message}"
            });
            logger.LogError(ex, "Failed to save delivery note");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task FetchReciptionNote()
    {
        if (string.IsNullOrEmpty(num))
        {
            orders = new List<DeliveryNoteDetailResponse>();
            totalHt = 0;
            totalVat = 0;
            totalTtc = 0;
            return;
        }

        try
        {
            if (!int.TryParse(num, out var numAsInt))
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = Localizer["error"],
                    Detail = $"{Localizer["receipt_note_not_found"]}"
                });

                return;
            }
            var recipietNote = await receiptNoteApiClient.GetReceiptNoteById(
                numAsInt,
                _cancellationTokenSource.Token);

            if (recipietNote.IsFailed)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = Localizer["error"],
                    Detail = $"{Localizer["receipt_note_not_found"]}: {recipietNote.Errors.First().Message}"
                });
                return;
            }

            var recipietNoteLignes = await receiptNoteApiClient.GetReceiptNoteLines(
                numAsInt,
                new GetReceiptNoteLinesWithSummariesQueryParams()
                {
                    PageNumber = 1,
                    PageSize = 5
                },
                _cancellationTokenSource.Token);

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception e)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = Localizer["error"],
                Detail = $"{Localizer["failed_to_fetch_delivery_note"]}: {e.Message}"
            });

            logger.LogError(e, "Failed to save delivery note");
            await InvokeAsync(StateHasChanged);
        }
    }

    async Task EditRow(DeliveryNoteDetailResponse order)
    {
        if (editMode == DataGridEditMode.Single && ordersToInsert.Count() > 0)
        {
            Reset();
        }

        ordersToUpdate.Add(order);
        await ordersGrid.EditRow(order);
    }

    async Task SaveRow(DeliveryNoteDetailResponse order)
    {
        if (order.ProductReference == "" || order.Quantity < 1)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = Localizer["error"],
                Detail = $"{Localizer["input_error"]}"
            });
            return;
        }
        try
        {
            await ordersGrid.UpdateRow(order);
        }
        catch (Exception e)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = Localizer["error"],
                Detail = $"{Localizer["failed_to_save_delivery_note"]}"
            });
            logger.LogError(e, "Failed to save delivery note");
        }
    }

    void CancelEdit(DeliveryNoteDetailResponse order)
    {
        Reset(order);
        ordersGrid.CancelEditRow(order);
    }

    async Task DeleteRow(DeliveryNoteDetailResponse order)
    {
        Reset(order);

        if (orders.Contains(order))
        {
            _ = orders.Remove(order);
            await ordersGrid.Reload();
        }
        else
        {
            ordersGrid.CancelEditRow(order);
            await ordersGrid.Reload();
        }

        UpdateTotals();
        StateHasChanged();
    }

    async Task InsertRow()
    {
        if (editMode == DataGridEditMode.Single)
        {
            Reset();
        }

        var order = new DeliveryNoteDetailResponse
        {
            Quantity = 1
        };
        ordersToInsert.Add(order);
        await ordersGrid.InsertRow(order);
    }

    private List<DeliveryNoteDetailResponse> GetCurrentProductList(DeliveryNoteDetailResponse currentOrder)
    {
        if (!string.IsNullOrEmpty(currentOrder.ProductReference))
        {
            var currentProduct = searchList.FirstOrDefault(p => p.ProductReference == currentOrder.ProductReference);
            if (currentProduct != null)
            {
                var list = new List<DeliveryNoteDetailResponse> { currentProduct };
                list.AddRange(searchList.Where(p => p.ProductReference != currentOrder.ProductReference));
                return list;
            }
        }
        return searchList;
    }

    async Task LoadData(LoadDataArgs args)
    {
        string _sortProperty = null;
        string _sortOrder = null;
        if (args.Sorts != null && args.Sorts.Any())
        {
            var sort = args.Sorts.First();
            _sortProperty = sort.Property;
            _sortOrder = sort.SortOrder == SortOrder.Ascending ? SortConstants.Ascending : SortConstants.Descending;
        }
        var parameters = new QueryStringParameters
        {
            PageNumber = (args.Skip.Value / DefaultPageSize) + 1,
            PageSize = DefaultPageSize,
            SearchKeyword = args.Filter,
            SortOrder = _sortOrder,
            SortProprety = _sortProperty
        };

        try
        {
            var pagedProducts = await productsApiClient.GetPagedAsync(parameters, _cancellationTokenSource.Token);
            searchList = pagedProducts.Items.Select(
                p => new DeliveryNoteDetailResponse
                {
                    ProductReference = p.Reference,
                    Description = p.Name,
                    UnitPriceExcludingTax = p.Price,
                    Quantity = 1,
                }).ToList();

            count = pagedProducts.TotalCount;

        }
        catch (Exception ex)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = Localizer["error"],
                Detail = $"{Localizer["failed_to_load_products"]}: {ex.Message}"
            });
            searchList = new List<DeliveryNoteDetailResponse>();
            count = 0;
        }

        await InvokeAsync(StateHasChanged);
    }

    async Task LoadCustomers(LoadDataArgs args)
    {
        isLoadingCustomers = true;
        var parameters = new QueryStringParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SearchKeyword = args?.Filter ?? string.Empty
        };

        try
        {
            var providerResult = await providerApiClient.GetPagedAsync(parameters, _cancellationTokenSource.Token);
            if (providerResult.TotalCount == 0)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = Localizer["no_customers_found"],
                    Detail = Localizer["try_different_search_criteria"]
                });
            }
            _filteredProviders = providerResult.Items;
        }
        catch (Exception ex)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = Localizer["error"],
                Detail = $"{Localizer["failed_to_load_customers"]}: {ex.Message}"
            });
            _filteredProviders = new List<ProviderResponse>();
        }

        isLoadingCustomers = false;
        await InvokeAsync(StateHasChanged);
    }

    async Task LoadCustomerInvoices(LoadDataArgs args)
    {
        if (!selectedCustomerId.HasValue)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = Localizer["select_a_customer_before"],
            });
            return;
        }

        isLoadingCustomers = true;
        var parameters = new QueryStringParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SearchKeyword = null
        };

        try
        {
            var pagedProviderInvoices = await invoicesApiClient.GetProvidersInvoicesAsync(
                selectedCustomerId.Value,
                parameters,
                _cancellationTokenSource.Token);

            _filteredInvoices = pagedProviderInvoices.AsT0.Invoices.Items.ToList();
        }
        catch (Exception ex)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = Localizer["error"],
                Detail = $"{Localizer["failed_to_load_customers"]}: {ex.Message}"
            });
            _filteredProviders = new List<ProviderResponse>();
        }

        isLoadingCustomers = false;
        await InvokeAsync(StateHasChanged);
    }

    async Task OnProductSelected(DeliveryNoteDetailResponse order, object value)
    {
        if (value is not null and string productReference)
        {
            var selectedProduct = GetCurrentProductList(order)
                .FirstOrDefault(p => p.ProductReference == productReference);

            if (selectedProduct != null)
            {
                order.Description = selectedProduct.Description;
                order.ProductReference = selectedProduct.ProductReference;
                order.UnitPriceExcludingTax = selectedProduct.UnitPriceExcludingTax;
                order.DiscountPercentage = getAppParametersResponse.DiscountPercentage;
                order.VatPercentage = getAppParametersResponse.VatAmount;
                CalculateTotals(order);
                UpdateTotals();
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private async Task OnValueChanged(DeliveryNoteDetailResponse order, object value, string propertyName)
    {
        switch (propertyName)
        {
            case nameof(order.Quantity):
                order.Quantity = Convert.ToInt16(value);
                break;
            case nameof(order.UnitPriceExcludingTax):
                order.UnitPriceExcludingTax = Convert.ToDecimal(value);
                break;
            case nameof(order.DiscountPercentage):
                break;
            case nameof(order.VatPercentage):
                break;
        }

        CalculateTotals(order);
        UpdateTotals();
        await InvokeAsync(StateHasChanged);
    }

    private void CalculateTotals(DeliveryNoteDetailResponse order)
    {
        if (order.Quantity > 0 && order.UnitPriceExcludingTax > 0)
        {
            decimal totalBeforeDiscount = order.Quantity * order.UnitPriceExcludingTax;
            decimal discountAmount = totalBeforeDiscount * (decimal)(order.DiscountPercentage / 100);
            order.TotalExcludingTax = totalBeforeDiscount - discountAmount;
            decimal vatAmount = order.TotalExcludingTax * (decimal)(order.VatPercentage / 100);
            order.TotalIncludingTax = order.TotalExcludingTax + vatAmount;
        }
        else
        {
            order.TotalExcludingTax = 0;
            order.TotalIncludingTax = 0;
        }
    }

    private void UpdateTotals()
    {
        totalHt = orders.Sum(o => o.TotalExcludingTax);
        totalVat = orders.Sum(o => o.TotalIncludingTax - o.TotalExcludingTax);
        totalTtc = orders.Sum(o => o.TotalIncludingTax);
    }

    void OnUpdateRow(DeliveryNoteDetailResponse order)
    {
        CalculateTotals(order);
        Reset(order);

        var toremv = orders.FirstOrDefault(t => t.Id == order.Id);
        if (toremv != null)
        {
            _ = orders.Remove(toremv);
        }
        orders.Add(order);
        UpdateTotals();
        StateHasChanged();
    }

    void OnCreateRow(DeliveryNoteDetailResponse order)
    {
        CalculateTotals(order);
        orders.Add(order);
        _ = ordersToInsert.Remove(order);
        UpdateTotals();
        StateHasChanged();
    }

    public async Task ShowDialog(string ProductReference)
    {
        await LoadStateAsync();

        await DialogService.OpenAsync<DialogHistory>($"ProductReference {ProductReference}",
               new Dictionary<string, object>() { { "ProductReference", ProductReference } },
               new DialogOptions()
               {
                   Resizable = true,
                   Draggable = true,
                   Resize = OnResize,
                   Drag = OnDrag,
                   Width = Settings != null ? Settings.Width : "700px",
                   Height = Settings != null ? Settings.Height : "512px",
                   Left = Settings != null ? Settings.Left : null,
                   Top = Settings != null ? Settings.Top : null
               });

        await SaveStateAsync();
    }

    void OnDrag(System.Drawing.Point point)
    {
        _ = JSRuntime.InvokeVoidAsync("eval", $"console.log('Dialog drag. Left:{point.X}, Top:{point.Y}')");

        Settings ??= new DialogSettings();

        Settings.Left = $"{point.X}px";
        Settings.Top = $"{point.Y}px";

        _ = InvokeAsync(SaveStateAsync);
    }

    void OnResize(System.Drawing.Size size)
    {
        _ = JSRuntime.InvokeVoidAsync("eval", $"console.log('Dialog resize. Width:{size.Width}, Height:{size.Height}')");

        Settings ??= new DialogSettings();

        Settings.Width = $"{size.Width}px";
        Settings.Height = $"{size.Height}px";

        _ = InvokeAsync(SaveStateAsync);
    }

    DialogSettings _settings;
    public DialogSettings Settings
    {
        get
        {
            return _settings;
        }
        set
        {
            if (_settings != value)
            {
                _settings = value;
                _ = InvokeAsync(SaveStateAsync);
            }
        }
    }

    private async Task LoadStateAsync()
    {
        await Task.CompletedTask;

        var result = await JSRuntime.InvokeAsync<string>("window.localStorage.getItem", "DialogSettings");
        if (!string.IsNullOrEmpty(result))
        {
            _settings = JsonSerializer.Deserialize<DialogSettings>(result);
        }
    }

    private async Task SaveStateAsync()
    {
        await Task.CompletedTask;

        await JSRuntime.InvokeVoidAsync("window.localStorage.setItem", "DialogSettings", JsonSerializer.Serialize<DialogSettings>(Settings));
    }

    public class DialogSettings
    {
        public string Left { get; set; }
        public string Top { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }
}
