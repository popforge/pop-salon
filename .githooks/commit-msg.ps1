#!/usr/bin/env pwsh
<#
Commit-msg hook — exige qu'un identifiant CHG-YYYYMMDD-NN apparaisse dans le message de commit.

Git passe le chemin du fichier contenant le message de commit en argument.
Exit non-zero pour bloquer le commit si l'identifiant n'est pas trouvé.
#>

param(
    [string]$msgFile
)

if (-not $msgFile) {
    Write-Host "commit-msg hook: missing message file argument." -ForegroundColor Yellow
    exit 0
}

$content = Get-Content $msgFile -Raw -ErrorAction SilentlyContinue
if (-not $content) { exit 0 }

if ($content -match "CHG-[0-9]{8}-[0-9]{2}") {
    Write-Host "Commit message contains CHG id." -ForegroundColor Green
    exit 0
} else {
    Write-Host "[ERROR] Commit message must reference a CHG id (example: CHG-20260327-01)." -ForegroundColor Red
    Write-Host "Either include the CHG id in the message or use --no-verify to bypass (not recommended)." -ForegroundColor Yellow
    exit 1
}
