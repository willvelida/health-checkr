@description('The name of the Key Vault that will be deployed')
param keyVaultName string

@description('The location that our resources will be deployed to.')
param location string = resourceGroup().location

@description('Flag to indicate if this is a new Key Vault, and therefore should have no access policies configured. Default is false')
param isNewKeyVault bool = false

@description('The tags that will be applied to the Key Vault')
param tags object

var accessPolicies = isNewKeyVault ? [] : reference(resourceId('Microsoft.KeyVault/vaults', keyVaultName), '2022-07-01').accessPolicies

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enabledForTemplateDeployment: true
    accessPolicies: accessPolicies
  }
}

output keyVaultName string = keyVault.name
