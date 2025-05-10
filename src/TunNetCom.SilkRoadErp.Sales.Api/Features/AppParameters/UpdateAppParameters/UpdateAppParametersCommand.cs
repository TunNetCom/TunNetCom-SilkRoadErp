﻿using System.Security.Cryptography;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.UpdateAppParameters;
public record UpdateAppParametersCommand(
    string NomSociete,
    decimal? Timbre,
    string? Adresse,
    string? Tel,
    string? Fax,
    string? Email,
    string? MatriculeFiscale,
    string? CodeTva,
    string? CodeCategorie,
    string? EtbSecondaire,
    decimal? PourcentageFodec,
    string? AdresseRetenu,
    double? PourcentageRetenu) : IRequest<Result>;