#!/bin/bash

# Single-file .NET demo runner
# This script simulates .NET 10's "dotnet run file.cs" functionality
# Usage: ./run-demo.sh 01-llm-completion.cs

if [ $# -eq 0 ]; then
    echo "Usage: ./run-demo.sh <demo-file.cs>"
    echo ""
    echo "Available demos:"
    ls *.cs 2>/dev/null | sort
    exit 1
fi

DEMO_FILE=$1
DEMO_NAME=$(basename "$DEMO_FILE" .cs)

if [ ! -f "$DEMO_FILE" ]; then
    echo "Error: File '$DEMO_FILE' not found"
    exit 1
fi

echo "🚀 Running $DEMO_FILE (simulating 'dotnet run $DEMO_FILE')"
echo "Note: When .NET 10 is available, use: dotnet run $DEMO_FILE"
echo ""

# Create temporary project directory
TEMP_DIR="/tmp/dotnet-run-$DEMO_NAME-$$"
mkdir -p "$TEMP_DIR"

# Copy the demo file as Program.cs
cp "$DEMO_FILE" "$TEMP_DIR/Program.cs"

# Create project file
cat > "$TEMP_DIR/TempDemo.csproj" << EOF
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
EOF

# Build and run in the temp directory
cd "$TEMP_DIR"
dotnet run

# Cleanup
cd - > /dev/null
rm -rf "$TEMP_DIR"