#!/usr/bin/env pwsh
<#
Pre-commit Git hook — vérifications légères avant création du commit.

Comportement:
- Détecte les fichiers mis en staging (staged) et exécute une validation minimale
  en appelant `tools/validate-change-metadata.ps1` sur ces fichiers.
- Si la validation échoue, le commit est annulé.

Note: garder ce hook léger pour ne pas ralentir les commits. Les validations lourdes
restent dans `pre-push` et dans la CI.
#>

try {
    $repoRoot = (git rev-parse --show-toplevel).Trim()
} catch {
    Write-Host "Cannot determine git repo root; skipping pre-commit validation." -ForegroundColor Yellow
    exit 0
}

$script = Join-Path $repoRoot 'tools\validate-change-metadata.ps1'
if (-not (Test-Path $script)) {
    Write-Host "Validation script not found at $script; skipping pre-commit validation." -ForegroundColor Yellow
    exit 0
}

# Get staged files
$staged = git diff --cached --name-only --diff-filter=ACM
if (-not $staged) {
    exit 0
}

$files = $staged -split "`n" | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' }
if ($files.Count -eq 0) { exit 0 }

$list = ($files -join ',')

Write-Host "Pre-commit: validating staged files: $list"
pwsh -File $script --files $list
$exit = $LASTEXITCODE
if ($exit -ne 0) {
    Write-Host "Pre-commit validation failed — aborting commit." -ForegroundColor Red
    exit $exit
}

Write-Host "Pre-commit validation passed." -ForegroundColor Green
exit 0
