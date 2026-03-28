<#
Validate change metadata in modified files.

Usage:
  .\validate-change-metadata.ps1 [--files file1,file2,...]

If `--files` not provided, the script will try to detect changed files using git (HEAD vs main).

Checks performed:
- Each target file contains a `change_id` frontmatter (or recognized comment) matching `CHG-YYYY` pattern.
- A corresponding `design-artifacts/changes/CHG-*.yaml` file exists.

Exit code: 0 if all checks pass, 1 otherwise.
#>

param(
    [string]$files
)

function Get-ChangedFiles {
    try {
        $branch = (git rev-parse --abbrev-ref HEAD).Trim()
        if ($LASTEXITCODE -ne 0) { return @() }
        $base = 'main'
        $names = git diff --name-only $base..$branch
        return $names -split "`n" | Where-Object { $_ -ne '' }
    } catch {
        return @()
    }
}

function Extract-ChangeIdFromFile([string]$path) {
    if (-not (Test-Path $path)) { return $null }
    $content = Get-Content $path -Raw -ErrorAction SilentlyContinue
    if ($null -eq $content) { return $null }
    # Look for YAML frontmatter
    if ($content -match "change_id:\s*(CHG-[0-9]{8}-[0-9]{2})") { return $matches[1] }
    # Look for inline comment pattern e.g. /* change_id: CHG-... */ or // change_id: CHG-...
    if ($content -match "change_id[:|=]\s*(CHG-[0-9]{8}-[0-9]{2})") { return $matches[1] }
    return $null
}

# Targets
if ($files) {
    $targets = $files -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' }
} else {
    $targets = Get-ChangedFiles
}

if (-not $targets -or $targets.Count -eq 0) {
    Write-Host "No target files detected. Provide --files or run from a git repo with an active branch." -ForegroundColor Yellow
    exit 1
}

$ok = $true
foreach ($f in $targets) {
    if (-not (Test-Path $f)) { Write-Host "Skipped missing file: $f" -ForegroundColor DarkYellow; continue }
    $cid = Extract-ChangeIdFromFile $f
    if (-not $cid) {
        Write-Host "[ERROR] $f: missing change_id frontmatter" -ForegroundColor Red
        $ok = $false
        continue
    }
    $chgFile = Join-Path "design-artifacts/changes" ($cid + ".yaml")
    if (-not (Test-Path $chgFile)) {
        Write-Host "[ERROR] $f: change_id $cid declared but $chgFile not found" -ForegroundColor Red
        $ok = $false
        continue
    }
    # Validate that the file is included in the CHG scope
    $chgContent = Get-Content $chgFile -Raw -ErrorAction SilentlyContinue
    $scope = @()
    if ($chgContent -match "(?ms)scope:\s*(?:\n|\r\n)(.*?)(?:\n[^\s-]|\z)") {
        $scopeBlock = $matches[1]
        $scopeLines = $scopeBlock -split "`n" | ForEach-Object { $_.Trim() } | Where-Object { $_ -match '^-' }
        foreach ($line in $scopeLines) {
            $entry = $line -replace '^-\s*', ''
            $entry = $entry.Trim()
            if ($entry) { $scope += $entry }
        }
    }

    $included = $false
    foreach ($entry in $scope) {
        if ($entry -eq $f) { $included = $true; break }
        if ($f -like "*$entry") { $included = $true; break }
        if ($entry -like "*$f") { $included = $true; break }
        $leaf = [System.IO.Path]::GetFileName($f)
        if ($entry -eq $leaf) { $included = $true; break }
    }

    if (-not $included) {
        Write-Host "[ERROR] $f: not listed in scope of $chgFile" -ForegroundColor Red
        $ok = $false
        continue
    }

    Write-Host "[OK] $f -> $cid (scope verified)" -ForegroundColor Green
}

if ($ok) { exit 0 } else { exit 1 }
