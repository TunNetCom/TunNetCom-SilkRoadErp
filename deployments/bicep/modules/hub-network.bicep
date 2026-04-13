param location string = resourceGroup().location
param hubAddressPrefix string = '10.0.0.0/16'
param enableAzureFirewall bool = true
param projectName string = 'silkroad'
param environment string = 'dev'

var hubVNetName = '${projectName}-hub-vnet-${environment}'
var firewallSubnetName = 'AzureFirewallSubnet'
var gatewaySubnetName = 'GatewaySubnet'
var firewallSubnetPrefix = '10.0.0.0/26'
var gatewaySubnetPrefix = '10.0.0.64/27'

resource hubVNet 'Microsoft.Network/virtualNetworks@2023-11-01' = {
  name: hubVNetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [hubAddressPrefix]
    }
    subnets: [
      {
        name: firewallSubnetName
        properties: {
          addressPrefix: firewallSubnetPrefix
          delegations: []
          privateEndpointNetworkPolicies: 'Enabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }
      }
      {
        name: gatewaySubnetName
        properties: {
          addressPrefix: gatewaySubnetPrefix
          delegations: []
          privateEndpointNetworkPolicies: 'Enabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }
      }
    ]
  }
}

resource azureFirewall 'Microsoft.Network/azureFirewalls@2023-11-01' = if (enableAzureFirewall) {
  name: '${projectName}-hub-fw-${environment}'
  location: location
  properties: {
    sku: {
      name: 'AZFW_Hub'
      tier: 'Standard'
    }
    threatIntelMode: 'Alert'
    ipConfigurations: [
      {
        name: 'configuration'
        properties: {
          subnet: {
            id: hubVNet.properties.subnets[0].id
          }
        }
      }
    ]
  }
}

resource dnsZoneSql 'Microsoft.Network/privateDnsZones@2023-11-01' = {
  name: 'privatelink.database.windows.net'
  location: 'global'
  properties: {}
}

resource dnsZoneBlob 'Microsoft.Network/privateDnsZones@2023-11-01' = {
  name: 'privatelink.blob.core.windows.net'
  location: 'global'
  properties: {}
}

output hubVNetId string = hubVNet.id
output hubVNetName string = hubVNet.name
output hubResourceGroup string = resourceGroup().name
output firewallPrivateIp string = enableAzureFirewall ? azureFirewall.properties.ipConfigurations[0].properties.privateIPAddress : ''
output dnsZoneSqlId string = dnsZoneSql.id
output dnsZoneBlobId string = dnsZoneBlob.id
