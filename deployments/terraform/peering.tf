resource "azurerm_virtual_network_peering" "hub_to_compute" {
  name                         = "${var.project_name}-hub-to-compute-${var.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  virtual_network_name        = azurerm_virtual_network.hub.name
  remote_virtual_network_id    = azurerm_virtual_network.compute.id
  allow_virtual_network_access = true
  allow_forwarded_traffic      = false
  allow_gateway_transit        = false
}

resource "azurerm_virtual_network_peering" "compute_to_hub" {
  name                         = "${var.project_name}-compute-to-hub-${var.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  virtual_network_name        = azurerm_virtual_network.compute.name
  remote_virtual_network_id    = azurerm_virtual_network.hub.id
  allow_virtual_network_access = true
  allow_forwarded_traffic      = false
  allow_gateway_transit        = false
}

resource "azurerm_virtual_network_peering" "hub_to_data" {
  name                         = "${var.project_name}-hub-to-data-${var.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  virtual_network_name        = azurerm_virtual_network.hub.name
  remote_virtual_network_id    = azurerm_virtual_network.data.id
  allow_virtual_network_access = true
  allow_forwarded_traffic      = false
  allow_gateway_transit        = false
}

resource "azurerm_virtual_network_peering" "data_to_hub" {
  name                         = "${var.project_name}-data-to-hub-${var.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  virtual_network_name        = azurerm_virtual_network.data.name
  remote_virtual_network_id    = azurerm_virtual_network.hub.id
  allow_virtual_network_access = true
  allow_forwarded_traffic      = false
  allow_gateway_transit        = false
}
