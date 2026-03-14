resource "azurerm_virtual_network" "data" {
  name                = "${var.project_name}-data-vnet-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  address_space       = [var.data_spoke_address_space]
}

resource "azurerm_subnet" "data_private_endpoints" {
  name                 = "private-endpoints-subnet"
  resource_group_name  = azurerm_resource_group.main.name
  virtual_network_name = azurerm_virtual_network.data.name
  address_prefixes     = ["10.2.2.0/26"]

  private_endpoint_network_policies_enabled = false
}

resource "azurerm_mssql_server" "main" {
  name                         = "${replace(var.project_name, "-", "")}-sql-${var.environment}-${substr(md5(azurerm_resource_group.main.id), 0, 8)}"
  resource_group_name          = azurerm_resource_group.main.name
  location                     = azurerm_resource_group.main.location
  version                      = "12.0"
  administrator_login          = var.sql_administrator_login
  administrator_login_password = var.sql_administrator_password
  minimum_tls_version          = "1.2"

  public_network_access_enabled = false
}

resource "azurerm_mssql_database" "main" {
  name           = "silkroaderp"
  server_id       = azurerm_mssql_server.main.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  max_size_gb    = 2
  sku_name       = "Basic"
  zone_redundant = false
}

resource "azurerm_private_endpoint" "sql" {
  name                = "${var.project_name}-sql-pe-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  subnet_id           = azurerm_subnet.data_private_endpoints.id

  private_service_connection {
    name                           = "sql-pls-connection"
    private_connection_resource_id = azurerm_mssql_server.main.id
    subresource_names              = ["sqlServer"]
    is_manual_connection           = false
  }

  private_dns_zone_group {
    name                 = "sql-dns-group"
    private_dns_zone_ids = [azurerm_private_dns_zone.sql.id]
  }
}

resource "azurerm_storage_account" "main" {
  name                     = "st${replace(var.project_name, "-", "")}${var.environment}${substr(md5(azurerm_resource_group.main.id), 0, 10)}"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  min_tls_version          = "TLS1_2"

  allow_nested_items_to_be_public = false

  network_rules {
    default_action             = "Deny"
    bypass                     = ["AzureServices"]
    ip_rules                   = []
    virtual_network_subnet_ids = []
  }
}

resource "azurerm_private_endpoint" "blob" {
  name                = "${var.project_name}-blob-pe-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  subnet_id           = azurerm_subnet.data_private_endpoints.id

  private_service_connection {
    name                           = "blob-pls-connection"
    private_connection_resource_id = azurerm_storage_account.main.id
    subresource_names              = ["blob"]
    is_manual_connection           = false
  }

  private_dns_zone_group {
    name                 = "blob-dns-group"
    private_dns_zone_ids = [azurerm_private_dns_zone.blob.id]
  }
}
