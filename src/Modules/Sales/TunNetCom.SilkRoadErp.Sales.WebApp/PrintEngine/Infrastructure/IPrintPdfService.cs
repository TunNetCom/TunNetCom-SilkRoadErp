namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public interface IPrintPdfService<TModel, TView>
{
    Task<byte[]> GeneratePdfAsync(TModel printModel, CancellationToken cancellationToken);
    Task<byte[]> GeneratePdfAsync(TModel printModel, SilkPdfOptions pdfOptions, CancellationToken cancellationToken);
}