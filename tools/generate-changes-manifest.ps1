<#
Generate a machine-readable manifest from design-artifacts/changes/*.yaml

Outputs: design-artifacts/changes/manifest.json

This script parses simple YAML entries (id, title, status, pr, commit, scope list)
and writes a JSON manifest useful for agents and tooling.
#>

param(
    [string]$out = "design-artifacts/changes/manifest.json"
)

function Parse-ChgFile([string]$path) {
    $content = Get-Content $path -Raw -ErrorAction SilentlyContinue
    if (-not $content) { return $null }
    $obj = [ordered]@{}
    # id
    if ($content -match "^id:\s*(.+)$") { $obj.id = $matches[1].Trim() }
    if ($content -match "^title:\s*\"?(.+?)\"?$") { $obj.title = $matches[1].Trim() }
    if ($content -match "^status:\s*(.+)$") { $obj.status = $matches[1].Trim() }
    if ($content -match "^pr:\s*\"?(.+?)\"?$") { $obj.pr = $matches[1].Trim() }
    if ($content -match "^commit:\s*\"?(.+?)\"?$") { $obj.commit = $matches[1].Trim() }

    # scope extraction
    $obj.scope = @()
    if ($content -match "(?ms)scope:\s*(?:\n|\r\n)(.*?)(?:\n[^\s-]|\z)") {
        $scopeBlock = $matches[1]
        $scopeLines = $scopeBlock -split "`n" | ForEach-Object { $_.Trim() } | Where-Object { $_ -match '^-' }
        foreach ($line in $scopeLines) {
            $entry = $line -replace '^-\s*', ''
            $entry = $entry.Trim()
            if ($entry) { $obj.scope += $entry }
        }
    }

    return $obj
}

$files = Get-ChildItem -Path design-artifacts/changes -Filter 'CHG-*.yaml' -File -ErrorAction SilentlyContinue
$manifest = [ordered]@{}
foreach ($f in $files) {
    $p = Parse-ChgFile $f.FullName
    if ($p -ne $null -and $p.id) {
        $manifest[$p.id] = $p
    }
}

$json = $manifest | ConvertTo-Json -Depth 6
Set-Content -Path $out -Value $json -Encoding UTF8
Write-Host "Wrote manifest to $out" -ForegroundColor Green
