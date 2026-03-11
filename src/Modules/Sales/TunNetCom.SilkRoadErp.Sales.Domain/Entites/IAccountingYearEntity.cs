namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Interface marker pour identifier les entités qui ont un AccountingYearId
/// et doivent être filtrées par l'exercice comptable actif via Global Query Filter.
/// </summary>
public interface IAccountingYearEntity
{
    int AccountingYearId { get; }
}

