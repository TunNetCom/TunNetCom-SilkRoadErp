# ==================================================================
# Stage 1: Build & Publish
# ==================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution & NuGet config
COPY *.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY nuget.config ./

# Copy source
COPY src/ ./src/

# Azure Artifacts private-feed (SilkRoadFeed) auth for dotnet restore.
# FEED_ACCESSTOKEN is injected at build time by the pipeline
# (NuGetAuthenticate task); the artifacts credential provider resolves it.
# The token never reaches the final runtime image (separate stage).
ARG FEED_ACCESSTOKEN
ARG FEED_URL=https://pkgs.dev.azure.com/TunNetCom/Silk_Road_ERP/_packaging/SilkRoadFeed/nuget/v3/index.json
ARG FEED_USERNAME=docker
RUN curl -sSL https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | sh

# Restore and publish API only
RUN export VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"${FEED_URL}\", \"username\":\"${FEED_USERNAME}\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}" \
    && dotnet restore src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj
RUN dotnet publish src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ==================================================================
# Stage 2: Runtime
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]
