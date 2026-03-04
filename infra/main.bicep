// Main orchestrator for Azure OpenAI deployment
// Run with: azd up
targetScope = 'subscription'

@description('Azure region for all resources')
param location string

@description('Environment name (used to generate unique resource names)')
param environmentName string

@description('Name of the resource group')
param resourceGroupName string = 'rg-${environmentName}'

@description('Name of the Azure OpenAI service')
param openAiServiceName string = 'openai-${environmentName}'

@description('SKU for the Azure OpenAI service')
param openAiSkuName string = 'S0'

// Resource Group
resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: location
}

// Azure OpenAI (Cognitive Services) account
module openAi 'modules/cognitive-services.bicep' = {
  name: 'openai'
  scope: rg
  params: {
    name: openAiServiceName
    location: location
    skuName: openAiSkuName
  }
}

// gpt-5-mini model deployment
module gpt5mini 'modules/model-deployment-chat.bicep' = {
  name: 'gpt5mini'
  scope: rg
  params: {
    openAiServiceName: openAi.outputs.name
    location: location
  }
}

// text-embedding-3-small deployment
module embedding 'modules/model-deployment-embedding.bicep' = {
  name: 'embedding'
  scope: rg
  params: {
    openAiServiceName: openAi.outputs.name
    location: location
  }
}

// Outputs for azd and downstream use
output AZURE_OPENAI_ENDPOINT string = openAi.outputs.endpoint
output AZURE_OPENAI_DEPLOYMENT_NAME string = gpt5mini.outputs.deploymentName
output AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME string = embedding.outputs.deploymentName
