namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

public class  GetDeliveryNotesWithSummariesResponse 
{
    [JsonPropertyName("totalNetAmount")]
    public decimal TotalNetAmount { get; set; }

    [JsonPropertyName("totalGrossAmount")]
    public decimal TotalGrossAmount { get; set; }

    [JsonPropertyName("totalVatAmount")]
    public decimal TotalVatAmount { get; set; }

    [JsonPropertyName("getDeliveryNoteBaseInfos")]
    public PagedList<GetDeliveryNoteBaseInfos> GetDeliveryNoteBaseInfos { get; set; } = new PagedList<GetDeliveryNoteBaseInfos>();
}