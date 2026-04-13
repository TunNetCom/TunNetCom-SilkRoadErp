targetScope = 'resourceGroup'

@description('Azure region for all resources')
param location string = resourceGroup().location

@description('Environment (dev | prod)')
@allowed(['dev', 'prod'])
param environment string = 'dev'

@description('Hub VNet address prefix')
param hubAddressPrefix string = '10.0.0.0/16'

@description('Compute spoke VNet address prefix')
param computeSpokeAddressPrefix string = '10.1.0.0/16'

@description('Data spoke VNet address prefix')
param dataSpokeAddressPrefix string = '10.2.0.0/16'

@description('Number of AKS agent nodes in user pool')
param aksNodeCount int = 2

@description('AKS agent node VM size')
param aksVmSize string = 'Standard_D2s_v3'

@description('AKS Kubernetes version')
param aksKubernetesVersion string = '1.28'

@description('SQL administrator login')
param sqlAdministratorLogin string

@description('SQL administrator password')
@secure()
param sqlAdministratorPassword string

@description('Enable Azure Firewall in Hub')
param enableAzureFirewall bool = true

@description('Enable private AKS cluster')
param enablePrivateCluster bool = false

@description('Project/app name for resource naming')
param projectName string = 'silkroad'

module hub 'modules/hub-network.bicep' = {
  name: 'hub-network-deployment'
  params: {
    location: location
    hubAddressPrefix: hubAddressPrefix
    enableAzureFirewall: enableAzureFirewall
    projectName: projectName
    environment: environment
  }
}

module spokeData 'modules/spoke-data.bicep' = {
  name: 'spoke-data-deployment'
  params: {
    location: location
    dataSpokeAddressPrefix: dataSpokeAddressPrefix
    projectName: projectName
    environment: environment
    sqlAdministratorLogin: sqlAdministratorLogin
    sqlAdministratorPassword: sqlAdministratorPassword
    dnsZoneSqlId: hub.outputs.dnsZoneSqlId
    dnsZoneBlobId: hub.outputs.dnsZoneBlobId
  }
}

module spokeCompute 'modules/spoke-compute.bicep' = {
  name: 'spoke-compute-deployment'
  dependsOn: [hub]
  params: {
    location: location
    computeSpokeAddressPrefix: computeSpokeAddressPrefix
    projectName: projectName
    environment: environment
    aksNodeCount: aksNodeCount
    aksVmSize: aksVmSize
    aksKubernetesVersion: aksKubernetesVersion
    enablePrivateCluster: enablePrivateCluster
  }
}

module peering 'modules/peering.bicep' = {
  name: 'peering-deployment'
  params: {
    projectName: projectName
    environment: environment
    hubVNetId: hub.outputs.hubVNetId
    hubVNetName: hub.outputs.hubVNetName
    computeSpokeVNetId: spokeCompute.outputs.computeSpokeVNetId
    computeSpokeVNetName: spokeCompute.outputs.computeSpokeVNetName
    dataSpokeVNetId: spokeData.outputs.dataSpokeVNetId
    dataSpokeVNetName: spokeData.outputs.dataSpokeVNetName
  }
}

output hubVNetId string = hub.outputs.hubVNetId
output aksClusterId string = spokeCompute.outputs.aksClusterId
output aksClusterName string = spokeCompute.outputs.aksClusterName
output aksClusterFqdn string = spokeCompute.outputs.aksClusterFqdn
output sqlServerFqdn string = spokeData.outputs.sqlServerFqdn
output storageAccountName string = spokeData.outputs.storageAccountName
