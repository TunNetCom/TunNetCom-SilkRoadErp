param location string = resourceGroup().location
param projectName string = 'silkroad'
param environment string = 'dev'
param aksSubnetId string
param aksNodeCount int = 2
param aksVmSize string = 'Standard_D2s_v3'
param aksKubernetesVersion string = '1.28'
param enablePrivateCluster bool = false

var aksClusterName = '${projectName}-aks-${environment}'

resource aksCluster 'Microsoft.ContainerService/managedClusters@2023-11-01' = {
  name: aksClusterName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    dnsPrefix: '${projectName}-${environment}'
    agentPoolProfiles: [
      {
        name: 'systempool'
        count: 2
        vmSize: aksVmSize
        osType: 'Linux'
        osDiskSizeGB: 30
        type: 'VirtualMachineScaleSets'
        mode: 'System'
        orchestratorVersion: aksKubernetesVersion
        enableNodePublicIP: false
        vnetSubnetID: aksSubnetId
      }
      {
        name: 'userpool'
        count: aksNodeCount
        vmSize: aksVmSize
        osType: 'Linux'
        osDiskSizeGB: 128
        type: 'VirtualMachineScaleSets'
        mode: 'User'
        orchestratorVersion: aksKubernetesVersion
        enableNodePublicIP: false
        vnetSubnetID: aksSubnetId
        enableAutoScaling: true
        minCount: 1
        maxCount: 5
      }
    ]
    networkProfile: {
      networkPlugin: 'azure'
      networkPolicy: 'azure'
      loadBalancerSku: 'standard'
      serviceCidr: '10.10.0.0/16'
      dnsServiceIP: '10.10.0.10'
    }
    kubernetesVersion: aksKubernetesVersion
    enableRBAC: true
    apiServerAccessProfile: {
      enablePrivateCluster: enablePrivateCluster
    }
  }
}

output aksClusterId string = aksCluster.id
output aksClusterName string = aksCluster.name
output aksClusterFqdn string = aksCluster.properties.fqdn
output aksPrincipalId string = aksCluster.identity.principalId
