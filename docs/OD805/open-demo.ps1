<#
.SYNOPSIS
    Opens VS Code with every file needed for the OD805 "AI Building Blocks for .NET" demo,
    in story order, so you don't have to hunt for them on stage.

.DESCRIPTION
    Opens the session script first (your teleprompter), then each demo file grouped by
    block. Files open as tabs in a single VS Code window. Uses VS Code Insiders if found,
    otherwise stable VS Code.

.PARAMETER Stable
    Force the stable `code` CLI even if `code-insiders` is installed.

.EXAMPLE
    pwsh ./docs/OD805/open-demo.ps1
    # Opens all demo files in VS Code Insiders (or stable if Insiders not found).

.EXAMPLE
    pwsh ./docs/OD805/open-demo.ps1 -Stable
    # Force the stable `code` CLI.
#>
[CmdletBinding()]
param(
    [switch]$Stable
)

$ErrorActionPreference = 'Stop'

# Repo root = two levels up from this script (docs/OD805/ -> repo root).
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

# Pick the VS Code CLI.
function Get-CodeCli {
    param([switch]$PreferStable)
    $candidates = if ($PreferStable) { @('code', 'code-insiders') } else { @('code-insiders', 'code') }
    foreach ($c in $candidates) {
        if (Get-Command $c -ErrorAction SilentlyContinue) { return $c }
    }
    return $null
}

$code = Get-CodeCli -PreferStable:$Stable
if (-not $code) {
    Write-Error "Neither 'code-insiders' nor 'code' is on PATH. In VS Code run: Command Palette -> 'Shell Command: Install code command in PATH'."
    return
}

# Demo files in story order. The session script opens first; the LAST file in the list
# gets focus, so the script is re-listed at the end to land you back on your teleprompter.
$demoFiles = @(
    # Block 0/6 -- cold open + reassembly (teleprompter)
    'docs/OD805/02-session-script.md'

    # Block 2 -- Foundations: Microsoft.Extensions.AI (chat)
    'samples/CoreSamples/BasicChat-05AIFoundryModels/app.cs'
    'samples/CoreSamples/BasicChat-03Ollama/app.cs'

    # Block 3 -- Intelligence: VectorData + DataIngestion
    'samples/CoreSamples/RAGSimple-02MEAIVectorsMemory/Program.cs'
    'samples/CoreSamples/DataIngestion-01-Simple/Program.cs'

    # Block 4 -- Tools: MCP with the C# SDK
    'samples/CoreSamples/MCP-03-MicrosoftLearn/Program.cs'

    # Block 5 -- The whole package: Microsoft Agent Framework + A2A
    'samples/MAF/MAF01/Program.cs'
    'samples/MAF/MAF-MCP-01/Program.cs'
    'samples/MAF/A2A-01/Program.cs'

    # Block 5 (optional) -- an agent that paints: GPT-Image-2 as a MAF tool
    'samples/MAF/MAF-ImageGen-03-Foundry/Program.cs'

    # Re-open the script last so it has focus when VS Code lands.
    'docs/OD805/02-session-script.md'
)

# Resolve to absolute paths, warn (don't fail) on anything missing.
$resolved = [System.Collections.Generic.List[string]]::new()
foreach ($rel in $demoFiles) {
    $full = Join-Path $repoRoot $rel
    if (Test-Path $full) {
        $resolved.Add($full)
    }
    else {
        Write-Warning "Skipping missing file: $rel"
    }
}

if ($resolved.Count -eq 0) {
    Write-Error "No demo files found under $repoRoot."
    return
}

Write-Host "Opening $($resolved.Count) demo file(s) in '$code'..." -ForegroundColor Cyan
& $code --reuse-window @resolved
Write-Host "Done. Files are open as tabs in story order (Block 0 -> 5)." -ForegroundColor Green
