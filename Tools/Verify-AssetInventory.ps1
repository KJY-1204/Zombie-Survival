# Assets 에셋 인벤토리를 생성하고 현재 상태와 대조하는 검사기
[CmdletBinding()]
param(
    [switch]$Update
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$assetsRoot = Join-Path $repositoryRoot 'Assets'
$inventoryPath = Join-Path $repositoryRoot 'docs/asset-inventory.tsv'

function Get-AssetEntries {
    Get-ChildItem -LiteralPath $assetsRoot -Recurse -File -Force |
        Where-Object { $_.Extension -ne '.meta' } |
        ForEach-Object {
            [PSCustomObject]@{
                Path = "Assets/$($_.FullName.Substring($assetsRoot.Length + 1).Replace('\', '/'))"
                Bytes = $_.Length
            }
        } |
        Sort-Object Path
}

$currentEntries = @(Get-AssetEntries)

if ($Update) {
    $inventoryDirectory = Split-Path -Parent $inventoryPath
    New-Item -ItemType Directory -Path $inventoryDirectory -Force | Out-Null
    $lines = @("Path`tBytes") + @($currentEntries | ForEach-Object { "$($_.Path)`t$($_.Bytes)" })
    [System.IO.File]::WriteAllLines($inventoryPath, $lines, [System.Text.UTF8Encoding]::new($false))
    Write-Output "인벤토리를 갱신했습니다. 에셋 파일 수: $($currentEntries.Count)."
    exit 0
}

if (-not (Test-Path -LiteralPath $inventoryPath)) {
    throw "에셋 인벤토리를 찾을 수 없습니다: $inventoryPath"
}

$expected = @{}
Get-Content -LiteralPath $inventoryPath -Encoding utf8 | Select-Object -Skip 1 | ForEach-Object {
    $columns = $_ -split "`t", 2
    if ($columns.Count -ne 2) {
        throw "잘못된 인벤토리 행입니다: $_"
    }

    $expected[$columns[0]] = [Int64]$columns[1]
}

$actual = @{}
foreach ($entry in $currentEntries) {
    $actual[$entry.Path] = $entry.Bytes
}

$missing = @($expected.Keys | Where-Object { -not $actual.ContainsKey($_) } | Sort-Object)
$added = @($actual.Keys | Where-Object { -not $expected.ContainsKey($_) } | Sort-Object)
$resized = @($expected.Keys | Where-Object { $actual.ContainsKey($_) -and $actual[$_] -ne $expected[$_] } | Sort-Object)

Write-Output "기준 파일 수: $($expected.Count). 현재 파일 수: $($actual.Count)."
Write-Output "누락: $($missing.Count). 추가: $($added.Count). 크기 변경: $($resized.Count)."

foreach ($path in $missing) { Write-Output "MISSING  $path" }
foreach ($path in $added) { Write-Output "ADDED    $path" }
foreach ($path in $resized) { Write-Output "RESIZED  $path" }

if ($missing.Count -gt 0 -or $added.Count -gt 0 -or $resized.Count -gt 0) {
    exit 1
}
