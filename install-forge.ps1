#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Compile, package et installe l'outil Forge en tant qu'outil .NET global.
    Une fois installé, la commande "forge" est disponible directement dans n'importe quel terminal.

.DESCRIPTION
    1. Compile Popforge.CodeGen en Release
    2. Crée le package NuGet dans src/forge/nupkg/
    3. Installe (ou met à jour) l'outil forge globalement via dotnet tool install --global

.EXAMPLE
    ./install-forge.ps1              # pack + install global
    ./install-forge.ps1 -Update      # force réinstallation si déjà installé
#>

param(
    [switch]$Update
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot   = $PSScriptRoot
$projectDir = Join-Path $repoRoot "src/forge/Popforge.CodeGen"
$nupkgDir   = Join-Path $repoRoot "src/forge/nupkg"

Write-Host "==> Compilation de Popforge.CodeGen..." -ForegroundColor Cyan
dotnet build $projectDir -c Release --nologo -v q
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "==> Création du package NuGet..." -ForegroundColor Cyan
dotnet pack $projectDir -c Release --nologo -v q -o $nupkgDir
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Vider le cache NuGet pour forcer l'utilisation du nouveau .nupkg
$cacheDir = Join-Path $env:USERPROFILE ".nuget\packages\popforge.codegen"
if (Test-Path $cacheDir) { Remove-Item $cacheDir -Recurse -Force }

Write-Host ""
if ($Update) {
    Write-Host "==> Mise à jour de l'outil forge (global)..." -ForegroundColor Cyan
    dotnet tool uninstall --global Popforge.CodeGen 2>$null | Out-Null
} else {
    Write-Host "==> Installation de l'outil forge (global)..." -ForegroundColor Cyan
}
dotnet tool install --global Popforge.CodeGen --add-source $nupkgDir

if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "✓ forge installé avec succès." -ForegroundColor Green
Write-Host ""
Write-Host "Utilisation (depuis n'importe quel répertoire) :" -ForegroundColor Yellow
Write-Host "  cd src/popsalon"
Write-Host "  forge generate --dry-run"
Write-Host "  forge generate"
Write-Host "  forge new-entity NomEntite"
