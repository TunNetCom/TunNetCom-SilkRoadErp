output "resource_group_name" {
  description = "Resource group name"
  value       = azurerm_resource_group.main.name
}

output "hub_vnet_id" {
  description = "Hub VNet resource ID"
  value       = azurerm_virtual_network.hub.id
}

output "aks_cluster_id" {
  description = "AKS cluster resource ID"
  value       = azurerm_kubernetes_cluster.main.id
}

output "aks_cluster_name" {
  description = "AKS cluster name (for az aks get-credentials)"
  value       = azurerm_kubernetes_cluster.main.name
}

output "aks_cluster_fqdn" {
  description = "AKS API server FQDN"
  value       = azurerm_kubernetes_cluster.main.fqdn
}

output "sql_server_fqdn" {
  description = "SQL Server FQDN for connection string"
  value       = "${azurerm_mssql_server.main.fully_qualified_domain_name}"
}

output "storage_account_name" {
  description = "Storage account name"
  value       = azurerm_storage_account.main.name
}
