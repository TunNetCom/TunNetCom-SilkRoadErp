param projectName string = 'silkroad'
param environment string = 'dev'

param hubVNetId string
param hubVNetName string
param computeSpokeVNetId string
param computeSpokeVNetName string
param dataSpokeVNetId string
param dataSpokeVNetName string

resource hubVNet 'Microsoft.Network/virtualNetworks@2023-11-01' existing = {
  name: hubVNetName
}

resource computeSpokeVNet 'Microsoft.Network/virtualNetworks@2023-11-01' existing = {
  name: computeSpokeVNetName
}

resource dataSpokeVNet 'Microsoft.Network/virtualNetworks@2023-11-01' existing = {
  name: dataSpokeVNetName
}

resource hubToComputePeering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2023-11-01' = {
  parent: hubVNet
  name: '${projectName}-hub-to-compute-${environment}'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    allowGatewayTransit: false
    remoteVirtualNetwork: {
      id: computeSpokeVNetId
    }
    useRemoteGateways: false
  }
}

resource computeToHubPeering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2023-11-01' = {
  parent: computeSpokeVNet
  name: '${projectName}-compute-to-hub-${environment}'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    allowGatewayTransit: false
    remoteVirtualNetwork: {
      id: hubVNetId
    }
    useRemoteGateways: false
  }
}

resource hubToDataPeering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2023-11-01' = {
  parent: hubVNet
  name: '${projectName}-hub-to-data-${environment}'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    allowGatewayTransit: false
    remoteVirtualNetwork: {
      id: dataSpokeVNetId
    }
    useRemoteGateways: false
  }
}

resource dataToHubPeering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2023-11-01' = {
  parent: dataSpokeVNet
  name: '${projectName}-data-to-hub-${environment}'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    allowGatewayTransit: false
    remoteVirtualNetwork: {
      id: hubVNetId
    }
    useRemoteGateways: false
  }
}
