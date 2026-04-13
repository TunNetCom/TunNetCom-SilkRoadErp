namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.PaiementFournisseur;

/// <summary>
/// Appointment item for RadzenScheduler displaying supplier payment due dates (échéances fournisseur).
/// </summary>
public class PaiementEcheanceAppointment
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Text { get; set; } = string.Empty;
    public int PaiementId { get; set; }
}
