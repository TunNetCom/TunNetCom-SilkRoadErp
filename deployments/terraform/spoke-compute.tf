resource "azurerm_virtual_network" "compute" {
  name                = "${var.project_name}-compute-vnet-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  address_space       = [var.compute_spoke_address_space]
}

resource "azurerm_subnet" "aks_system" {
  name                 = "aks-system-subnet"
  resource_group_name  = azurerm_resource_group.main.name
  virtual_network_name = azurerm_virtual_network.compute.name
  address_prefixes     = ["10.1.0.0/22"]

  delegation {
    name = "aks-delegation"

    service_delegation {
      name = "Microsoft.ContainerService/managedClusters"
      actions = [
        "Microsoft.Network/virtualNetworks/subnets/join/action"
      ]
    }
  }
}

resource "azurerm_subnet" "aks_nodes" {
  name                 = "aks-nodes-subnet"
  resource_group_name  = azurerm_resource_group.main.name
  virtual_network_name = azurerm_virtual_network.compute.name
  address_prefixes     = ["10.1.4.0/22"]

  delegation {
    name = "aks-delegation"

    service_delegation {
      name = "Microsoft.ContainerService/managedClusters"
      actions = [
        "Microsoft.Network/virtualNetworks/subnets/join/action"
      ]
    }
  }
}

resource "azurerm_kubernetes_cluster" "main" {
  name                = "${var.project_name}-aks-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  dns_prefix          = "${var.project_name}-${var.environment}"
  kubernetes_version  = var.aks_kubernetes_version

  default_node_pool {
    name                = "systempool"
    node_count          = 2
    vm_size             = var.aks_vm_size
    os_disk_size_gb     = 30
    vnet_subnet_id      = azurerm_subnet.aks_system.id
    type                = "VirtualMachineScaleSets"
    enable_auto_scaling = false
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin    = "azure"
    network_policy    = "azure"
    load_balancer_sku = "standard"
    service_cidr     = "10.10.0.0/16"
    dns_service_ip   = "10.10.0.10"
  }

  private_cluster_enabled = var.enable_private_cluster
}

resource "azurerm_kubernetes_cluster_node_pool" "user" {
  name                  = "userpool"
  kubernetes_cluster_id  = azurerm_kubernetes_cluster.main.id
  vm_size               = var.aks_vm_size
  node_count            = var.aks_node_count
  os_disk_size_gb       = 128
  vnet_subnet_id        = azurerm_subnet.aks_nodes.id
  enable_auto_scaling   = true
  min_count            = 1
  max_count            = 5
}

resource "azurerm_private_dns_zone_virtual_network_link" "compute_sql" {
  name                  = "${var.project_name}-compute-sql-link-${var.environment}"
  resource_group_name   = azurerm_resource_group.main.name
  private_dns_zone_name = azurerm_private_dns_zone.sql.name
  virtual_network_id    = azurerm_virtual_network.compute.id
  registration_enabled  = false
}

resource "azurerm_private_dns_zone_virtual_network_link" "compute_blob" {
  name                  = "${var.project_name}-compute-blob-link-${var.environment}"
  resource_group_name   = azurerm_resource_group.main.name
  private_dns_zone_name = azurerm_private_dns_zone.blob.name
  virtual_network_id    = azurerm_virtual_network.compute.id
  registration_enabled  = false
}
