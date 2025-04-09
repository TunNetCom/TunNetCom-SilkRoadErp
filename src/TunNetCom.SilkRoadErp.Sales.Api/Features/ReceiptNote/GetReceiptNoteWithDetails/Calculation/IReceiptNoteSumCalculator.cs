namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteWithDetails.Calculation;

public interface IReceiptNoteSumCalculator
{
    public Task<ReceiptNoteDetailsResponse> CalculateTotalHTvaAsync(
        ReceiptNoteResponse receiptNote,
        CancellationToken cancellationToken);
}