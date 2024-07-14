namespace TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

public class UpdateCustomerRequest
{
    public string Nom { get; set; } = null!;

    public string? Tel { get; set; }

    public string? Adresse { get; set; }

    public string? Matricule { get; set; }

    public string? Code { get; set; }

    public string? CodeCat { get; set; }

    public string? EtbSec { get; set; }

    public string? Mail { get; set; }
}
