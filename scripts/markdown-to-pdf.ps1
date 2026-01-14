param(
    [Parameter(Position=0)]
    [string]$Path = ".",

    [Parameter()]
    [switch]$Recurse = $true,

    [Parameter()]
    [string]$OutputDir
)

function Write-Info($msg) { Write-Host "[md->pdf] $msg" -ForegroundColor Cyan }
function Write-Err($msg) { Write-Host "[md->pdf] $msg" -ForegroundColor Red }

# Resolve path
$root = Resolve-Path $Path
Write-Info "Root: $root"

# Verify npx availability
if (-not (Get-Command npx -ErrorAction SilentlyContinue)) {
    Write-Err "npx is not available. Please install Node.js (https://nodejs.org)."
    exit 1
}

# Create output directory if provided
if ($OutputDir) {
    $outResolved = Resolve-Path -Path $OutputDir -ErrorAction SilentlyContinue
    if (-not $outResolved) {
        New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
        $outResolved = Resolve-Path -Path $OutputDir
    }
    Write-Info "OutputDir: $outResolved"
}

# Gather markdown files
$searchOpts = @{ Path = $root; Filter = "*.md"; File = $true; }
if ($Recurse) { $searchOpts.Recurse = $true }
$files = Get-ChildItem @searchOpts |
    Where-Object { $_.FullName -notmatch "\\node_modules\\" }

if ($files.Count -eq 0) {
    Write-Err "No Markdown files found under $root"
    exit 2
}

Write-Info "Found $($files.Count) Markdown file(s)."

$errors = @()
foreach ($f in $files) {
    $targetPdf = if ($OutputDir) {
        $baseName = [System.IO.Path]::GetFileNameWithoutExtension($f.Name)
        Join-Path $OutputDir "$baseName.pdf"
    } else {
        [System.IO.Path]::ChangeExtension($f.FullName, ".pdf")
    }

    Write-Info "Converting: $($f.FullName) -> $targetPdf"
    try {
        # Use npx to run md-to-pdf without prompt
        $cmd = "npx --yes md-to-pdf `"$($f.FullName)`""
        $null = Invoke-Expression $cmd
        # md-to-pdf outputs in same folder by default; move if OutputDir specified
        if ($OutputDir) {
            $produced = [System.IO.Path]::ChangeExtension($f.FullName, ".pdf")
            if (Test-Path $produced) {
                Move-Item -Force -Path $produced -Destination $targetPdf
            }
        }
    }
    catch {
        Write-Err "Failed: $($f.FullName) - $($_.Exception.Message)"
        $errors += $f.FullName
    }
}

if ($errors.Count -gt 0) {
    Write-Err "Completed with $($errors.Count) error(s)."
    foreach ($e in $errors) { Write-Err " - $e" }
    exit 3
}

Write-Info "All conversions completed successfully."