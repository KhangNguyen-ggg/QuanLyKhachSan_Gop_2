param([string]$Root = $PSScriptRoot)
$ErrorActionPreference = 'Stop'
$manifest = Get-Content -LiteralPath (Join-Path $Root 'DOI_CHIEU_GOP_2.json') -Raw -Encoding UTF8 | ConvertFrom-Json
foreach ($entry in $manifest.files) {
    $path = Join-Path $Root $entry.target
    if (!(Test-Path -LiteralPath $path -PathType Leaf)) { throw "Missing file: $($entry.target)" }
    if ((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $entry.sha256) { throw "File changed: $($entry.target)" }
    if ($entry.kind -eq 'ui-binding') {
        $code = Get-Content -LiteralPath $path -Raw -Encoding UTF8
        if ($code -notmatch '// Ghép (Gop|Hệ thống):') { throw "Missing merge comment: $($entry.target)" }
    }
}
$uiRoot = Join-Path $Root 'QuanLyKhachSan_Demo'
$xamlFiles = @(Get-ChildItem -LiteralPath $uiRoot -Recurse -File -Filter '*.xaml' | Where-Object { $_.FullName -notmatch '\\(bin|obj|\.vs|\.git)\\' })
$expectedXaml = @($manifest.files | Where-Object { $_.target -like 'QuanLyKhachSan_Demo\*.xaml' })
if ($xamlFiles.Count -ne $expectedXaml.Count) { throw 'The UI has added or removed XAML files.' }
$uiCodeFiles = @(Get-ChildItem -LiteralPath $uiRoot -Recurse -File -Filter '*.cs' | Where-Object { $_.FullName -notmatch '\\(bin|obj|\.vs|\.git)\\' })
$expectedUiCode = @($manifest.files | Where-Object { $_.target -like 'QuanLyKhachSan_Demo\*.cs' })
if ($uiCodeFiles.Count -ne $expectedUiCode.Count) { throw 'The UI has added or removed C# files.' }
foreach ($file in $xamlFiles) {
    $doc = New-Object System.Xml.XmlDocument
    $doc.Load($file.FullName)
}
$core = @($manifest.files | Where-Object { $_.kind -eq 'gop-core' })
$unchanged = @($manifest.files | Where-Object { $_.kind -eq 'ui-code' })
$binding = @($manifest.files | Where-Object { $_.kind -eq 'ui-binding' })
$originalXaml = @($manifest.files | Where-Object { $_.kind -eq 'ui-xaml' })
Write-Output "PASS: $($originalXaml.Count) original XAML files unchanged; $($expectedXaml.Count) total XAML files verified, including manual login and startup routing; $($core.Count) original BUS/DAL/ET files unchanged; $($unchanged.Count) original UI code files unchanged; $($binding.Count) scoped binding files verified."
