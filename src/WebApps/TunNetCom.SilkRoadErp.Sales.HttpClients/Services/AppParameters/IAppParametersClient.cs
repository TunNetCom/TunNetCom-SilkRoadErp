﻿using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;

public interface IAppParametersClient
{
    Task<OneOf<GetAppParametersResponse, bool>> GetAppParametersAsync(CancellationToken cancellationToken);
    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAppParametersAsync(
        UpdateAppParametersRequest request,
        CancellationToken cancellationToken);
}
