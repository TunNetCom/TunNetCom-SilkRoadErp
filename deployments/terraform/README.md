# AKS Hub-Spoke Infrastructure (Terraform)

This folder contains Terraform configuration to provision an AKS cluster in a Hub and Spoke network topology:
- **Hub VNet**: Azure Firewall, Private DNS Zones for SQL and Blob
- **Compute spoke**: AKS cluster (system + user node pools)
- **Data spoke**: Azure SQL Database, Storage Account, each with Private Endpoints

## Prerequisites

- Terraform >= 1.5
- Azure CLI (`az login`)
- Contributor (or higher) on target subscription

## Deployment

1. Copy the example variables and customize:
   ```bash
   cp terraform.tfvars.example terraform.tfvars
   # Edit terraform.tfvars - set resource_group_name, sql_administrator_login, sql_administrator_password
   ```

2. Initialize Terraform:
   ```bash
   terraform init
   ```

3. Create the resource group (if it does not exist):
   ```bash
   az group create --name rg-silkroad-hubspoke-dev --location eastus
   ```

4. Plan and apply:
   ```bash
   terraform plan -out=tfplan
   terraform apply tfplan
   ```

5. Get AKS credentials:
   ```bash
   az aks get-credentials --resource-group $(terraform output -raw resource_group_name) --name $(terraform output -raw aks_cluster_name)
   ```
   Or if using a fixed RG name:
   ```bash
   az aks get-credentials --resource-group rg-silkroad-hubspoke-dev --name silkroad-aks-dev
   ```

## Variables

| Variable | Description | Default |
|----------|-------------|---------|
| location | Azure region | eastus |
| environment | dev or prod | dev |
| resource_group_name | Resource group name | (required) |
| hub_address_space | Hub VNet CIDR | 10.0.0.0/16 |
| compute_spoke_address_space | Compute spoke CIDR | 10.1.0.0/16 |
| data_spoke_address_space | Data spoke CIDR | 10.2.0.0/16 |
| aks_node_count | User pool node count | 2 |
| aks_vm_size | Node VM size | Standard_D2s_v3 |
| aks_kubernetes_version | Kubernetes version | 1.28 |
| sql_administrator_login | SQL admin login | (required) |
| sql_administrator_password | SQL admin password | (required, sensitive) |
| enable_azure_firewall | Deploy Azure Firewall | true |
| enable_private_cluster | Private AKS API | false |
| project_name | Resource naming prefix | silkroad |

## File Structure

```
terraform/
├── main.tf              # Resource group
├── variables.tf         # Input variables
├── outputs.tf           # Output values
├── versions.tf         # Provider configuration
├── hub.tf               # Hub VNet, Firewall, Private DNS Zones
├── spoke-compute.tf     # Compute spoke, AKS
├── spoke-data.tf        # Data spoke, SQL, Storage, Private Endpoints
├── peering.tf           # VNet peering
├── terraform.tfvars.example
└── README.md
```

## Outputs

- `hub_vnet_id`: Hub VNet resource ID
- `aks_cluster_id`: AKS cluster resource ID
- `aks_cluster_name`: AKS cluster name
- `aks_cluster_fqdn`: AKS API FQDN
- `sql_server_fqdn`: SQL Server FQDN for connection string
- `storage_account_name`: Storage account name
