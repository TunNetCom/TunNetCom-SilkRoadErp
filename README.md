# SilkRoadErp

SilkRoadErp is a multi-tenant ERP solution built with .NET Aspire. It provides modules for sales operations, supplier management, administration, and tenant provisioning. The application uses a distributed microservices architecture with Blazor Server web apps consuming backend APIs.

## Architecture

The solution is orchestrated by **Aspire AppHost** and runs the following services:

```mermaid
flowchart TB
    subgraph AppHost["Aspire AppHost (Orchestrator)"]
        HOST[AppHost]
    end

    subgraph Services["Services"]
        SA[sales-api]
        AA[admin-api]
        SW[sales-web]
        AW[admin-webapp]
        TW[tenant-webapp]
    end

    subgraph Data["Data Layer"]
        SALESDB[(salesdb SQL)]
        ADMINDB[(admindb SQL)]
        REDIS[(Redis)]
    end

    subgraph Observability["Observability"]
        LOKI[Grafana Loki]
        GRAFANA[Grafana]
    end

    HOST --> SA & AA & SW & AW & TW
    SA --> SALESDB
    SA --> REDIS
    AA --> ADMINDB
    SW & AW & TW --> SA
    SW & AW & TW --> AA
    SA & AA --> LOKI
    LOKI --> GRAFANA
```

### Services

| Service | Description |
|--------|-------------|
| **Sales API** | REST API for sales, clients, suppliers, invoices, delivery notes, products, orders, quotations |
| **Administration API** | Tenant and administration management |
| **Sales WebApp** | Blazor Server UI for sales operations |
| **Administration WebApp** | Blazor Server UI for administration |
| **TenantSetup WebApp** | Blazor Server UI for initial tenant configuration |

### Infrastructure

- **SQL Server** – Two databases: `salesdb` (sales module), `admindb` (administration)
- **Redis** – Distributed caching for the Sales API
- **Grafana Loki** – Log aggregation
- **Grafana** – Dashboards and monitoring (port 3000)

## Project Structure

```mermaid
flowchart TD
    subgraph src[src]
        subgraph Aspire
            AH[AppHost - Orchestrator]
            SD[ServiceDefaults]
        end
        subgraph Modules
            subgraph Sales
                SAPI[Sales.Api]
                SDOM[Sales.Domain]
                SCT[Sales.Contracts]
                SHC[Sales.HttpClients]
                SWA[Sales.WebApp]
            end
        end
        subgraph Administration
            AAPI[Administration.Api]
            ADOM[Administration.Domain]
            ACT[Administration.Contracts]
            AHC[Administration.HttpClients]
            AWA[Administration.WebApp]
            TWA[TenantSetup.WebApp]
        end
        subgraph SharedKernel
            SK[SharedKernel]
            subgraph Infra[Infrastructure]
                CACHE[Caching]
                MT[MultiTenancy]
            end
        end
    end
```

| Path | Description |
|------|-------------|
| `Aspire/AppHost` | Orchestrator — run this to start all services |
| `Aspire/ServiceDefaults` | Shared service configuration |
| `Modules/Sales/*` | Sales REST API, domain, contracts, HTTP clients, Blazor UI |
| `Administration/*` | Administration and tenant APIs and web apps |
| `SharedKernel/` | Shared kernel, caching, and multi-tenancy infrastructure |

### Sales Module

The Sales module handles both customer-facing sales and supplier-side operations:

- **Sales**: Clients, invoices (Factures), credit notes (Avoirs), delivery notes (Bons de livraison), orders, quotations, payments
- **Purchasing**: Suppliers (Fournisseurs), supplier invoices (Factures fournisseurs), supplier credit notes, expense invoices (Factures dépenses), reception notes, returns
- **Catalog**: Products (Produits), categories, subcategories, tags, inventory

It uses **multi-tenancy** (per-tenant data isolation) and **accounting year** scoping for financial documents.

## Technology Stack

| Layer | Technology |
|-------|------------|
| Backend | .NET 8+, Carter (minimal APIs), MediatR (CQRS) |
| Frontend | Blazor Server, Blazor.Bootstrap, Radzen |
| Database | SQL Server, Entity Framework Core 10 |
| Caching | Redis |
| Auth | JWT Bearer |
| Logging | Serilog, Grafana Loki |
| Orchestration | Aspire AppHost |

## Getting Started

```mermaid
flowchart LR
    A[Prerequisites] --> B[Run AppHost]
    B --> C[Aspire Dashboard]
    C --> D[All Services]
    D --> E[salesdb + admindb]
    D --> F[Redis + Loki + Grafana]
```

### Prerequisites

- .NET 8 SDK
- Docker (for SQL Server, Redis, Loki, Grafana when running via Aspire)

### Run the application

From the repository root:

```sh
cd src/Aspire/TunNetCom.SilkRoadErp.AppHost
dotnet run
```

This starts the Aspire dashboard and all services. The dashboard shows endpoints for each application.

### Run without Aspire

To run individual projects (e.g. Sales API or Sales WebApp), configure connection strings in `appsettings.json` or user secrets, then:

```sh
# Sales API
cd src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.Api
dotnet run

# Sales WebApp
cd src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.WebApp
dotnet run
```

### Database migrations

From the Sales Domain project:

```sh
cd src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.Api
dotnet ef database update --project ../TunNetCom.SilkRoadErp.Sales.Domain
```

## Contributing

1. Fork the repository.
2. Create a feature branch.
3. Commit your changes and open a pull request.

## License

This project is licensed under the MIT License.

## Contact

