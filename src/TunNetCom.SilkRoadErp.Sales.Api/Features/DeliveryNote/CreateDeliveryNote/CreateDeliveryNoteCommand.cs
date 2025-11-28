namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public record CreateDeliveryNoteCommand(
    DateTime Date,
    decimal TotHTva,
    decimal TotTva,
    decimal NetPayer,
    TimeOnly TempBl,
    int? NumFacture,
    int? ClientId,
    int? InstallationTechnicianId,
    IEnumerable<LigneBlSubCommand> DeliveryNoteDetails)
    : IRequest<Result<int>>;

public record LigneBlSubCommand
{
    public string RefProduit { get; set; } = string.Empty;

    public string DesignationLi { get; set; } = string.Empty;

    public int QteLi { get; set; }

    public int? QteLivree { get; set; }

    public decimal PrixHt { get; set; }

    public double Remise { get; set; }

    public decimal TotHt { get; set; }

    public double Tva { get; set; }

    public decimal TotTtc { get; set; }
}

