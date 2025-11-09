using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Radzen.Blazor;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Response;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
using TunNetCom.SilkRoadErp.Sales.Contracts.Sorting;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Products;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ReceiptNote;
using TunNetCom.SilkRoadErp.Sales.WebApp.Components.SharedHelper;
using TunNetCom.SilkRoadErp.Sales.WebApp.Locales;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.ReciptionNotes;

public partial class AddOrUpdateRecipietNote : ComponentBase
{
    #region Parameters and Dependencies
    [Parameter] public string? num { get; set; }

    [Inject] public IStringLocalizer<SharedResource> Localizer { get; set; } = default!;
    [Inject] public NotificationService NotificationService { get; set; } = default!;
    [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] public IReceiptNoteApiClient receiptNoteApiClient { get; set; } = default!;
    [Inject] public IProductsApiClient productsApiClient { get; set; } = default!;
    [Inject] public IProviderInvoiceApiClient providerInvoicesApiClient { get; set; } = default!;
    [Inject] public IProvidersApiClient providerApiClient { get; set; } = default!;
    [Inject] public IAppParametersClient appParametersClient { get; set; } = default!;
    [Inject] public ILogger<AddOrUpdateRecipietNote> logger { get; set; } = default!;
    [Inject] public DialogService DialogService { get; set; } = default!;
    #endregion

    #region State Management
    private readonly ReceiptNoteState _state = new();
    private readonly GridState _gridState = new();
    private readonly LoadingState _loadingState = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private GetAppParametersResponse _appParameters = default!;
    #endregion

    #region UI References
    RadzenDataGrid<ReceiptLineWrapper> receiptLinesGrid = default!;
    #endregion

    #region Properties
    private int? _selectedProviderId;
    int? selectedProviderId
    {
        get => _selectedProviderId;
        set
        {
            if (_selectedProviderId != value)
            {
                _selectedProviderId = value;
                selectedInvoiceId = null;
                _state.ClearInvoices();
                _ = LoadProviderInvoicesAsync(new LoadDataArgs());
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

    bool isLoading => _loadingState.IsPageLoading;
    bool isLoadingProvider => _loadingState.IsLoadingProviders;
    bool isLoadingInvoices => _loadingState.IsLoadingInvoices;
    List<ReceiptLineWrapper> receiptLines => _state.ReceiptLines;
    List<ReceiptLineWrapper> searchList => _gridState.ProductSearchList;
    int count => _gridState.ProductCount;
    DataGridEditMode editMode => _gridState.EditMode;
    List<ReceiptLineWrapper> linesToInsert => _gridState.LinesToInsert;
    List<ReceiptLineWrapper> linesToUpdate => _gridState.LinesToUpdate;
    List<ProviderResponse> _filteredProviders => _state.FilteredProviders;
    List<ProviderInvoiceResponse> _filteredInvoices => _state.FilteredInvoices;
    decimal totalHt => _state.TotalHt;
    decimal totalVat => _state.TotalVat;
    decimal totalTtc => _state.TotalTtc;
    string receiptNoteNumber
    {
        get => _state.ReceiptNoteNumber;
        set => _state.SetReceiptNoteNumber(value);
    }
    DateTime receiptNoteDate
    {
        get => _state.ReceiptNoteDate;
        set => _state.ReceiptNoteDate = value;
    }
    #endregion

    #region Lifecycle Methods
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await InitializeComponentAsync();
    }

    private async Task InitializeComponentAsync()
    {
        try
        {
            _appParameters = await LoadAppParametersAsync();
            await LoadProvidersAsync(null);
            await LoadReceiptNoteAsync();
            await LoadProviderInvoicesAsync(null);
        }
        finally
        {
            _loadingState.SetPageLoading(false);
        }
    }
    #endregion

    #region Data Loading Methods
    private async Task<GetAppParametersResponse> LoadAppParametersAsync()
    {
        var result = await appParametersClient.GetAppParametersAsync(_cancellationTokenSource.Token);
        return result.AsT0;
    }
 

    public async Task AddReceiptNoteWithLine()
    {
        if(receiptNoteNumber != string.Empty)
        {
            ShowErrorNotification(Localizer["receipt_note_already_exists"]);
            return;
        }

        _loadingState.SetPageLoading(true);
        StateHasChanged();
        try
        {
            var request = new CreateReceiptNoteWithLinesRequest
            {
                Date = _state.ReceiptNoteDate,
                NumBonFournisseur = 0,
                DateLivraison = DateTime.Now,
                IdFournisseur = selectedProviderId ?? 0,
                NumFactureFournisseur = selectedInvoiceId,
                ReceiptNoteLines = linesToInsert.Select(line => new ReceiptNoteLineRequest(
                
                    ProductRef : line.ProductReference,
                    ProductDescription : line.ItemDescription,
                    Quantity : line.ItemQuantity,
                    UnitPrice : line.UnitPriceExcludingTax,
                    Discount : line.Discount,
                    Tax : line.VatRate
                    
                )).ToList()
            };
            var response = await receiptNoteApiClient.CreateReceiptNoteWithLinesRequestTemplate(request);

            if (!response.IsSuccess)
            {
                ShowErrorNotification($"{Localizer["failed_to_save_delivery_note"]}: {response.Errors.First().Message}");
            }

            ShowSuccessNotification(Localizer["delivery_note_saved_successfully"]);
            receiptNoteNumber = response.Value.ToString();
            _state.SetReceiptNoteNumber(response.ValueOrDefault.ToString());
            linesToInsert.Clear();
            await LoadReceiptNoteAsync();

        }
        catch (Exception ex)
        {
            HandleSaveError(ex);
        }
        finally
        {
            _loadingState.SetPageLoading(false);
            StateHasChanged();
        }
    }

    public async Task AddOrUpdateReceiptNote()
    {
        if (!string.IsNullOrWhiteSpace(receiptNoteNumber) && !linesToInsert.Any() && !linesToUpdate.Any())
        {
            ShowErrorNotification(Localizer["no_changes_to_save"]);
            return;
        }

        if (!string.IsNullOrEmpty(receiptNoteNumber) && linesToUpdate.Any())
        {
            await UpdateReceiptNoteLines();
        }

        if (linesToInsert.Any() && !string.IsNullOrWhiteSpace(receiptNoteNumber))
        {
            await AddReceiptNoteLines();
        }
    }


    private async Task UpdateReceiptNoteLines()
    {

    }

    private async Task AddReceiptNoteLines()
    {
        var lines = linesToInsert.Select(x => new CreateReceiptNoteLineRequest(
            RecipetNoteNumber: int.Parse(receiptNoteNumber),
            ProductRef: x.ProductReference,
            ProductDescription: x.ItemDescription,
            Quantity: x.ItemQuantity,
            UnitPrice: x.UnitPriceExcludingTax,
            Discount: x.Discount,
            Tax: x.VatRate)).ToList();

        var response = await receiptNoteApiClient.CreateReceiptNoteLines(lines);

        if (!response.IsSuccess)
        {
            ShowErrorNotification($"{Localizer["failed_to_save_delivery_note"]}: {response.Errors.First().Message}");
            return;
        }
    }

    private async Task LoadReceiptNoteAsync()
    {
        if (string.IsNullOrEmpty(num))
        {
            _state.ClearReceiptNote();
            return;
        }

        if (!int.TryParse(num, out var numAsInt))
        {
            ShowErrorNotification(Localizer["receipt_note_not_found"]);
            return;
        }

        var result = await receiptNoteApiClient.GetReceiptNoteById(numAsInt, _cancellationTokenSource.Token);
        
        if (result.IsFailed)
        {
            ShowErrorNotification($"{Localizer["receipt_note_not_found"]}: {result.Errors.First().Message}");
            return;
        }

        await PopulateReceiptNoteDataAsync(result.Value, numAsInt);
    }

    private async Task PopulateReceiptNoteDataAsync(ReceiptNoteResponse receiptNote, int receiptNoteId)
    {
        _selectedProviderId = receiptNote.IdFournisseur;
        _state.SetReceiptNoteNumber(receiptNote.Num.ToString());
        
        await LoadProviderInvoicesAsync(new LoadDataArgs());
        
        LogInvoiceDebugInfo(receiptNote.NumFactureFournisseur);
        
        _selectedInvoiceId = receiptNote.NumFactureFournisseur;
        
        await LoadReceiptNoteLinesAsync(receiptNoteId);
        
        StateHasChanged();
    }

    private async Task LoadReceiptNoteLinesAsync(int receiptNoteId)
    {
        var linesResult = await receiptNoteApiClient.GetReceiptNoteLines(
            receiptNoteId,
            new GetReceiptNoteLinesWithSummariesQueryParams { PageNumber = 1, PageSize = 5 },
            _cancellationTokenSource.Token);

        if (linesResult.IsFailed)
        {
            ShowErrorNotification($"{Localizer["failed_to_fetch_delivery_note"]}: {linesResult.Errors.First().Message}");
            return;
        }

        _state.SetReceiptLines(linesResult.ValueOrDefault);
        await InvokeAsync(StateHasChanged);
    }

    // Wrapper methods for Razor binding (without Async suffix)
    Task LoadProviders(LoadDataArgs args) => LoadProvidersAsync(args);
    Task LoadProviderInvoices(LoadDataArgs args) => LoadProviderInvoicesAsync(args);

    async Task LoadProvidersAsync(LoadDataArgs args)
    {
        _loadingState.SetLoadingProviders(true);
        
        try
        {
            var parameters = CreateQueryParameters(1, 10, args?.Filter);
            var result = await providerApiClient.GetPagedAsync(parameters, _cancellationTokenSource.Token);
            
            if (result.TotalCount == 0)
            {
                ShowWarningNotification(Localizer["no_customers_found"], Localizer["try_different_search_criteria"]);
            }
            
            _state.SetProviders(result.Items);
        }
        catch (Exception ex)
        {
            HandleLoadError("failed_to_load_providers", ex);
        }
        finally
        {
            _loadingState.SetLoadingProviders(false);
            await InvokeAsync(StateHasChanged);
        }
    }

    async Task LoadProviderInvoicesAsync(LoadDataArgs args)
    {
        if (!_selectedProviderId.HasValue) return;

        _loadingState.SetLoadingInvoices(true);

        try
        {
            var parameters = CreateQueryParameters(1, 100);
            var result = await providerInvoicesApiClient.GetProvidersInvoicesAsync(
                _selectedProviderId.Value, parameters, _cancellationTokenSource.Token);

            _state.SetInvoices(result.AsT0.Invoices.Items.ToList());
        }
        catch (Exception ex)
        {
            HandleLoadError("failed_to_load_invoices", ex);
        }
        finally
        {
            _loadingState.SetLoadingInvoices(false);
            await InvokeAsync(StateHasChanged);
        }
    }

    async Task LoadData(LoadDataArgs args)
    {
        var (sortProperty, sortOrder) = ExtractSortParameters(args);
        var parameters = CreateQueryParameters(
            (args.Skip.Value / ReceiptNoteState.DefaultPageSize) + 1,
            ReceiptNoteState.DefaultPageSize,
            args.Filter,
            sortProperty,
            sortOrder);

        try
        {
            var pagedProducts = await productsApiClient.GetPagedAsync(parameters, _cancellationTokenSource.Token);
            _gridState.SetProductSearchResults(pagedProducts);
        }
        catch (Exception ex)
        {
            HandleLoadError("failed_to_load_products", ex);
        }

        await InvokeAsync(StateHasChanged);
    }
    #endregion

    #region Receipt Note Operations
    private async Task SaveDeliveryNote()
    {
        if (!ValidateProvider()) return;

        _loadingState.SetPageLoading(true);
        StateHasChanged();

        try
        {
            var request = CreateReceiptNoteRequest();
            var response = await receiptNoteApiClient.CreateReceiptNote(request, _cancellationTokenSource.Token);

            HandleSaveResponse(response);
        }
        catch (Exception ex)
        {
            HandleSaveError(ex);
        }
        finally
        {
            _loadingState.SetPageLoading(false);
            StateHasChanged();
        }
    }

    private bool ValidateProvider()
    {
        if (selectedProviderId.HasValue) return true;
        
        ShowErrorNotification(Localizer["select_customer"]);
        return false;
    }

    private CreateReceiptNoteRequest CreateReceiptNoteRequest()
    {
        return new CreateReceiptNoteRequest
        {
            Date = _state.ReceiptNoteDate,
            NumBonFournisseur = 1,
            DateLivraison = DateTime.Now,
            IdFournisseur = selectedProviderId ?? 0,
            NumFactureFournisseur = null,
        };
    }

    private void HandleSaveResponse(FluentResults.Result<long> response)
    {
        if (response.IsSuccess)
        {
            ShowSuccessNotification(Localizer["delivery_note_saved_successfully"]);
            
            if (string.IsNullOrEmpty(_state.ReceiptNoteNumber))
            {
                _state.SetReceiptNoteNumber(response.ValueOrDefault.ToString());
            }
        }
        else
        {
            ShowErrorNotification($"{Localizer["failed_to_save_delivery_note"]}: {response.Errors.First().Message}");
        }
    }
    #endregion

    #region Grid Operations
    async Task EditRow(ReceiptLineWrapper line) => await _gridState.EditRowAsync(receiptLinesGrid, line);
    void CancelEdit(ReceiptLineWrapper line) => _gridState.CancelEdit(receiptLinesGrid, line);
    async Task InsertRow() => await _gridState.InsertRowAsync(receiptLinesGrid);
    
    async Task SaveRow(ReceiptLineWrapper line)
    {
        if (!ValidateReceiptLine(line)) return;

        try
        {
            await receiptLinesGrid.UpdateRow(line);
        }
        catch (Exception e)
        {
            HandleSaveError(e);
        }
    }

    async Task DeleteRow(ReceiptLineWrapper line)
    {
        _gridState.ResetLineTracking(line);

        if (_state.ReceiptLines.Contains(line))
        {
            _state.RemoveLine(line);
            await receiptLinesGrid.Reload();
        }
        else
        {
            receiptLinesGrid.CancelEditRow(line);
            await receiptLinesGrid.Reload();
        }

        _state.UpdateTotals();
        StateHasChanged();
    }

    void OnUpdateRow(ReceiptLineWrapper line)
    {
        ReceiptLineCalculator.CalculateTotals(line);
        _gridState.ResetLineTracking(line);
        _state.UpdateLine(line);
        StateHasChanged();
    }

    void OnCreateRow(ReceiptLineWrapper line)
    {
        ReceiptLineCalculator.CalculateTotals(line);
        _state.AddLine(line);
        _gridState.RemoveFromInsertTracking(line);
        StateHasChanged();
    }

    void Reset() => _gridState.ResetAll();
    #endregion

    #region Product Selection
    async Task OnProductSelected(ReceiptLineWrapper line, object value)
    {
        if (value is not string productReference) return;

        var selectedProduct = _gridState.FindProduct(productReference, line);
        if (selectedProduct == null) return;

        ProductLinePopulator.PopulateLineFromProduct(line, selectedProduct, _appParameters);
        _state.UpdateTotals();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnValueChanged(ReceiptLineWrapper line, object value, string propertyName)
    {
        ReceiptLineUpdater.UpdateProperty(line, value, propertyName);
        ReceiptLineCalculator.CalculateTotals(line);
        _state.UpdateTotals();
        await InvokeAsync(StateHasChanged);
    }

    private List<ReceiptLineWrapper> GetCurrentProductList(ReceiptLineWrapper currentLine) =>
        _gridState.GetProductListWithCurrent(currentLine);
    #endregion

    #region Dialog Management
    public async Task ShowDialog(string ProductReference)
    {
        var settings = await DialogSettingsManager.LoadSettingsAsync(JSRuntime);
        
        await DialogService.OpenAsync<DialogHistory>(
            $"ProductReference {ProductReference}",
            new Dictionary<string, object> { { "ProductReference", ProductReference } },
            DialogSettingsManager.CreateDialogOptions(settings, OnResize, OnDrag));

        await DialogSettingsManager.SaveSettingsAsync(JSRuntime, settings);
    }

    void OnDrag(System.Drawing.Point point) => 
        DialogSettingsManager.OnDrag(JSRuntime, point, ref _settings);

    void OnResize(System.Drawing.Size size) => 
        DialogSettingsManager.OnResize(JSRuntime, size, ref _settings);

    DialogSettings _settings = default!;
    public DialogSettings Settings
    {
        get => _settings;
        set
        {
            if (_settings != value)
            {
                _settings = value;
                _ = DialogSettingsManager.SaveSettingsAsync(JSRuntime, value);
            }
        }
    }
    #endregion

    #region Helper Methods
    private bool ValidateReceiptLine(ReceiptLineWrapper line)
    {
        if (string.IsNullOrEmpty(line.ProductReference) || line.ItemQuantity < 1)
        {
            ShowErrorNotification(Localizer["input_error"]);
            return false;
        }
        return true;
    }

    private QueryStringParameters CreateQueryParameters(
        int pageNumber, 
        int pageSize, 
        string? searchKeyword = null,
        string? sortProperty = null,
        string? sortOrder = null)
    {
        return new QueryStringParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchKeyword = searchKeyword,
            SortProprety = sortProperty,
            SortOrder = sortOrder
        };
    }

    private (string? property, string? order) ExtractSortParameters(LoadDataArgs args)
    {
        if (args.Sorts == null || !args.Sorts.Any()) 
            return (null, null);

        var sort = args.Sorts.First();
        var sortOrder = sort.SortOrder == SortOrder.Ascending 
            ? SortConstants.Ascending 
            : SortConstants.Descending;
        
        return (sort.Property, sortOrder);
    }

    private void LogInvoiceDebugInfo(int? invoiceId)
    {
        logger.LogInformation($"Receipt Note Invoice ID: {invoiceId}");
        logger.LogInformation($"Loaded {_state.FilteredInvoices.Count} invoices");
        
        if (_state.FilteredInvoices.Any())
        {
            logger.LogInformation($"Invoice IDs in list: {string.Join(", ", _state.FilteredInvoices.Select(i => i.Num))}");
        }
    }

    private void ShowSuccessNotification(string detail) =>
        NotificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = Localizer["success"],
            Detail = detail
        });

    private void ShowErrorNotification(string detail) =>
        NotificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Error,
            Summary = Localizer["error"],
            Detail = detail
        });

    private void ShowWarningNotification(string summary, string detail) =>
        NotificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Warning,
            Summary = summary,
            Detail = detail
        });

    private void HandleLoadError(string errorKey, Exception ex)
    {
        ShowErrorNotification($"{Localizer[errorKey]}: {ex.Message}");
        logger.LogError(ex, errorKey);
    }

    private void HandleSaveError(Exception ex)
    {
        ShowErrorNotification($"{Localizer["failed_to_save_delivery_note"]}: {ex.Message}");
        logger.LogError(ex, "Failed to save delivery note");
    }
    #endregion

    #region Nested Classes - State Management (SRP)
    private class ReceiptNoteState
    {
        public const int DefaultPageSize = 7;
        
        public List<ReceiptLineWrapper> ReceiptLines { get; private set; } = new();
        public List<ProviderResponse> FilteredProviders { get; private set; } = new();
        public List<ProviderInvoiceResponse> FilteredInvoices { get; private set; } = new();
        public string ReceiptNoteNumber { get; private set; } = string.Empty;
        public DateTime ReceiptNoteDate { get; set; } = DateTime.Now;
        public decimal TotalHt { get; private set; }
        public decimal TotalVat { get; private set; }
        public decimal TotalTtc { get; private set; }

        public void SetProviders(List<ProviderResponse> providers) => FilteredProviders = providers;
        public void SetInvoices(List<ProviderInvoiceResponse> invoices) => FilteredInvoices = invoices;
        public void ClearInvoices() => FilteredInvoices = new List<ProviderInvoiceResponse>();
        public void SetReceiptNoteNumber(string number) => ReceiptNoteNumber = number;
        
        public void SetReceiptLines(GetReceiptNoteLinesByReceiptNoteIdResponse response)
        {
            ReceiptLines = response.ReceiptLinesBaseInfos.Items
                .Select(l => new ReceiptLineWrapper
                {
                    LineId = l.LineId,
                    ProductReference = l.ProductReference,
                    ItemDescription = l.ItemDescription,
                    ItemQuantity = l.ItemQuantity,
                    UnitPriceExcludingTax = l.UnitPriceExcludingTax,
                    Discount = l.Discount,
                    TotalExcludingTax = l.TotalExcludingTax,
                    VatRate = l.VatRate,
                    TotalIncludingTax = l.TotalIncludingTax
                }).ToList();

            TotalHt = response.TotalNetAmount;
            TotalVat = response.TotalVatAmount;
            TotalTtc = response.TotalGrossAmount;
        }

        public void ClearReceiptNote()
        {
            ReceiptLines = new List<ReceiptLineWrapper>();
            TotalHt = TotalVat = TotalTtc = 0;
        }

        public void UpdateTotals()
        {
            TotalHt = ReceiptLines.Sum(l => l.TotalExcludingTax);
            TotalVat = ReceiptLines.Sum(l => l.TotalIncludingTax - l.TotalExcludingTax);
            TotalTtc = ReceiptLines.Sum(l => l.TotalIncludingTax);
        }

        public void AddLine(ReceiptLineWrapper line)
        {
            ReceiptLines.Add(line);
            UpdateTotals();
        }

        public void RemoveLine(ReceiptLineWrapper line)
        {
            _ = ReceiptLines.Remove(line);
            UpdateTotals();
        }

        public void UpdateLine(ReceiptLineWrapper line)
        {
            var existing = ReceiptLines.FirstOrDefault(l => l.LineId == line.LineId);
            if (existing != null)
            {
                _ = ReceiptLines.Remove(existing);
            }
            ReceiptLines.Add(line);
            UpdateTotals();
        }
    }

    private class GridState
    {
        public DataGridEditMode EditMode { get; } = DataGridEditMode.Single;
        public List<ReceiptLineWrapper> ProductSearchList { get; private set; } = new();
        public int ProductCount { get; private set; }
        public List<ReceiptLineWrapper> LinesToInsert { get; } = new();
        public List<ReceiptLineWrapper> LinesToUpdate { get; } = new();

        public void SetProductSearchResults(PagedList<ProductResponse> pagedProducts)
        {
            ProductSearchList = pagedProducts.Items.Select(p => new ReceiptLineWrapper
            {
                ProductReference = p.Reference,
                ItemDescription = p.Name,
                UnitPriceExcludingTax = p.Price,
                ItemQuantity = 1,
            }).ToList();
            ProductCount = pagedProducts.TotalCount;
        }

        public async Task EditRowAsync(RadzenDataGrid<ReceiptLineWrapper> grid, ReceiptLineWrapper line)
        {
            if (EditMode == DataGridEditMode.Single && LinesToInsert.Any())
            {
                ResetAll();
            }
            LinesToUpdate.Add(line);
            await grid.EditRow(line);
        }

        public void CancelEdit(RadzenDataGrid<ReceiptLineWrapper> grid, ReceiptLineWrapper line)
        {
            ResetLineTracking(line);
            grid.CancelEditRow(line);
        }

        public async Task InsertRowAsync(RadzenDataGrid<ReceiptLineWrapper> grid)
        {
            if (EditMode == DataGridEditMode.Single)
            {
                ResetAll();
            }

            var line = new ReceiptLineWrapper { ItemQuantity = 1 };
            LinesToInsert.Add(line);
            await grid.InsertRow(line);
        }

        public void ResetLineTracking(ReceiptLineWrapper line)
        {
            _ = LinesToInsert.Remove(line);
            _ = LinesToUpdate.Remove(line);
        }

        public void ResetAll()
        {
            LinesToInsert.Clear();
            LinesToUpdate.Clear();
        }

        public void RemoveFromInsertTracking(ReceiptLineWrapper line) => LinesToInsert.Remove(line);

        public ReceiptLineWrapper? FindProduct(string productReference, ReceiptLineWrapper currentLine) =>
            GetProductListWithCurrent(currentLine).FirstOrDefault(p => p.ProductReference == productReference);

        public List<ReceiptLineWrapper> GetProductListWithCurrent(ReceiptLineWrapper currentLine)
        {
            if (string.IsNullOrEmpty(currentLine.ProductReference))
                return ProductSearchList;

            var currentProduct = ProductSearchList.FirstOrDefault(p => p.ProductReference == currentLine.ProductReference);
            if (currentProduct == null)
                return ProductSearchList;

            var list = new List<ReceiptLineWrapper> { currentProduct };
            list.AddRange(ProductSearchList.Where(p => p.ProductReference != currentLine.ProductReference));
            return list;
        }
    }

    private class LoadingState
    {
        public bool IsPageLoading { get; private set; } = true;
        public bool IsLoadingProviders { get; private set; }
        public bool IsLoadingInvoices { get; private set; }

        public void SetPageLoading(bool isLoading) => IsPageLoading = isLoading;
        public void SetLoadingProviders(bool isLoading) => IsLoadingProviders = isLoading;
        public void SetLoadingInvoices(bool isLoading) => IsLoadingInvoices = isLoading;
    }
    #endregion

    #region Nested Classes - Business Logic (SRP)
    private static class ReceiptLineCalculator
    {
        public static void CalculateTotals(ReceiptLineWrapper line)
        {
            if (line.ItemQuantity <= 0 || line.UnitPriceExcludingTax <= 0)
            {
                line.TotalExcludingTax = 0;
                line.TotalIncludingTax = 0;
                return;
            }

            var totalBeforeDiscount = line.ItemQuantity * line.UnitPriceExcludingTax;
            var discountAmount = totalBeforeDiscount * (decimal)(line.Discount / 100);
            line.TotalExcludingTax = totalBeforeDiscount - discountAmount;
            var vatAmount = line.TotalExcludingTax * (decimal)(line.VatRate / 100);
            line.TotalIncludingTax = line.TotalExcludingTax + vatAmount;
        }
    }

    private static class ReceiptLineUpdater
    {
        public static void UpdateProperty(ReceiptLineWrapper line, object value, string propertyName)
        {
            switch (propertyName)
            {
                case nameof(line.ItemQuantity):
                    line.ItemQuantity = Convert.ToInt16(value);
                    break;
                case nameof(line.UnitPriceExcludingTax):
                    line.UnitPriceExcludingTax = Convert.ToDecimal(value);
                    break;
                case nameof(line.Discount):
                    line.Discount = Convert.ToDouble(value);
                    break;
                case nameof(line.VatRate):
                    line.VatRate = Convert.ToDouble(value);
                    break;
            }
        }
    }

    private static class ProductLinePopulator
    {
        public static void PopulateLineFromProduct(
            ReceiptLineWrapper line, 
            ReceiptLineWrapper product, 
            GetAppParametersResponse appParameters)
        {
            line.ItemDescription = product.ItemDescription;
            line.ProductReference = product.ProductReference;
            line.UnitPriceExcludingTax = product.UnitPriceExcludingTax;
            line.Discount = appParameters.DiscountPercentage;
            line.VatRate = appParameters.VatAmount;
            ReceiptLineCalculator.CalculateTotals(line);
        }
    }

    public class DialogSettings
    {
        public string Left { get; set; } = string.Empty;
        public string Top { get; set; } = string.Empty;
        public string Width { get; set; } = string.Empty;
        public string Height { get; set; } = string.Empty;
    }
    #endregion
}
