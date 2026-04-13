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
RUN dotnet restore src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj
RUN dotnet publish src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# Install Playwright CLI and Chromium
RUN dotnet tool install --global Microsoft.Playwright.CLI
ENV PATH="$PATH:/root/.dotnet/tools"
ENV PLAYWRIGHT_BROWSERS_PATH=/root/.playwright
WORKDIR /src/src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.WebApp
RUN playwright install chromium

# ==================================================================
# Stage 2: Runtime
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

# Copy published WebApp
COPY --from=build /app/publish ./

# Copy Playwright tools & browsers
COPY --from=build /root/.dotnet /root/.dotnet
COPY --from=build /root/.playwright /root/.playwright
ENV PATH="$PATH:/root/.dotnet/tools"
ENV PLAYWRIGHT_BROWSERS_PATH=/root/.playwright

# Install Linux dependencies required by Chromium
RUN apt-get update && apt-get install -y \
    libnss3 libatk1.0-0 libcups2 libxss1 libx11-xcb1 libxcomposite1 \
    libxdamage1 libxrandr2 libgbm1 libasound2t64 fonts-liberation libappindicator3-1 \
    libatk-bridge2.0-0 libgtk-3-0 libpangocairo-1.0-0 libxshmfence1 \
    && rm -rf /var/lib/apt/lists/*

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]
