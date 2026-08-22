$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($env:AZURE_SUBSCRIPTION_ID) -or
    [string]::IsNullOrWhiteSpace($env:FOUNDRY_RESOURCE_GROUP_NAME) -or
    [string]::IsNullOrWhiteSpace($env:FOUNDRY_ACCOUNT_NAME)) {
    Write-Host 'Foundry cleanup settings are incomplete; no Foundry role assignment could have been provisioned.'
    exit 0
}

$resourceGroup = $env:AZURE_RESOURCE_GROUP
if ([string]::IsNullOrWhiteSpace($resourceGroup) -and
    -not [string]::IsNullOrWhiteSpace($env:AZURE_ENV_NAME)) {
    $resourceGroup = "rg-$($env:AZURE_ENV_NAME)"
}

if ([string]::IsNullOrWhiteSpace($resourceGroup)) {
    Write-Host 'No application resource group is available for Foundry role cleanup.'
    exit 0
}

$resourceGroupExists = az group exists `
    --subscription $env:AZURE_SUBSCRIPTION_ID `
    --name $resourceGroup

if ($LASTEXITCODE -ne 0) {
    throw "Could not determine whether the application resource group exists."
}

if ($resourceGroupExists -ne 'true') {
    Write-Host 'The application resource group does not exist; no Foundry role assignment needs removal.'
    exit 0
}

$managedIdentityName = $env:MANAGED_IDENTITY_NAME
if ([string]::IsNullOrWhiteSpace($managedIdentityName)) {
    $managedIdentityName = az identity list `
        --subscription $env:AZURE_SUBSCRIPTION_ID `
        --resource-group $resourceGroup `
        --query '[0].name' `
        --output tsv

    if ($LASTEXITCODE -ne 0) {
        throw "Could not discover the managed identity for Foundry role cleanup."
    }
}

if ([string]::IsNullOrWhiteSpace($managedIdentityName)) {
    Write-Host 'No managed identity exists; no Foundry role assignment needs removal.'
    exit 0
}

$principalId = az identity show `
    --subscription $env:AZURE_SUBSCRIPTION_ID `
    --resource-group $resourceGroup `
    --name $managedIdentityName `
    --query principalId `
    --output tsv

if ($LASTEXITCODE -ne 0) {
    throw "Could not query the managed identity before cleanup."
}

if ([string]::IsNullOrWhiteSpace($principalId)) {
    Write-Host 'The managed identity has no principal; no Foundry role assignment needs removal.'
    exit 0
}

$foundryScope = "/subscriptions/$($env:AZURE_SUBSCRIPTION_ID)/resourceGroups/$($env:FOUNDRY_RESOURCE_GROUP_NAME)/providers/Microsoft.CognitiveServices/accounts/$($env:FOUNDRY_ACCOUNT_NAME)"
$roleName = 'Cognitive Services OpenAI User'
$assignmentIds = @(
    az role assignment list `
        --subscription $env:AZURE_SUBSCRIPTION_ID `
        --assignee-object-id $principalId `
        --scope $foundryScope `
        --role $roleName `
        --query '[].id' `
        --output tsv
)

if ($LASTEXITCODE -ne 0) {
    throw "Could not query the managed identity's Foundry role assignment."
}

if ($assignmentIds.Count -eq 0) {
    Write-Host 'No managed identity Foundry role assignment needs removal.'
    exit 0
}

az role assignment delete --subscription $env:AZURE_SUBSCRIPTION_ID --ids $assignmentIds

if ($LASTEXITCODE -ne 0) {
    throw "Could not remove the managed identity's Foundry role assignment."
}

Write-Host 'Removed the managed identity Foundry role assignment.'
