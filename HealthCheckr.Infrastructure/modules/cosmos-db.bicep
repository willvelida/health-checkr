@description('Name of the Cosmos DB account that will be deployed')
param cosmosDBAccountName string

@description('The name of the database in this Cosmos DB account')
param databaseName string

@description('The name of the container in this Cosmos DB account')
param containerName string

@description('The location that our Cosmos DB resources will be deployed to')
param location string

@description('The tags that will be applied to the Cosmos DB account')
param tags object

@description('The name of the key vault to store secrets in.')
param keyVaultName string

@description('The App Config instance to store Cosmos Values in')
param appConfigName string

var cosmosPrimaryMasterKeySecretName = 'CosmosDbPrimaryMasterKey'
var cosmosPrimaryReadKeySecretName = 'CosmosDbPrimaryReadKey'
var cosmosSecondaryMasterKeySecretName = 'CosmosDbSecondaryMasterKey'
var cosmosSecondaryReadKeySecretName = 'CosmosDbSecondaryReadKey'
var cosmosConnectionStringSecretName = 'CosmosDbConnectionString'
var cosmosDbEndpointSecretName = 'CosmosDbEndpoint'
var cosmosDatabaseSettingName = 'HealthCheckr:DatabaseName'
var cosmosContainerSettingName = 'HealthCheckr:ContainerName'

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: appConfigName
}

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2022-05-15' = {
  name: cosmosDBAccountName
  location: location
  tags: tags
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    enableFreeTier: true
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2022-05-15' = {
  name: databaseName
  parent: cosmosAccount
  properties: {
    resource: {
      id: databaseName
    }
    options: {
      throughput: 1000
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2022-05-15' = {
  name: containerName
  parent: database
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: [
          '/DocumentType'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        includedPaths: [
          {
            path: '/*'
          }
        ]
      }
    }
  }
}

resource cosmosDbPrimaryMasterKeySecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: cosmosPrimaryMasterKeySecretName
  parent: keyVault
  properties: {
    value: cosmosAccount.listKeys().primaryMasterKey
  }
}

resource cosmosDbPrimaryReadKeySecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: cosmosPrimaryReadKeySecretName
  parent: keyVault
  properties: {
    value: cosmosAccount.listKeys().primaryReadonlyMasterKey
  }
}

resource cosmosDbSecondaryMasterKeySecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: cosmosSecondaryMasterKeySecretName
  parent: keyVault
  properties: {
    value: cosmosAccount.listKeys().primaryMasterKey
  }
}

resource cosmosDbSecondaryReadKeySecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: cosmosSecondaryReadKeySecretName
  parent: keyVault
  properties: {
    value: cosmosAccount.listKeys().primaryReadonlyMasterKey
  }
}

resource cosmosDbConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: cosmosConnectionStringSecretName
  parent: keyVault
  properties: {
    value: cosmosAccount.listConnectionStrings().connectionStrings[0].connectionString
  }
}

resource cosmosDbEndpointSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: cosmosDbEndpointSecretName
  parent: keyVault
  properties: {
    value: cosmosAccount.properties.documentEndpoint
  }
}

resource cosmosDatabaseSetting 'Microsoft.AppConfiguration/configurationStores/keyValues@2022-05-01' = {
  name: cosmosDatabaseSettingName
  parent: appConfig
  properties: {
    value: database.name
  }
}

resource cosmosContainerSetting 'Microsoft.AppConfiguration/configurationStores/keyValues@2022-05-01' = {
  name: cosmosContainerSettingName
  parent: appConfig
  properties: {
    value: container.name
  }
}
