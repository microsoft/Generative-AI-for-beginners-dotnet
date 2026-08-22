targetScope = 'subscription'

@minLength(1)
@maxLength(64)
param environmentName string

param location string

@minLength(2)
param foundryAccountName string

@minLength(1)
param foundryResourceGroupName string

@minLength(1)
param foundryEndpoint string

@minLength(1)
param foundryDeploymentName string

var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var resourceGroupName = 'rg-${environmentName}'

resource appResourceGroup 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: resourceGroupName
  location: location
  tags: {
    'azd-env-name': environmentName
  }
}

module app 'app.bicep' = {
  name: 'app-${resourceToken}'
  scope: appResourceGroup
  params: {
    location: location
    resourceToken: resourceToken
    foundryEndpoint: foundryEndpoint
    foundryDeploymentName: foundryDeploymentName
  }
}

module foundryAccess 'foundry-access.bicep' = {
  name: 'foundry-access-${resourceToken}'
  scope: resourceGroup(foundryResourceGroupName)
  params: {
    foundryAccountName: foundryAccountName
    principalId: app.outputs.functionPrincipalId
  }
}

output AZURE_LOCATION string = location
output AZURE_RESOURCE_GROUP string = resourceGroupName
output SERVICE_API_NAME string = app.outputs.functionAppName
output SERVICE_API_URI string = app.outputs.functionAppUri
output MANAGED_IDENTITY_NAME string = app.outputs.managedIdentityName
