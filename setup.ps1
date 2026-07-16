#!/usr/bin/env pwsh
# setup.ps1 — Automated setup for Generative AI for Beginners .NET
# Deploys Azure infrastructure via azd and configures .NET user secrets.

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "`n=== Generative AI for Beginners .NET - Automated Setup ===" -ForegroundColor Cyan

# --- Check prerequisites ---
foreach ($tool in @("azd", "dotnet")) {
    if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
        Write-Host "ERROR: '$tool' is not installed or not on PATH." -ForegroundColor Red
        exit 1
    }
}
if (Get-Command "az" -ErrorAction SilentlyContinue) {
    Write-Host "Prerequisites OK (azd, dotnet, az)" -ForegroundColor Green
} else {
    Write-Host "Prerequisites OK (azd, dotnet). 'az' CLI not found - tenant detection will be skipped." -ForegroundColor Green
}

# --- Deploy Azure infrastructure ---
Write-Host "`nRunning 'azd up' to deploy Azure infrastructure..." -ForegroundColor Yellow
Write-Host "You will be prompted to select a subscription, location, etc.`n"

azd up
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: 'azd up' failed." -ForegroundColor Red
    exit 1
}
Write-Host "`nDeployment complete!" -ForegroundColor Green

# --- Extract the Azure OpenAI endpoint ---
Write-Host "`nExtracting Azure OpenAI endpoint from deployed environment..." -ForegroundColor Yellow

# azd env get-values returns KEY="VALUE" pairs; parse out the endpoint
$envValues = azd env get-values 2>$null
$endpoint = $null
foreach ($line in $envValues) {
    if ($line -match '^\s*AZURE_OPENAI_ENDPOINT\s*=\s*"?([^"]+)"?\s*$') {
        $endpoint = $Matches[1]
    }
}

if (-not $endpoint) {
    Write-Host "Could not auto-detect endpoint from azd env. Trying az CLI..." -ForegroundColor Yellow
    if (Get-Command "az" -ErrorAction SilentlyContinue) {
        $rgLine = $envValues | Where-Object { $_ -match 'AZURE_RESOURCE_GROUP' }
        if ($rgLine -match '=\s*"?([^"]+)"?') {
            $rg = $Matches[1]
            $endpoint = az cognitiveservices account list --resource-group $rg --query "[0].properties.endpoint" -o tsv 2>$null
        }
    }
}

if ($endpoint) {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "  Azure OpenAI Endpoint:" -ForegroundColor Cyan
    Write-Host "  $endpoint" -ForegroundColor White
    Write-Host "========================================`n" -ForegroundColor Cyan
} else {
    Write-Host "WARNING: Could not extract endpoint automatically." -ForegroundColor Red
    Write-Host "Run:  azd env get-values  to find it manually.`n"
}

# --- Set User Secrets ---
$chatDeploymentName = "gpt-5-mini"
$embeddingDeploymentName = "text-embedding-3-small"
$secretsId = "genai-beginners-dotnet"

if ($endpoint) {
    Write-Host "Setting User Secrets (--id $secretsId)..." -ForegroundColor Yellow
    dotnet user-secrets set --id $secretsId "AzureOpenAI:Endpoint" $endpoint
    dotnet user-secrets set --id $secretsId "AzureOpenAI:Deployment" $chatDeploymentName
    dotnet user-secrets set --id $secretsId "AzureOpenAI:EmbeddingDeployment" $embeddingDeploymentName
    Write-Host "User Secrets configured!" -ForegroundColor Green
} else {
    Write-Host "Skipping User Secrets (no endpoint detected). Set them manually:" -ForegroundColor Yellow
    Write-Host "  dotnet user-secrets set --id $secretsId `"AzureOpenAI:Endpoint`" `"<your-endpoint>`""
    Write-Host "  dotnet user-secrets set --id $secretsId `"AzureOpenAI:Deployment`" `"$chatDeploymentName`""
    Write-Host "  dotnet user-secrets set --id $secretsId `"AzureOpenAI:EmbeddingDeployment`" `"$embeddingDeploymentName`""
}

# --- Detect tenant and remind user to az login to the correct one ---
if (Get-Command "az" -ErrorAction SilentlyContinue) {
    $subId = $null
    foreach ($line in $envValues) {
        if ($line -match '^\s*AZURE_SUBSCRIPTION_ID\s*=\s*"?([^"]+)"?\s*$') {
            $subId = $Matches[1]
        }
    }
    if ($subId) {
        $tenantId = az account show --subscription $subId --query tenantId -o tsv 2>$null
        if ($tenantId) {
            Write-Host "`nIMPORTANT: Before running the samples, login to the correct tenant:" -ForegroundColor Yellow
            Write-Host "  az login --tenant $tenantId" -ForegroundColor White
        }
    }
}

# --- Summary ---
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  Setup Summary:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Secrets ID:           $secretsId" -ForegroundColor White
Write-Host "  Chat Deployment:      $chatDeploymentName" -ForegroundColor White
Write-Host "  Embedding Deployment: $embeddingDeploymentName" -ForegroundColor White
if ($endpoint) {
    Write-Host "  Endpoint:             $endpoint" -ForegroundColor White
}
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "Setup complete. Navigate to a sample folder and run:" -ForegroundColor Green
Write-Host "  dotnet run app.cs`n"
