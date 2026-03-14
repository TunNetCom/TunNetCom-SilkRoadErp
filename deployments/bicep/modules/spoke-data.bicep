param location string = resourceGroup().location
param dataSpokeAddressPrefix string = '10.2.0.0/16'
param projectName string = 'silkroad'
param environment string = 'dev'
param sqlAdministratorLogin string
@secure()
param sqlAdministratorPassword string
param dnsZoneSqlId string
param dnsZoneBlobId string

var dataSpokeVNetName = '${projectName}-data-vnet-${environment}'
var peSubnetName = 'private-endpoints-subnet'
var peSubnetPrefix = '10.2.2.0/26'
var sqlServerName = '${replace(projectName, '-', '')}-sql-${environment}-${take(uniqueString(resourceGroup().id), 8)}'
var storageAccountName = 'st${replace(projectName, '-', '')}${environment}${take(uniqueString(resourceGroup().id), 10)}'
var sqlDbName = 'silkroaderp'

resource dataSpokeVNet 'Microsoft.Network/virtualNetworks@2023-11-01' = {
  name: dataSpokeVNetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [dataSpokeAddressPrefix]
    }
    subnets: [
      {
        name: peSubnetName
        properties: {
          addressPrefix: peSubnetPrefix
          privateEndpointNetworkPolicies: 'Disabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }
      }
    ]
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-11-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Disabled'
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2023-11-01-preview' = {
  parent: sqlServer
  name: sqlDbName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648
  }
}

resource sqlPrivateEndpoint 'Microsoft.Network/privateEndpoints@2023-11-01' = {
  name: '${projectName}-sql-pe-${environment}'
  location: location
  properties: {
    subnet: {
      id: dataSpokeVNet.properties.subnets[0].id
    }
    privateLinkServiceConnections: [
      {
        name: 'sql-pls-connection'
        properties: {
          privateLinkServiceId: sqlServer.id
          groupIds: ['sqlServer']
        }
      }
    ]
    privateDnsZoneGroup: {
      name: 'sql-dns-group'
      privateDnsZoneConfigs: [
        {
          name: 'sql-dns'
          properties: {
            privateDnsZoneId: dnsZoneSqlId
          }
        }
      ]
    }
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-11-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
  }
}

resource blobPrivateEndpoint 'Microsoft.Network/privateEndpoints@2023-11-01' = {
  name: '${projectName}-blob-pe-${environment}'
  location: location
  properties: {
    subnet: {
      id: dataSpokeVNet.properties.subnets[0].id
    }
    privateLinkServiceConnections: [
      {
        name: 'blob-pls-connection'
        properties: {
          privateLinkServiceId: storageAccount.id
          groupIds: ['blob']
        }
      }
    ]
    privateDnsZoneGroup: {
      name: 'blob-dns-group'
      privateDnsZoneConfigs: [
        {
          name: 'blob-dns'
          properties: {
            privateDnsZoneId: dnsZoneBlobId
          }
        }
      ]
    }
  }
}

output dataSpokeVNetId string = dataSpokeVNet.id
output dataSpokeVNetName string = dataSpokeVNet.name
output sqlServerFqdn string = '${sqlServer.name}.database.windows.net'
output sqlConnectionString string = 'Server=tcp:${sqlServer.name}.database.windows.net,1433;Database=${sqlDbName};User ID=${sqlAdministratorLogin};Password=***;Encrypt=True;'
output storageAccountName string = storageAccount.name
output storageBlobEndpoint string = storageAccount.properties.primaryEndpoints.blob
