// Model deployment for gpt-5-mini on an existing Azure OpenAI account

@description('Name of the parent Azure OpenAI service')
param openAiServiceName string

@description('Azure region (unused but kept for consistency)')
param location string

resource openAi 'Microsoft.CognitiveServices/accounts@2024-10-01' existing = {
  name: openAiServiceName
}

resource deployment 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  parent: openAi
  name: 'gpt-5-mini'
  sku: {
    name: 'GlobalStandard'
    capacity: 10 // 10K TPM
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-5-mini'
      version: '2025-08-07'
    }
  }
}

output deploymentName string = deployment.name
