namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
public class AttachToInvoiceResult
{
    public bool IsSuccess { get; set; }
    public AttachErrorType ErrorType { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}
// Result model to handle different response scenarios


public enum AttachErrorType
{
    None,
    NotFound,
    Validation
}