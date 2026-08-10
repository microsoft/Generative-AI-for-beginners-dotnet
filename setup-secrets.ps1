#!/usr/bin/env pwsh
# setup-secrets.ps1 — Manually configure user secrets for all samples
# Use this if you already have an Azure OpenAI resource and don't want to use azd.

param(
    [Parameter(Mandatory=$true)]
    [string]$Endpoint,
    
    [string]$Deployment = "gpt-5-mini",
    
    [string]$EmbeddingDeployment = "text-embedding-3-small"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$secretsId = "genai-beginners-dotnet"

Write-Host "`n=== Generative AI for Beginners .NET — Secret Setup ===" -ForegroundColor Cyan

# Check prerequisites
if (-not (Get-Command "dotnet" -ErrorAction SilentlyContinue)) {
    Write-Host "ERROR: 'dotnet' is not installed or not on PATH." -ForegroundColor Red
    exit 1
}

Write-Host "Setting User Secrets (ID: $secretsId)..." -ForegroundColor Yellow

dotnet user-secrets set --id $secretsId "AzureOpenAI:Endpoint" $Endpoint
dotnet user-secrets set --id $secretsId "AzureOpenAI:Deployment" $Deployment
dotnet user-secrets set --id $secretsId "AzureOpenAI:EmbeddingDeployment" $EmbeddingDeployment

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  User Secrets configured!" -ForegroundColor Green
Write-Host "  Secrets ID: $secretsId" -ForegroundColor White
Write-Host "  Endpoint:   $Endpoint" -ForegroundColor White
Write-Host "  Chat Model: $Deployment" -ForegroundColor White
Write-Host "  Embedding:  $EmbeddingDeployment" -ForegroundColor White
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "For the MAF Foundry Agent samples (MAF-MicrosoftFoundryAgents-*, MAF-AIFoundryAgents-01, MAF-MultiAgents), also run:" -ForegroundColor Yellow
Write-Host "  dotnet user-secrets set --id $secretsId `"azureFoundryProjectEndpoint`" `"<your-foundry-project-endpoint>`""
Write-Host "  dotnet user-secrets set --id $secretsId `"agentName`" `"<your-agent-name>`""

Write-Host "`nFor the AgentLabs samples (AgentLabs-01/02/03), also run:" -ForegroundColor Yellow
Write-Host "  dotnet user-secrets set --id $secretsId `"aifoundryproject_endpoint`" `"<your-foundry-project-endpoint>`""
Write-Host "  dotnet user-secrets set --id $secretsId `"aifoundryproject_tenantid`" `"<your-tenant-id>`""

Write-Host "`nFor Azure AI Search (RAGSimple-03MEAIVectorsAISearch), also run:" -ForegroundColor Yellow
Write-Host "  dotnet user-secrets set --id $secretsId `"AZURE_AISEARCH_URI`" `"<your-search-endpoint>`""
Write-Host "  dotnet user-secrets set --id $secretsId `"AZURE_AISEARCH_SECRET`" `"<your-search-key>`""

Write-Host "`nDone! Make sure to run 'az login' first, then run file-based samples with: dotnet run app.cs`n" -ForegroundColor Green
