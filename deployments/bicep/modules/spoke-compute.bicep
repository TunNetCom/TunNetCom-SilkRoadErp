param location string = resourceGroup().location
param computeSpokeAddressPrefix string = '10.1.0.0/16'
param projectName string = 'silkroad'
param environment string = 'dev'
param aksNodeCount int = 2
param aksVmSize string = 'Standard_D2s_v3'
param aksKubernetesVersion string = '1.28'
param enablePrivateCluster bool = false

var computeSpokeVNetName = '${projectName}-compute-vnet-${environment}'
var aksSystemSubnetName = 'aks-system-subnet'
var aksNodesSubnetName = 'aks-nodes-subnet'
var aksSystemSubnetPrefix = '10.1.0.0/22'
var aksNodesSubnetPrefix = '10.1.4.0/22'

resource computeSpokeVNet 'Microsoft.Network/virtualNetworks@2023-11-01' = {
  name: computeSpokeVNetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [computeSpokeAddressPrefix]
    }
    subnets: [
      {
        name: aksSystemSubnetName
        properties: {
          addressPrefix: aksSystemSubnetPrefix
          delegations: [
            {
              name: 'aks-delegation'
              properties: {
                serviceName: 'Microsoft.ContainerService/managedClusters'
              }
            }
          ]
          privateEndpointNetworkPolicies: 'Enabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }
      }
      {
        name: aksNodesSubnetName
        properties: {
          addressPrefix: aksNodesSubnetPrefix
          delegations: [
            {
              name: 'aks-delegation'
              properties: {
                serviceName: 'Microsoft.ContainerService/managedClusters'
              }
            }
          ]
          privateEndpointNetworkPolicies: 'Enabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }
      }
    ]
  }
}

module aksCluster 'aks-cluster.bicep' = {
  name: 'aks-cluster-deployment'
  params: {
    location: location
    projectName: projectName
    environment: environment
    aksSubnetId: computeSpokeVNet.properties.subnets[1].id
    aksNodeCount: aksNodeCount
    aksVmSize: aksVmSize
    aksKubernetesVersion: aksKubernetesVersion
    enablePrivateCluster: enablePrivateCluster
  }
}

resource dnsZoneSql 'Microsoft.Network/privateDnsZones@2023-11-01' existing = {
  name: 'privatelink.database.windows.net'
}

resource dnsZoneBlob 'Microsoft.Network/privateDnsZones@2023-11-01' existing = {
  name: 'privatelink.blob.core.windows.net'
}

resource dnsZoneSqlLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2023-11-01' = {
  parent: dnsZoneSql
  name: '${projectName}-compute-sql-link-${environment}'
  location: 'global'
  properties: {
    virtualNetwork: {
      id: computeSpokeVNet.id
    }
    registrationEnabled: false
  }
}

resource dnsZoneBlobLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2023-11-01' = {
  parent: dnsZoneBlob
  name: '${projectName}-compute-blob-link-${environment}'
  location: 'global'
  properties: {
    virtualNetwork: {
      id: computeSpokeVNet.id
    }
    registrationEnabled: false
  }
}

output computeSpokeVNetId string = computeSpokeVNet.id
output computeSpokeVNetName string = computeSpokeVNet.name
output aksClusterId string = aksCluster.outputs.aksClusterId
output aksClusterName string = aksCluster.outputs.aksClusterName
output aksClusterFqdn string = aksCluster.outputs.aksClusterFqdn
