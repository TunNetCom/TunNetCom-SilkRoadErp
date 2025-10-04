using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Radzen.Blazor;
using System.Text.Json;
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
    [Inject] public IProviderInvoiceApiClient providerInvoicesApiClient { get; set; } = default!;
    [Inject] public IProvidersApiClient providerApiClient { get; set; } = default!;
    [Inject] public IAppParametersClient appParametersClient { get; set; } = default!;
    [Inject] public ILogger<AddOrUpdateRecipietNote> logger { get; set; } = default!;
    [Inject] public DialogService DialogService { get; set; } = default!;

    RadzenDataGrid<ReceiptLineWrapper> receiptLinesGrid = default!;
    List<ReceiptLineWrapper> receiptLines = new();
    List<ReceiptLineWrapper> searchList = new();
    int count;
    private const int DefaultPageSize = 7;
    private CancellationTokenSource _cancellationTokenSource = new();
    private List<ProviderResponse> _filteredProviders { get; set; } = new();
    private List<ProviderInvoiceResponse> _filteredInvoices { get; set; } = new();
    List<KeyValuePair<int, string>> editedFields = new();
    DataGridEditMode editMode = DataGridEditMode.Single;
    List<ReceiptLineWrapper> linesToInsert = new();
    List<ReceiptLineWrapper> linesToUpdate = new();
    private bool isLoading = true;
    bool isLoadingProvider = false;
    bool isLoadingInvoices = false;
    decimal totalHt;
    decimal totalVat;
    decimal totalTtc;
    string receiptNoteNumber = string.Empty;
    DateTime receiptNoteDate = DateTime.Now;
    private GetAppParametersResponse getAppParametersResponse = default!;
    
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
                _filteredInvoices = new List<ProviderInvoiceResponse>();
                _ = LoadProviderInvoices(new LoadDataArgs());
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
        linesToInsert.Clear();
        linesToUpdate.Clear();
    }

    void Reset(ReceiptLineWrapper line)
    {
        _ = linesToInsert.Remove(line);
        _ = linesToUpdate.Remove(line);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var param = await appParametersClient.GetAppParametersAsync(_cancellationTokenSource.Token);
        getAppParametersResponse = param.AsT0;
        await LoadProviders(null);
        await FetchReciptNote();
        await LoadProviderInvoices(null);
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
            
            isLoading = true;
            StateHasChanged();

            var request = new CreateReceiptNoteRequest
            {
                Date = receiptNoteDate,
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

                if (string.IsNullOrEmpty(receiptNoteNumber))
                {
                    receiptNoteNumber = response.ValueOrDefault.ToString();
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

    private async Task FetchReciptNote()
    {
        if (string.IsNullOrEmpty(num))
        {
            receiptLines = new List<ReceiptLineWrapper>();
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

            selectedInvoiceId = recipietNote.Value.NumFactureFournisseur;
            selectedProviderId = recipietNote.Value.IdFournisseur;
            receiptNoteNumber = recipietNote.Value.Num.ToString();

            var recipietNoteLignes = await receiptNoteApiClient.GetReceiptNoteLines(
                numAsInt,
                new GetReceiptNoteLinesWithSummariesQueryParams()
                {
                    PageNumber = 1,
                    PageSize = 5
                },
                _cancellationTokenSource.Token);

            if (recipietNoteLignes.IsFailed)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = Localizer["error"],
                    Detail = $"{Localizer["failed_to_fetch_delivery_note"]}: {recipietNoteLignes.Errors.First().Message}"
                });
                return;
            }

            receiptLines = recipietNoteLignes.ValueOrDefault.ReceiptLinesBaseInfos.Items
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

            totalHt = recipietNoteLignes.ValueOrDefault.TotalNetAmount ;
            totalVat = recipietNoteLignes.ValueOrDefault.TotalVatAmount;
            totalTtc = recipietNoteLignes.ValueOrDefault.TotalGrossAmount;

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

    async Task EditRow(ReceiptLineWrapper line)
    {
        if (editMode == DataGridEditMode.Single && linesToInsert.Count() > 0)
        {
            Reset();
        }

        linesToUpdate.Add(line);
        await receiptLinesGrid.EditRow(line);
    }

    async Task SaveRow(ReceiptLineWrapper line)
    {
        if (line.ProductReference == "" || line.ItemQuantity < 1)
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
            await receiptLinesGrid.UpdateRow(line);
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

    void CancelEdit(ReceiptLineWrapper line)
    {
        Reset(line);
        receiptLinesGrid.CancelEditRow(line);
    }

    async Task DeleteRow(ReceiptLineWrapper line)
    {
        Reset(line);

        if (receiptLines.Contains(line))
        {
            _ = receiptLines.Remove(line);
            await receiptLinesGrid.Reload();
        }
        else
        {
            receiptLinesGrid.CancelEditRow(line);
            await receiptLinesGrid.Reload();
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

        var line = new ReceiptLineWrapper
        {
            ItemQuantity = 1
        };
        linesToInsert.Add(line);
        await receiptLinesGrid.InsertRow(line);
    }

    private List<ReceiptLineWrapper> GetCurrentProductList(ReceiptLineWrapper currentLine)
    {
        if (!string.IsNullOrEmpty(currentLine.ProductReference))
        {
            var currentProduct = searchList.FirstOrDefault(p => p.ProductReference == currentLine.ProductReference);
            if (currentProduct != null)
            {
                var list = new List<ReceiptLineWrapper> { currentProduct };
                list.AddRange(searchList.Where(p => p.ProductReference != currentLine.ProductReference));
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
                p => new ReceiptLineWrapper
                {
                    ProductReference = p.Reference,
                    ItemDescription = p.Name,
                    UnitPriceExcludingTax = p.Price,
                    ItemQuantity = 1,
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
            searchList = new List<ReceiptLineWrapper>();
            count = 0;
        }

        await InvokeAsync(StateHasChanged);
    }

    async Task LoadProviders(LoadDataArgs args)
    {
        isLoadingProvider = true;
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
                Detail = $"{Localizer["failed_to_load_providers"]}: {ex.Message}"
            });
            _filteredProviders = new List<ProviderResponse>();
        }

        isLoadingProvider = false;
        await InvokeAsync(StateHasChanged);
    }

    async Task LoadProviderInvoices(LoadDataArgs args)
    {
        if (!selectedProviderId.HasValue)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = Localizer["select_a_provider_before"],
            });
            return;
        }

        isLoadingProvider = true;

        try
        {
            var pagedProviderInvoices = await providerInvoicesApiClient.GetProvidersInvoicesAsync(
                selectedProviderId.Value,
                new QueryStringParameters
                {
                    PageNumber = 1,
                    PageSize = 5,
                    SearchKeyword = null
                },
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

        isLoadingProvider = false;
        await InvokeAsync(StateHasChanged);
    }

    async Task OnProductSelected(ReceiptLineWrapper line, object value)
    {
        if (value is not null and string productReference)
        {
            var selectedProduct = GetCurrentProductList(line)
                .FirstOrDefault(p => p.ProductReference == productReference);

            if (selectedProduct != null)
            {
                line.ItemDescription = selectedProduct.ItemDescription;
                line.ProductReference = selectedProduct.ProductReference;
                line.UnitPriceExcludingTax = selectedProduct.UnitPriceExcludingTax;
                line.Discount = getAppParametersResponse.DiscountPercentage;
                line.VatRate = getAppParametersResponse.VatAmount;
                CalculateTotals(line);
                UpdateTotals();
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private async Task OnValueChanged(ReceiptLineWrapper line, object value, string propertyName)
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

        CalculateTotals(line);
        UpdateTotals();
        await InvokeAsync(StateHasChanged);
    }

    private void CalculateTotals(ReceiptLineWrapper line)
    {
        if (line.ItemQuantity > 0 && line.UnitPriceExcludingTax > 0)
        {
            decimal totalBeforeDiscount = line.ItemQuantity * line.UnitPriceExcludingTax;
            decimal discountAmount = totalBeforeDiscount * (decimal)(line.Discount / 100);
            line.TotalExcludingTax = totalBeforeDiscount - discountAmount;
            decimal vatAmount = line.TotalExcludingTax * (decimal)(line.VatRate / 100);
            line.TotalIncludingTax = line.TotalExcludingTax + vatAmount;
        }
        else
        {
            line.TotalExcludingTax = 0;
            line.TotalIncludingTax = 0;
        }
    }

    private void UpdateTotals()
    {
        totalHt = receiptLines.Sum(l => l.TotalExcludingTax);
        totalVat = receiptLines.Sum(l => l.TotalIncludingTax - l.TotalExcludingTax);
        totalTtc = receiptLines.Sum(l => l.TotalIncludingTax);
    }

    void OnUpdateRow(ReceiptLineWrapper line)
    {
        CalculateTotals(line);
        Reset(line);

        var toremv = receiptLines.FirstOrDefault(t => t.LineId == line.LineId);
        if (toremv != null)
        {
            _ = receiptLines.Remove(toremv);
        }
        receiptLines.Add(line);
        UpdateTotals();
        StateHasChanged();
    }

    void OnCreateRow(ReceiptLineWrapper line)
    {
        CalculateTotals(line);
        receiptLines.Add(line);
        _ = linesToInsert.Remove(line);
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

    DialogSettings _settings = default!;
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
        public string Left { get; set; } = string.Empty;
        public string Top { get; set; } = string.Empty;
        public string Width { get; set; } = string.Empty;
        public string Height { get; set; } = string.Empty;
    }
}
