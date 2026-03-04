// Azure Cognitive Services account configured for OpenAI

@description('Name of the OpenAI service')
param name string

@description('Azure region')
param location string

@description('SKU name')
param skuName string = 'S0'

resource openAi 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: name
  location: location
  kind: 'OpenAI'
  sku: {
    name: skuName
  }
  properties: {
    publicNetworkAccess: 'Enabled'
    customSubDomainName: name
  }
}

output endpoint string = openAi.properties.endpoint
output name string = openAi.name
output id string = openAi.id
