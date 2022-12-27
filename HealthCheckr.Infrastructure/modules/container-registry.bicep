@description('The name of the container registry that will be deployed')
param acrName string

@description('The location to deploy the container registry')
param location string

@description('The tags to apply to the container registry')
param tags object

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2022-02-01-preview' = {
  name: acrName
  location: location
  tags: tags
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
  identity: {
    type: 'SystemAssigned'
  }
}
