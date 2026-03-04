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

Write-Host "For AI Foundry Agent samples, also run:" -ForegroundColor Yellow
Write-Host "  dotnet user-secrets set --id $secretsId `"AIFoundry:Endpoint`" `"<your-foundry-endpoint>`""
Write-Host "  dotnet user-secrets set --id $secretsId `"AIFoundry:TenantId`" `"<your-tenant-id>`""

Write-Host "`nFor Azure AI Search (RAG samples), also run:" -ForegroundColor Yellow
Write-Host "  dotnet user-secrets set --id $secretsId `"AzureAISearch:Endpoint`" `"<your-search-endpoint>`""
Write-Host "  dotnet user-secrets set --id $secretsId `"AzureAISearch:Key`" `"<your-search-key>`""

Write-Host "`nDone! Run any sample with: dotnet run (or dotnet run app.cs for file-based samples)`n" -ForegroundColor Green
