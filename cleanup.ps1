#!/usr/bin/env pwsh
# cleanup.ps1 — Clean up Azure resources and local configuration
# Removes Azure deployment, clears environment variables, and deletes local artifacts.

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "`n=== Generative AI for Beginners .NET — Cleanup ===" -ForegroundColor Cyan

$cleanedUp = @()

# --- Delete Azure resources ---
Write-Host "`nDeleting Azure resources with 'azd down --purge --force'..." -ForegroundColor Yellow
try {
    azd down --purge --force
    if ($LASTEXITCODE -eq 0) {
        $cleanedUp += "Azure resources deleted"
        Write-Host "Azure resources deleted successfully." -ForegroundColor Green
    }
} catch {
    Write-Host "ERROR: Failed to run 'azd down --purge --force'." -ForegroundColor Red
    exit 1
}

# --- Delete local .azure folder ---
$azurePath = ".azure"
if (Test-Path $azurePath) {
    Write-Host "`nRemoving local '$azurePath' folder..." -ForegroundColor Yellow
    Remove-Item -Path $azurePath -Recurse -Force
    $cleanedUp += "Local .azure folder deleted"
    Write-Host "'$azurePath' folder removed." -ForegroundColor Green
}

# --- Clear User Secrets ---
$secretsId = "genai-beginners-dotnet"
Write-Host "`nClearing User Secrets (--id $secretsId)..." -ForegroundColor Yellow
try {
    dotnet user-secrets clear --id $secretsId
    $cleanedUp += "User Secrets cleared ($secretsId)"
    Write-Host "  Cleared: User Secrets ($secretsId)" -ForegroundColor Green
} catch {
    Write-Host "  WARNING: Could not clear User Secrets" -ForegroundColor Yellow
}

# --- Print cleanup summary ---
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  Cleanup Summary:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
foreach ($item in $cleanedUp) {
    Write-Host "  ✓ $item" -ForegroundColor Green
}
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "Cleanup complete!" -ForegroundColor Green
