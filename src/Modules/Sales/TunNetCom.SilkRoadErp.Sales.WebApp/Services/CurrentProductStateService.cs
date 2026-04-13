namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

/// <summary>
/// Represents the current state of a product being edited in a line item.
/// Used for reactive updates between LineItemsGrid and ProductHistoryDialog.
/// </summary>
public record CurrentProductState(
    string ProductReference,
    string ProductName,
    decimal UnitPriceHt,
    decimal TotalHt,
    decimal TotalTtc,
    double DiscountPercentage,
    double VatPercentage,
    bool IsConstructorProvider,
    decimal FodecPercentage,
    int Quantity = 1
);

/// <summary>
/// Service interface for managing the current product state.
/// Enables reactive communication between parent pages and dialogs.
/// </summary>
public interface ICurrentProductStateService
{
    /// <summary>
    /// Gets the current product state.
    /// </summary>
    CurrentProductState? State { get; }

    /// <summary>
    /// Event triggered when the state changes.
    /// </summary>
    event Action? OnStateChanged;

    /// <summary>
    /// Updates the current product state and notifies subscribers.
    /// </summary>
    /// <param name="state">The new state.</param>
    void UpdateState(CurrentProductState state);

    /// <summary>
    /// Clears the current state.
    /// </summary>
    void Clear();
}

/// <summary>
/// Scoped service implementation for managing current product state.
/// Registered per-circuit for Blazor Server isolation.
/// </summary>
public class CurrentProductStateService : ICurrentProductStateService
{
    private CurrentProductState? _state;

    public CurrentProductState? State => _state;

    public event Action? OnStateChanged;

    public void UpdateState(CurrentProductState state)
    {
        _state = state;
        OnStateChanged?.Invoke();
    }

    public void Clear()
    {
        _state = null;
        OnStateChanged?.Invoke();
    }
}

public interface ICurrentProductCalculationService
{
    decimal CalculateFodecValue(decimal totalHt, decimal fodecPercentage);
    decimal CalculateFodecValue(CurrentProductState state);
}

public class CurrentProductCalculationService : ICurrentProductCalculationService
{
    public decimal CalculateFodecValue(decimal totalHt, decimal fodecPercentage) =>
        totalHt * (fodecPercentage / 100m);

    public decimal CalculateFodecValue(CurrentProductState state) =>
        CalculateFodecValue(state.TotalHt, state.FodecPercentage);
}
