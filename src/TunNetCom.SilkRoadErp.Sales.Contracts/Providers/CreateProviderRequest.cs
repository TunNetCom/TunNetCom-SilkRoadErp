﻿namespace TunNetCom.SilkRoadErp.Sales.Api.Contracts.Providers;
public class CreateProviderRequest
{
    public string Nom { get; set; } = null!;

    public string Tel { get; set; } = null!;

    public string? Fax { get; set; }

    public string? Matricule { get; set; }

    public string? Code { get; set; }

    public string? CodeCat { get; set; }

    public string? EtbSec { get; set; }

    public string? Mail { get; set; }

    public string? MailDeux { get; set; }

    public bool Constructeur { get; set; }

    public string? Adresse { get; set; }
}
