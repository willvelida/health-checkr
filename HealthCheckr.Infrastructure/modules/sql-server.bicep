@description('The name of the SQL logical server.')
param serverName string

@description('The name of the SQL Database.')
param sqlDBName string

@description('Location for all resources.')
param location string = resourceGroup().location

@description('The administrator username of the SQL logical server.')
param administratorLogin string

@description('The administrator password of the SQL logical server.')
param administratorLoginPassword string

@description('The tags applied to this SQL Server')
param tags object

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: serverName
  location: location
  tags: tags
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  name: sqlDBName
  location: location
  parent: sqlServer
  tags: tags
  sku: {
    name: 'Free'
    tier: 'Free'
  }
}

