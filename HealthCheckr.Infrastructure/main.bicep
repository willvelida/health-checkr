@description('The location that resources will be deployed to. Default is the location of the resource group')
param location string = resourceGroup().location

@description('Name of the App Service Plan')
param appServicePlanName string

@description('Name of the App Insights instance that will be deployed')
param appInsightsName string

@description('Name of the Cosmos DB account that will be deployed')
param cosmosDBAccountName string

@description('The name of the database in this Cosmos DB account')
param databaseName string

@description('The name of the container in this Cosmos DB account')
param containerName string

@description('The name of the Key Vault that will be deployed')
param keyVaultName string

@description('The name of the App Configuration resource that will be deployed')
param appConfigurationName string

@description('The name of the Service Bus Namespace that will be deployed')
param serviceBusNamespaceName string

@description('The time that the resource was last deployed')
param lastDeployed string = utcNow()

var tags = {
  ApplicationName: 'health-checkr'
  Component: 'Common Infrastructure'
  Environment: 'Production'
  LastDeployed: lastDeployed
}

module appConfig 'modules/app-config.bicep' = {
  name: 'appconfig'
  params: {
    appConfigurationName: appConfigurationName
    location: location
    tags: tags
  }
}

module appInsights 'modules/app-insights.bicep' = {
  name: 'appins'
  params: {
    appInsightsName: appInsightsName
    tags: tags
    location: location
  }
}

module appService 'modules/app-service-plan.bicep' = {
  name: 'asp'
  params: {
    appServicePlanName: appServicePlanName
    location: location
    tags: tags
  }
}

module cosmosDb 'modules/cosmos-db.bicep' = {
  name: 'cosmosdb'
  params: {
    containerName: containerName
    cosmosDBAccountName: cosmosDBAccountName
    databaseName: databaseName
    keyVaultName: keyVault.outputs.keyVaultName
    location: location
    tags: tags
  }
}

module keyVault 'modules/key-vault.bicep' = {
  name: 'kv'
  params: {
    keyVaultName: keyVaultName
    tags: tags
    location: location
  }
}

module serviceBus 'modules/service-bus.bicep' = {
  name: 'servicebus'
  params: {
    serviceBusNamespaceName: serviceBusNamespaceName
    tags: tags
    location: location
  }
}
