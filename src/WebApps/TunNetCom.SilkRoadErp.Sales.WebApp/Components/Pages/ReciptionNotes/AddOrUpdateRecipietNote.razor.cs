using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
using TunNetCom.SilkRoadErp.Sales.Contracts.Sorting;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Products;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ReceiptNote;
using TunNetCom.SilkRoadErp.Sales.WebApp.Components.Shared;
using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;
using TunNetCom.SilkRoadErp.Sales.WebApp.Locales;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.ReciptionNotes;

public partial class AddOrUpdateRecipietNote : ComponentBase
{
    [Parameter] public string? num { get; set; }
    
    List<ReceiptNoteDetailResponse> orders;
    List<ReceiptNoteDetailResponse> searchList;
    int count;
    private const int DefaultPageSize = 7;
    private CancellationTokenSource _cancellationTokenSource = new();
    private List<ProviderResponse> _filteredProviders { get; set; } = new();
    private bool isLoading = true;
    bool isLoadingProvider = false;
    decimal totalHt;
    decimal totalVat;
    decimal totalTtc;
    string receiptNoteNumber;
    DateTime receiptNoteDate = DateTime.Now;
    private GetAppParametersResponse getAppParametersResponse;
    private int? _selectedProviderId;
    int? selectedProviderId
    {
        get => _selectedProviderId;
        set
        {
            if (_selectedProviderId != value)
            {
                _selectedProviderId = value;
                StateHasChanged();
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        searchList = new List<ReceiptNoteDetailResponse>();
        orders = new List<ReceiptNoteDetailResponse>();
        var param = await appParametersClient.GetAppParametersAsync(_cancellationTokenSource.Token);
        getAppParametersResponse = param.AsT0;
        await LoadProviders(null);
        isLoading = false;
    }

    protected override async Task OnParametersSetAsync()
    {
        await FetchReceiptNote();
        await base.OnParametersSetAsync();
    }

    private async Task SaveReceiptNote()
    {
        try
        {
            if (!_selectedProviderId.HasValue)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = Localizer["error"],
                    Detail = $"{Localizer["select_provider"]}"
                });
                return;
            }
            isLoading = true;
            StateHasChanged();

            var request = new CreateReceiptNoteWithLinesRequest
            {
                Date = receiptNoteDate,
                NumBonFournisseur = 0,
                DateLivraison = DateTime.Now,
                IdFournisseur = _selectedProviderId.Value,
                NumFactureFournisseur = null,
                ReceiptNoteLines = orders.Select(o => new ReceiptNoteLineRequest(
                    ProductRef: o.ProductReference,
                    ProductDescription: o.Description,
                    Quantity: o.Quantity,
                    UnitPrice: o.UnitPriceExcludingTax,
                    Discount: o.DiscountPercentage,
                    Tax: o.VatPercentage
                )).ToList()
            };

            int receiptNoteNum = 0;
            bool isEditMode = !string.IsNullOrEmpty(receiptNoteNumber) && int.TryParse(receiptNoteNumber, out receiptNoteNum);
            
            if (isEditMode)
            {
                // Update existing receipt note with lines
                var updateResponse = await receiptNoteApiClient.UpdateReceiptNoteWithLinesAsync(receiptNoteNum, request, _cancellationTokenSource.Token);
                
                if (updateResponse.IsSuccess)
                {
                    // Ensure receiptNoteNumber is set before fetching
                    receiptNoteNumber = receiptNoteNum.ToString();
                    
                    // Force reload by clearing orders first
                    orders = new List<ReceiptNoteDetailResponse>();
                    await InvokeAsync(StateHasChanged);
                    
                    await FetchReceiptNote();
                    
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = Localizer["success"],
                        Detail = Localizer["receipt_note_updated_successfully"] ?? Localizer["delivery_note_saved_successfully"]
                    });
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = Localizer["error"],
                        Detail = $"{Localizer["failed_to_save_delivery_note"]}: {updateResponse.Errors.First().Message}"
                    });
                }
            }
            else
            {
                var createResponse = await receiptNoteApiClient.CreateReceiptNoteWithLinesRequestTemplate(request);
                
                if (createResponse.IsSuccess)
                {
                    var newReceiptNoteNum = createResponse.ValueOrDefault;
                    receiptNoteNumber = newReceiptNoteNum.ToString();
                    
                    await FetchReceiptNote();
                    
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = Localizer["success"],
                        Detail = Localizer["delivery_note_saved_successfully"]
                    });
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = Localizer["error"],
                        Detail = $"{Localizer["failed_to_save_delivery_note"]}: {createResponse.Errors.First().Message}"
                    });
                }
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
            logger.LogError(ex, "Failed to save receipt note");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task FetchReceiptNote()
    {
        var receiptNoteNumToLoad = num;
        if (string.IsNullOrEmpty(receiptNoteNumToLoad) && !string.IsNullOrEmpty(receiptNoteNumber))
        {
            receiptNoteNumToLoad = receiptNoteNumber;
        }
        
        if (string.IsNullOrEmpty(receiptNoteNumToLoad))
        {
            orders = new List<ReceiptNoteDetailResponse>();
            totalHt = 0;
            totalVat = 0;
            totalTtc = 0;
            return;
        }

        try
        {
            if (int.TryParse(receiptNoteNumToLoad, out var numAsInt))
            {
                var receiptNote = await receiptNoteApiClient.GetReceiptNoteById(
                    numAsInt,
                    _cancellationTokenSource.Token);

                if (receiptNote != null && receiptNote.IsSuccess)
                {
                    receiptNoteNumber = receiptNote.Value.Num.ToString();
                    receiptNoteDate = receiptNote.Value.Date;
                    selectedProviderId = receiptNote.Value.IdFournisseur;

                    var linesResult = await receiptNoteApiClient.GetReceiptNoteLines(
                        numAsInt,
                        new GetReceiptNoteLinesWithSummariesQueryParams { PageNumber = 1, PageSize = 100 },
                        _cancellationTokenSource.Token);

                    if (linesResult.IsSuccess && linesResult.Value != null)
                    {
                        orders = linesResult.Value.ReceiptLinesBaseInfos.Items.Select(l => new ReceiptNoteDetailResponse
                        {
                            Id = l.LineId,
                            ProductReference = l.ProductReference,
                            Description = l.ItemDescription,
                            Quantity = l.ItemQuantity,
                            UnitPriceExcludingTax = l.UnitPriceExcludingTax,
                            DiscountPercentage = l.Discount,
                            TotalExcludingTax = l.TotalExcludingTax,
                            VatPercentage = l.VatRate,
                            TotalIncludingTax = l.TotalIncludingTax
                        }).ToList();

                        totalHt = linesResult.Value.TotalNetAmount;
                        totalVat = linesResult.Value.TotalVatAmount;
                        totalTtc = linesResult.Value.TotalGrossAmount;
                        
                        logger.LogInformation("Fetched receipt note {Num} with {LineCount} lines", numAsInt, orders.Count);
                    }
                    else
                    {
                        orders = new List<ReceiptNoteDetailResponse>();
                        totalHt = 0;
                        totalVat = 0;
                        totalTtc = 0;
                        logger.LogWarning("Failed to fetch lines for receipt note {Num}", numAsInt);
                    }
                }
            }

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

            logger.LogError(e, "Failed to fetch receipt note");
            await InvokeAsync(StateHasChanged);
        }
    }

    async Task DeleteRow(ReceiptNoteDetailResponse order)
    {
        if (orders.Contains(order))
        {
            orders.Remove(order);
        }
        else if (order.Id > 0)
        {
            var itemToRemove = orders.FirstOrDefault(t => t.Id == order.Id && t.Id > 0);
            if (itemToRemove != null)
            {
                orders.Remove(itemToRemove);
            }
        }

        UpdateTotals();
        await OnItemsChanged(orders);
        StateHasChanged();
    }

    private List<ReceiptNoteDetailResponse> GetCurrentProductList(ReceiptNoteDetailResponse currentOrder)
    {
        var resultList = new List<ReceiptNoteDetailResponse>();
        
        if (!string.IsNullOrEmpty(currentOrder.ProductReference))
        {
            var currentProduct = searchList.FirstOrDefault(p => p.ProductReference == currentOrder.ProductReference);
            
            if (currentProduct != null)
            {
                resultList.Add(currentProduct);
            }
            else
            {
                resultList.Add(new ReceiptNoteDetailResponse
                {
                    ProductReference = currentOrder.ProductReference,
                    Description = currentOrder.Description,
                    UnitPriceExcludingTax = currentOrder.UnitPriceExcludingTax,
                    Quantity = 1
                });
            }
            
            resultList.AddRange(searchList.Where(p => p.ProductReference != currentOrder.ProductReference));
        }
        else
        {
            resultList.AddRange(searchList);
        }
        
        return resultList;
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
                p => new ReceiptNoteDetailResponse
                {
                    ProductReference = p.Reference,
                    Description = p.Name,
                    UnitPriceExcludingTax = p.Price,
                    Quantity = 1,
                    DiscountPercentage = p.DiscountPourcentage,
                    VatPercentage = p.VatRate,
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
            searchList = new List<ReceiptNoteDetailResponse>();
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
            var pagedProviders = await providerApiClient.GetPagedAsync(parameters, _cancellationTokenSource.Token);
            _filteredProviders = pagedProviders.Items;
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

    async Task OnProductSelectedHandler(ReceiptNoteDetailResponse order)
    {
        UpdateTotals();
        await InvokeAsync(StateHasChanged);
    }

    private async Task PrintReceiptNote()
    {
        // TODO: Implement print functionality when available
        NotificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Info,
            Summary = Localizer["info"],
            Detail = Localizer["print_functionality_not_implemented"] ?? "Print functionality not yet implemented"
        });
    }

    async Task OnItemsChanged(List<ReceiptNoteDetailResponse> items)
    {
        orders = items;
        UpdateTotals();
        await InvokeAsync(StateHasChanged);
    }

    private void UpdateTotals()
    {
        LineItemCalculator.UpdateTotals(orders, out totalHt, out totalVat, out totalTtc);
    }

    public async Task ShowDialog(string ProductReference)
    {
        await LoadStateAsync();

        await DialogService.OpenAsync<ProductHistoryDialog>($"{Localizer["history"]} {Localizer["article"]} {ProductReference}",
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
        JSRuntime.InvokeVoidAsync("eval", $"console.log('Dialog drag. Left:{point.X}, Top:{point.Y}')");

        if (Settings == null)
        {
            Settings = new DialogSettings();
        }

        Settings.Left = $"{point.X}px";
        Settings.Top = $"{point.Y}px";

        InvokeAsync(SaveStateAsync);
    }

    void OnResize(System.Drawing.Size size)
    {
        JSRuntime.InvokeVoidAsync("eval", $"console.log('Dialog resize. Width:{size.Width}, Height:{size.Height}')");

        if (Settings == null)
        {
            Settings = new DialogSettings();
        }

        Settings.Width = $"{size.Width}px";
        Settings.Height = $"{size.Height}px";

        InvokeAsync(SaveStateAsync);
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
                InvokeAsync(SaveStateAsync);
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
