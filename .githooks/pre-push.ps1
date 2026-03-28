#!/usr/bin/env pwsh
<#
Pre-push Git hook — exécute la validation locale des `change_id` avant push.

Pour activer localement :
  git config core.hooksPath .githooks

Le hook exécute `tools/validate-change-metadata.ps1` depuis la racine du repo.
Bloque le push si la validation échoue.
#>

try {
    $repoRoot = (git rev-parse --show-toplevel).Trim()
} catch {
    Write-Host "Cannot determine git repo root; skipping validation." -ForegroundColor Yellow
    exit 0
}

$script = Join-Path $repoRoot 'tools\validate-change-metadata.ps1'
if (-not (Test-Path $script)) {
    Write-Host "Validation script not found at $script; skipping validation." -ForegroundColor Yellow
    exit 0
}

Write-Host "Running change metadata validation..."
pwsh -File $script
$exit = $LASTEXITCODE
if ($exit -ne 0) {
    Write-Host "Change metadata validation failed — aborting push." -ForegroundColor Red
    exit $exit
}
Write-Host "Change metadata validation passed." -ForegroundColor Green
exit 0
