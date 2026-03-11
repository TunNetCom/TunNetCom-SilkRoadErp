namespace TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

public class UpdateCustomerRequest
{
    public UpdateCustomerRequest()
    {
        
    }

    public UpdateCustomerRequest(string nom,
                                 string? tel,
                                 string? adresse,
                                 string? matricule,
                                 string? code,
                                 string? codeCat,
                                 string? etbSec,
                                 string? mail)
    {
        Nom = nom;
        Tel = tel;
        Adresse = adresse;
        Matricule = matricule;
        Code = code;
        CodeCat = codeCat;
        EtbSec = etbSec;
        Mail = mail;
    }

    public string Nom { get; set; } = null!;

    public string? Tel { get; set; }

    public string? Adresse { get; set; }

    public string? Matricule { get; set; }

    public string? Code { get; set; }

    public string? CodeCat { get; set; }

    public string? EtbSec { get; set; }

    public string? Mail { get; set; }
}
