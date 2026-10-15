[CmdletBinding()]
param([switch]$Live)

$ErrorActionPreference = 'Stop'
$sample = Join-Path $PSScriptRoot 'app.cs'
$tests = Join-Path $PSScriptRoot '..\MCP-04-ParallelSearch.Tests\MCP-04-ParallelSearch.Tests.csproj'

dotnet build $sample --configuration Release --verbosity minimal
if ($LASTEXITCODE -ne 0) { throw 'Parallel Search sample build failed.' }

dotnet test $tests --configuration Release --verbosity minimal
if ($LASTEXITCODE -ne 0) { throw 'Parallel Search regression tests failed.' }

if ($Live) {
    # The sample does not load credentials or send an Authorization header.
    $lines = @(dotnet run --file $sample --configuration Release --no-build 2>&1)
    $runExitCode = $LASTEXITCODE
    $lines | Write-Output
    if ($runExitCode -ne 0) { throw 'Anonymous Parallel Search smoke test failed.' }

    $output = $lines -join "`n"
    $sections = $output -split 'Fetched page:', 2
    if ($sections.Count -ne 2 -or $sections[0] -notmatch 'Search results:') {
        throw 'The sample did not complete both search and fetch.'
    }
    foreach ($section in $sections) {
        if ($section -notmatch 'Source: https?://\S+' -or $section -notmatch 'Excerpt: \S') {
            throw 'A search or fetch response is missing a source URL or excerpt.'
        }
    }
    Write-Output 'Anonymous search and fetch smoke test passed.'
}
