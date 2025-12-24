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
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Tags;
using TunNetCom.SilkRoadErp.Sales.WebApp.Components.Shared;
using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;
using TunNetCom.SilkRoadErp.Sales.WebApp.Locales;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

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
    decimal totalFodec;
    string receiptNoteNumber;
    private TagSelector? tagSelectorRef;
    DateTime receiptNoteDate = DateTime.Now;
    long numBonFournisseur = 0;
    DateTime dateLivraison = DateTime.Now;
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
                UpdateTotals(); // Recalculate FODEC when provider changes
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
                NumBonFournisseur = numBonFournisseur,
                DateLivraison = dateLivraison,
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
                    await InvokeAsync(StateHasChanged);
                    
                    // Force reload by clearing orders first
                    orders = new List<ReceiptNoteDetailResponse>();
                    await InvokeAsync(StateHasChanged);
                    
                    await FetchReceiptNote();
                    
                    // Sauvegarder les tags après la mise à jour du document
                    await SaveTags();
                    
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = Localizer["success"],
                        Detail = Localizer["receipt_note_updated_successfully"] ?? Localizer["delivery_note_saved_successfully"]
                    });
                }
                else
                {
                    var errorMessage = updateResponse.Errors.First().Message;
                    var localizedError = errorMessage == "not_found" 
                        ? Localizer["receipt_note_not_found"] ?? "Bon de réception non trouvé"
                        : errorMessage;
                    
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = Localizer["error"],
                        Detail = $"{Localizer["failed_to_save_receipt_note"] ?? "Échec de l'enregistrement du bon de réception"}: {localizedError}"
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
                    await InvokeAsync(StateHasChanged);
                    
                    await FetchReceiptNote();
                    
                    // Attendre un peu pour que le TagSelector soit mis à jour avec le nouveau DocumentId
                    await Task.Delay(100);
                    
                    // Sauvegarder les tags après la création du document
                    await SaveTags();
                    
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = Localizer["success"],
                        Detail = Localizer["delivery_note_saved_successfully"]
                    });
                }
                else
                {
                    var errorMessage = createResponse.Errors.First().Message;
                    var localizedError = errorMessage == "not_found" 
                        ? Localizer["receipt_note_not_found"] ?? "Bon de réception non trouvé"
                        : errorMessage;
                    
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = Localizer["error"],
                        Detail = $"{Localizer["failed_to_save_receipt_note"] ?? "Échec de l'enregistrement du bon de réception"}: {localizedError}"
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
            // Pour un nouveau bon de réception, initialiser avec une ligne vide pour permettre la saisie
            orders = new List<ReceiptNoteDetailResponse> { new ReceiptNoteDetailResponse { Quantity = 1 } };
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
                    numBonFournisseur = receiptNote.Value.NumBonFournisseur;
                    dateLivraison = receiptNote.Value.DateLivraison;
                    selectedProviderId = receiptNote.Value.IdFournisseur;

                    // Ensure the selected provider is loaded in _filteredProviders for FODEC calculation
                    if (selectedProviderId.HasValue && !_filteredProviders.Any(p => p.Id == selectedProviderId.Value))
                    {
                        try
                        {
                            var providerResult = await providerApiClient.GetAsync(selectedProviderId.Value, _cancellationTokenSource.Token);
                            if (providerResult.IsT0)
                            {
                                var provider = providerResult.AsT0;
                                _filteredProviders.Add(provider);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning(ex, "Failed to load provider {ProviderId} for FODEC calculation", selectedProviderId.Value);
                        }
                    }

                    // Use Items directly from ReceiptNoteResponse (like DeliveryNote)
                    if (receiptNote.Value.Items != null && receiptNote.Value.Items.Any())
                    {
                        orders = receiptNote.Value.Items.ToList();
                        
                        // FODEC is already calculated and included in TotalIncludingTax by the backend
                        // Just recalculate to ensure consistency if provider changed
                        CalculateFodecForItems();
                        
                        // Calculate totals from items (after FODEC calculation)
                        totalHt = orders.Sum(o => o.TotalExcludingTax);
                        totalVat = orders.Sum(o => o.TotalIncludingTax - o.TotalExcludingTax - (o.PrixHtFodec ?? 0));
                        totalFodec = orders.Sum(o => o.PrixHtFodec ?? 0);
                        totalTtc = orders.Sum(o => o.TotalIncludingTax);
                        
                        logger.LogInformation("Fetched receipt note {Num} with {LineCount} lines, TotalTTC: {TotalTtc}, TotalFODEC: {TotalFodec}", 
                            numAsInt, orders.Count, totalTtc, totalFodec);
                    }
                    else
                    {
                        orders = new List<ReceiptNoteDetailResponse>();
                        totalHt = 0;
                        totalVat = 0;
                        totalTtc = 0;
                        logger.LogInformation("Receipt note {Num} has no lines", numAsInt);
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
            searchList = (pagedProducts?.Items ?? new List<ProductResponse>()).Select(
                p => new ReceiptNoteDetailResponse
                {
                    ProductReference = p.Reference,
                    Description = p.Name,
                    UnitPriceExcludingTax = p.Price,
                    Quantity = 1,
                    DiscountPercentage = p.DiscountPourcentage,
                    VatPercentage = p.VatRate,
                }).ToList();

            count = pagedProducts?.TotalCount ?? 0;
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

    private void CalculateFodecForItems()
    {
        if (!_selectedProviderId.HasValue || getAppParametersResponse == null)
        {
            // Clear FODEC if no provider selected
            foreach (var item in orders)
            {
                // Recalculate TotalIncludingTax without FODEC
                var baseTotalIncludingTax = item.TotalExcludingTax + (item.TotalExcludingTax * (decimal)(item.VatPercentage / 100));
                item.TotalIncludingTax = baseTotalIncludingTax;
                item.PrixHtFodec = null;
            }
            return;
        }

        // Get provider to check if it's a constructor
        var provider = _filteredProviders.FirstOrDefault(p => p.Id == _selectedProviderId.Value);
        if (provider == null || !provider.Constructeur)
        {
            // Clear FODEC if provider is not a constructor
            foreach (var item in orders)
            {
                // Recalculate TotalIncludingTax without FODEC
                var baseTotalIncludingTax = item.TotalExcludingTax + (item.TotalExcludingTax * (decimal)(item.VatPercentage / 100));
                item.TotalIncludingTax = baseTotalIncludingTax;
                item.PrixHtFodec = null;
            }
            return;
        }

        // Calculate FODEC for each line item if provider is constructor
        var fodecRate = getAppParametersResponse.PourcentageFodec;
        foreach (var item in orders)
        {
            // Calculate base TotalIncludingTax (HT + VAT) without FODEC
            var baseTotalIncludingTax = DecimalHelper.RoundAmount(item.TotalExcludingTax + (item.TotalExcludingTax * (decimal)(item.VatPercentage / 100)));
            
            if (item.TotalExcludingTax > 0)
            {
                // Calculate FODEC amount
                var fodecAmount = DecimalHelper.RoundAmount(item.TotalExcludingTax * (fodecRate / 100));
                item.PrixHtFodec = fodecAmount;
                
                // Add FODEC to TotalIncludingTax
                item.TotalIncludingTax = DecimalHelper.RoundAmount(baseTotalIncludingTax + fodecAmount);
            }
            else
            {
                item.PrixHtFodec = null;
                item.TotalIncludingTax = baseTotalIncludingTax;
            }
        }
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
        // First calculate base totals (HT + VAT, without FODEC)
        LineItemCalculator.UpdateTotals(orders, out totalHt, out totalVat, out totalTtc);
        
        // Then calculate and add FODEC if applicable
        CalculateFodecForItems();
        
        // Recalculate totals after FODEC is added
        totalHt = orders.Sum(o => o.TotalExcludingTax);
        totalVat = orders.Sum(o => o.TotalIncludingTax - o.TotalExcludingTax - (o.PrixHtFodec ?? 0));
        totalFodec = orders.Sum(o => o.PrixHtFodec ?? 0);
        totalTtc = orders.Sum(o => o.TotalIncludingTax);
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

    private async Task SaveTags()
    {
        if (tagSelectorRef == null || string.IsNullOrEmpty(receiptNoteNumber) || !int.TryParse(receiptNoteNumber, out var receiptNumber))
        {
            logger.LogWarning("Cannot save tags: tagSelectorRef={TagSelectorRef}, receiptNoteNumber={ReceiptNoteNumber}", 
                tagSelectorRef == null ? "null" : "not null", receiptNoteNumber);
            return;
        }

        try
        {
            // Récupérer les tags à ajouter et à supprimer
            var tagsToAdd = tagSelectorRef.GetTagsToAdd();
            var tagIdsToRemove = tagSelectorRef.GetTagIdsToRemove();

            logger.LogInformation("Saving tags for receipt note {Number}: {AddCount} to add, {RemoveCount} to remove", 
                receiptNumber, tagsToAdd.Count, tagIdsToRemove.Count);

            // Supprimer les tags (seulement ceux qui ont un Id > 0, car les tags avec Id = 0 ne sont pas encore en base)
            var validTagIdsToRemove = tagIdsToRemove.Where(id => id > 0).ToList();
            if (validTagIdsToRemove.Any())
            {
                var removeRequest = new RemoveTagsFromDocumentRequest { TagIds = validTagIdsToRemove };
                var removeSuccess = await TagsService.RemoveTagsFromDocumentAsync("BonDeReception", receiptNumber, removeRequest);
                if (!removeSuccess)
                {
                    logger.LogWarning("Failed to remove tags from receipt note {Number}", receiptNumber);
                }
            }

            // Ajouter les nouveaux tags
            if (tagsToAdd.Any())
            {
                var addRequest = new AddTagsToDocumentByNameRequest { TagNames = tagsToAdd };
                var addSuccess = await TagsService.AddTagsToDocumentByNameAsync("BonDeReception", receiptNumber, addRequest);
                if (!addSuccess)
                {
                    logger.LogWarning("Failed to add tags to receipt note {Number}", receiptNumber);
                }
            }

            // Recharger les tags dans le TagSelector pour mettre à jour InitialTags
            if (tagSelectorRef != null)
            {
                await tagSelectorRef.LoadDocumentTags();
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save tags for receipt note {Number}", receiptNoteNumber);
            // Ne pas bloquer la sauvegarde du document si les tags échouent
            // Afficher une notification pour informer l'utilisateur
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = Localizer["warning"] ?? "Warning",
                Detail = "Les tags n'ont pas pu être sauvegardés, mais le document a été enregistré."
            });
        }
    }
}
