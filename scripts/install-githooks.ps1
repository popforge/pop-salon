#!/usr/bin/env pwsh
<#
Install githooks for this repository by setting `core.hooksPath` to `.githooks`.

Usage (run from the repo root):
  .\scripts\install-githooks.ps1

This configures Git to use the bundled hooks directory. Developers should run
this once after cloning the repo.
#>

try {
    $repoRoot = (git rev-parse --show-toplevel).Trim()
} catch {
    Write-Host "Not inside a git repository." -ForegroundColor Red
    exit 1
}

Set-Location $repoRoot

git config core.hooksPath .githooks
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to set core.hooksPath" -ForegroundColor Red
    exit 1
}

Write-Host "Git hooks path set to .githooks."
Write-Host "Ensure .githooks contains executable hook scripts (pre-commit, pre-push)." -ForegroundColor Yellow

exit 0
