$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
dotnet run --project (Join-Path $repoRoot 'checks\SqlIntegration\SqlIntegration.csproj') --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Reception and system integration checks failed.' }