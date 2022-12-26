@description('The location that resources will be deployed to. Default is the location of the resource group')
param location string = resourceGroup().location

@description('Name of the App Service Plan')
param appServicePlanName string

@description('Name of the App Insights instance that will be deployed')
param appInsightsName string

@description('The name of the Key Vault that will be deployed')
param keyVaultName string

@description('The time that the resource was last deployed')
param lastDeployed string = utcNow()

var tags = {
  ApplicationName: 'health-checkr'
  Component: 'Common Infrastructure'
  Environment: 'Production'
  LastDeployed: lastDeployed
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

module keyVault 'modules/key-vault.bicep' = {
  name: 'kv'
  params: {
    keyVaultName: keyVaultName
    tags: tags
    location: location
  }
}
