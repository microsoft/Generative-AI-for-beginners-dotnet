// Model deployment for text-embedding-3-small on an existing Azure OpenAI account

@description('Name of the parent Azure OpenAI service')
param openAiServiceName string

@description('Azure region (unused but kept for consistency)')
param location string

resource openAi 'Microsoft.CognitiveServices/accounts@2024-10-01' existing = {
  name: openAiServiceName
}

resource deployment 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  parent: openAi
  name: 'text-embedding-3-small'
  sku: {
    name: 'Standard'
    capacity: 10 // 10K TPM
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'text-embedding-3-small'
      version: '1'
    }
  }
}

output deploymentName string = deployment.name
