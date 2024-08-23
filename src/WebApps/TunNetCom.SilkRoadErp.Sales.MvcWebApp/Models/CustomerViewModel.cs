namespace TunNetCom.SilkRoadErp.Sales.MvcWebApp.Models;

using System.ComponentModel.DataAnnotations;

public class CustomerViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nom { get; set; }

    [MaxLength(50)]
    public string Tel { get; set; }

    [MaxLength(50)]
    public string Adresse { get; set; }

    public string Matricule { get; set; }
    public string Code { get; set; }
    public string CodeCat { get; set; }
    public string EtbSec { get; set; }
    public string Mail { get; set; }
}
