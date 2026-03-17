#Requires -Version 7
<#
.SYNOPSIS
    Recompile et redémarre l'environnement Docker Compose complet.
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$ComposeFile = "src/popsalon/docker-compose.yml"

Write-Host "`n=== Arrêt des conteneurs ===" -ForegroundColor Cyan
docker compose -f $ComposeFile down

Write-Host "`n=== Build .NET backend ===" -ForegroundColor Cyan
Push-Location "src/popsalon/backend"
dotnet build Popsalon.sln --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "`n❌ Build .NET échoué. Arrêt." -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location

Write-Host "`n=== Build frontend ===" -ForegroundColor Cyan
Push-Location "src/popsalon/frontend"
npm run build
if ($LASTEXITCODE -ne 0) {
    Write-Host "`n❌ Build frontend échoué. Arrêt." -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location

Write-Host "`n=== Build & démarrage Docker Compose ===" -ForegroundColor Cyan
docker compose -f $ComposeFile up -d --build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Environnement démarré :" -ForegroundColor Green
    Write-Host "   Frontend : http://localhost:8088" -ForegroundColor Green
    Write-Host "   API      : http://localhost:5000" -ForegroundColor Green
    Write-Host "   Scalar   : http://localhost:5000/scalar/v1" -ForegroundColor Green
} else {
    Write-Host "`n❌ docker compose up échoué." -ForegroundColor Red
    exit 1
}
