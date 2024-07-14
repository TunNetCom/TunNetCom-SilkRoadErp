namespace TunNetCom.SilkRoadErp.Sales.Contracts.ErrorResponses;

public class ValidationError
{
    public string? PropertyName { get; set; }
    public string? ErrorMessage { get; set; }
}
