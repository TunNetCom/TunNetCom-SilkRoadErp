# AKS Hub-Spoke Infrastructure (Bicep)

This folder contains Bicep templates to provision an AKS cluster in a Hub and Spoke network topology:
- **Hub VNet**: Azure Firewall, Private DNS Zones for SQL and Blob
- **Compute spoke**: AKS cluster (system + user node pools)
- **Data spoke**: Azure SQL Database, Storage Account, each with Private Endpoints

## Prerequisites

- Azure CLI with Bicep (`az bicep version`)
- Contributor (or higher) on target subscription/resource group
- Sufficient quota for AKS, Firewall, SQL in the region

## Deployment

1. Create a resource group:
   ```bash
   az group create --name rg-silkroad-hubspoke-dev --location eastus
   ```

2. Deploy using the parameters file (provide `sqlAdministratorPassword` at deploy time):
   ```bash
   az deployment group create \
     --resource-group rg-silkroad-hubspoke-dev \
     --template-file main.bicep \
     --parameters main.parameters.json \
     --parameters sqlAdministratorPassword='YourSecurePassword123!'
   ```

   Or deploy without a parameters file, passing all required params:
   ```bash
   az deployment group create \
     --resource-group rg-silkroad-hubspoke-dev \
     --template-file main.bicep \
     --parameters sqlAdministratorLogin=sqladmin \
     --parameters sqlAdministratorPassword='YourSecurePassword123!'
   ```

3. Get AKS credentials after deployment:
   ```bash
   az aks get-credentials --resource-group rg-silkroad-hubspoke-dev --name silkroad-aks-dev
   ```

## Parameters

| Parameter | Description | Default |
|-----------|-------------|---------|
| location | Azure region | resourceGroup().location |
| environment | dev or prod | dev |
| hubAddressPrefix | Hub VNet CIDR | 10.0.0.0/16 |
| computeSpokeAddressPrefix | Compute spoke CIDR | 10.1.0.0/16 |
| dataSpokeAddressPrefix | Data spoke CIDR | 10.2.0.0/16 |
| aksNodeCount | User pool node count | 2 |
| aksVmSize | Node VM size | Standard_D2s_v3 |
| aksKubernetesVersion | Kubernetes version | 1.28 |
| sqlAdministratorLogin | SQL admin login | (required) |
| sqlAdministratorPassword | SQL admin password | (required, secure) |
| enableAzureFirewall | Deploy Azure Firewall | true |
| enablePrivateCluster | Private AKS API | false |
| projectName | Resource naming prefix | silkroad |

## File Structure

```
bicep/
├── main.bicep              # Entry point
├── parameters.bicep        # (Inlined in main.bicep)
├── main.parameters.json    # Default parameter values
├── modules/
│   ├── hub-network.bicep   # Hub VNet, Firewall, Private DNS Zones
│   ├── spoke-compute.bicep # Compute spoke VNet, AKS, DNS links
│   ├── spoke-data.bicep    # Data spoke VNet, SQL, Storage, PEs
│   ├── peering.bicep       # VNet peering (Hub ↔ Compute, Hub ↔ Data)
│   └── aks-cluster.bicep   # AKS cluster resource
└── README.md
```

## Outputs

- `hubVNetId`: Hub VNet resource ID
- `aksClusterId`: AKS cluster resource ID
- `aksClusterName`: AKS cluster name (for `az aks get-credentials`)
- `aksClusterFqdn`: AKS API FQDN
- `sqlServerFqdn`: SQL Server FQDN (for connection string)
- `storageAccountName`: Storage account name
