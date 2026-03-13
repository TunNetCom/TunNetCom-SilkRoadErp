# ==================================================================
# Stage 1: Build & Publish
# ==================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution & NuGet config
COPY *.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy source
COPY src/ ./src/

# Restore and publish WebApp only
RUN dotnet restore src/Administration/TunNetCom.SilkRoadErp.Administration.WebApp/TunNetCom.SilkRoadErp.Administration.WebApp.csproj
RUN dotnet publish src/Administration/TunNetCom.SilkRoadErp.Administration.WebApp/TunNetCom.SilkRoadErp.Administration.WebApp.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ==================================================================
# Stage 2: Runtime
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish ./

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Administration.WebApp.dll"]
