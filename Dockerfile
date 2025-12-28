# ==================================================================
# Stage 1: Base Restore Stage (CACHE SAFE)
# ==================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

# Copy solution & NuGet config
COPY *.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy csproj ONLY (important!)
COPY src/TunNetCom.SilkRoadErp.Sales.Api/*.csproj src/TunNetCom.SilkRoadErp.Sales.Api/
COPY src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/*.csproj src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/

# Restore projects
RUN dotnet restore src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj
RUN dotnet restore src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj


# ==================================================================
# Stage 2: Build & Publish API
# ==================================================================
FROM restore AS publish-api
WORKDIR /src

# Copy full source AFTER restore
COPY src/ ./src/

RUN dotnet publish \
    src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    -c Release \
    -o /out/api \
    /p:UseAppHost=false


# ==================================================================
# Stage 3: Build & Publish WebApp
# ==================================================================
FROM restore AS publish-webapp
WORKDIR /src

# Copy full source AFTER restore
COPY src/ ./src/

RUN dotnet publish \
    src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj \
    -c Release \
    -o /out/webapp \
    /p:UseAppHost=false


# ==================================================================
# Stage 4: Playwright (WebApp only)
# ==================================================================
FROM publish-webapp AS webapp-playwright

ENV PLAYWRIGHT_BROWSERS_PATH=/root/.playwright
RUN dotnet tool install --global Microsoft.Playwright.CLI \
 && playwright install chromium


# ==================================================================
# Stage 5: API Runtime
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api
WORKDIR /app

COPY --from=publish-api /out/api .

EXPOSE 5000
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]


# ==================================================================
# Stage 6: WebApp Runtime
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS webapp
WORKDIR /app

COPY --from=webapp-playwright /out/webapp .
COPY --from=webapp-playwright /root/.playwright /root/.playwright

ENV PLAYWRIGHT_BROWSERS_PATH=/root/.playwright
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]
