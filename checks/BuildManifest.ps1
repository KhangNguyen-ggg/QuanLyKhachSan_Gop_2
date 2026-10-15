$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$workspace = Split-Path $root -Parent
$design = Join-Path $workspace 'QuanLyKhachSan5Sao\QuanLyKhachSan_Demo_0.3'
$original = Join-Path $workspace 'QuanLyKhachSan_Gop'
$entries = @()
foreach ($file in Get-ChildItem -LiteralPath $design -Recurse -File | Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git|\.vs)\\' -and $_.Extension -in @('.xaml','.cs') }) {
    $relative = $file.FullName.Substring($design.Length + 1)
    $target = 'QuanLyKhachSan_Demo\' + $relative
    $sourceHash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
    $targetHash = (Get-FileHash -LiteralPath (Join-Path $root $target) -Algorithm SHA256).Hash
    $kind = if ($file.Extension -eq '.xaml') { 'ui-xaml' } else { 'ui-code' }
    if ($sourceHash -ne $targetHash) {
        if ($relative -in @('App.xaml','App.xaml.cs','Views\MainWindow.xaml.cs')) {
            $kind = 'login-routing'
        } else {
            if ($file.Extension -eq '.xaml') { throw "UI changed: $relative" }
            if ($relative -notmatch '^Views\\((DatPhong|HoaDon|KhachHang|LichSuLuuTru|LichSuThuTien|LuuTru|PhanBoPhong|TaiKhoan)View\.xaml\.cs|Dialogs\\(DatPhong|HoaDon|KhachHang|LuuTru|TaiKhoan)\\)') { throw "Change outside System/Reception: $relative" }
            $kind = 'ui-binding'
        }
    }
    $entries += [pscustomobject]@{target=$target;kind=$kind;source=('QuanLyKhachSan5Sao\QuanLyKhachSan_Demo_0.3\' + $relative);sourceSha256=$sourceHash;sha256=$targetHash}
}
foreach ($relative in @('Views\LoginThuCongWindow.xaml','Views\LoginThuCongWindow.xaml.cs')) {
    $target = 'QuanLyKhachSan_Demo\' + $relative
    $hash = (Get-FileHash -LiteralPath (Join-Path $root $target) -Algorithm SHA256).Hash
    $entries += [pscustomobject]@{target=$target;kind='manual-login';source='Form đăng nhập nhập tay bổ sung';sourceSha256=$null;sha256=$hash}
}
foreach ($folder in @('BUS_QuanlyKhachSan','DAL_QuanlyKhachSan','ET_QuanLyKhachSan')) {
    foreach ($file in Get-ChildItem -LiteralPath (Join-Path $original $folder) -Recurse -File | Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git|\.vs)\\' }) {
        $relative = $file.FullName.Substring($original.Length + 1)
        $hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
        if ($hash -ne (Get-FileHash -LiteralPath (Join-Path $root $relative) -Algorithm SHA256).Hash) { throw "Business code changed: $relative" }
        $entries += [pscustomobject]@{target=$relative;kind='gop-core';source=('QuanLyKhachSan_Gop\' + $relative);sourceSha256=$hash;sha256=$hash}
    }
}
[pscustomobject]@{interface='QuanLyKhachSan5Sao';business='QuanLyKhachSan_Gop';files=$entries} | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $root 'DOI_CHIEU_GOP_2.json') -Encoding UTF8
& (Join-Path $root 'KIEM_TRA_GOP_2.ps1')
