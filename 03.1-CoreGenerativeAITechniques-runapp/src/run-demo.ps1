# Single-file .NET demo runner for Windows
# This script simulates .NET 10's "dotnet run file.cs" functionality
# Usage: .\run-demo.ps1 01-llm-completion.cs

param(
    [Parameter(Mandatory=$false)]
    [string]$DemoFile
)

if (-not $DemoFile) {
    Write-Host "Usage: .\run-demo.ps1 <demo-file.cs>"
    Write-Host ""
    Write-Host "Available demos:"
    Get-ChildItem -Filter "*.cs" | Sort-Object Name | ForEach-Object { Write-Host "  $($_.Name)" }
    exit 1
}

if (-not (Test-Path $DemoFile)) {
    Write-Host "Error: File '$DemoFile' not found" -ForegroundColor Red
    exit 1
}

$DemoName = [System.IO.Path]::GetFileNameWithoutExtension($DemoFile)

Write-Host "🚀 Running $DemoFile (simulating 'dotnet run $DemoFile')" -ForegroundColor Green
Write-Host "Note: When .NET 10 is available, use: dotnet run $DemoFile" -ForegroundColor Yellow
Write-Host ""

# Create temporary project directory
$TempDir = Join-Path $env:TEMP "dotnet-run-$DemoName-$(Get-Random)"
New-Item -ItemType Directory -Path $TempDir -Force | Out-Null

try {
    # Copy the demo file as Program.cs
    Copy-Item $DemoFile (Join-Path $TempDir "Program.cs")

    # Create project file
    $ProjectContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UserSecretsId>03-1-core-generative-ai-runapp</UserSecretsId>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.AI" Version="9.7.1" />
    <PackageReference Include="Microsoft.Extensions.AI.AzureAIInference" Version="9.6.0-preview.1.25310.2" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.7" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="9.0.7" />
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="9.0.7" />
    <PackageReference Include="Microsoft.SemanticKernel.Connectors.InMemory" Version="1.62.0-preview" />
  </ItemGroup>
</Project>
"@

    $ProjectContent | Out-File -FilePath (Join-Path $TempDir "TempDemo.csproj") -Encoding UTF8

    # Build and run in the temp directory
    Push-Location $TempDir
    dotnet run
}
finally {
    # Cleanup
    Pop-Location
    if (Test-Path $TempDir) {
        Remove-Item -Path $TempDir -Recurse -Force
    }
}