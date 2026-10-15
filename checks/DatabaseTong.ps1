# Run: powershell -ExecutionPolicy Bypass -File checks/DatabaseTong.ps1
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data
$repo = Split-Path $PSScriptRoot -Parent
$script:checks = 0
$script:stage = 'Start'
$created = [Collections.Generic.List[string]]::new()

function Assert([bool]$ok, [string]$message) {
    if (-not $ok) { throw $message }
    $script:checks++
}
function Execute($connection, [string]$sql) {
    $command = $connection.CreateCommand()
    $command.CommandTimeout = 60
    $command.CommandText = $sql
    try { [void]$command.ExecuteNonQuery() } finally { $command.Dispose() }
}
function Scalar($connection, [string]$sql) {
    $command = $connection.CreateCommand()
    $command.CommandTimeout = 60
    $command.CommandText = $sql
    try { $command.ExecuteScalar() } finally { $command.Dispose() }
}
function Batches($connection, [string]$sql) {
    foreach ($batch in [regex]::Split($sql, '(?im)^\s*GO\s*$')) {
        if (-not [string]::IsNullOrWhiteSpace($batch)) { Execute $connection $batch }
    }
}
function Run-Total($connection, [string]$database) {
    Assert ($database -match '^QLKS_TongCheck_[0-9a-f]{32}$') 'Unsafe test database name.'
    Batches $connection ($script:total.Replace('QuanLyKhachSan5Sao2', $database))
}
function Snapshot($connection, $tables) {
    $result = @{}
    foreach ($table in $tables) {
        $keys = Scalar $connection ("SELECT STRING_AGG(CONVERT(nvarchar(max),QUOTENAME(c.name)),',') WITHIN GROUP(ORDER BY k.key_ordinal) FROM sys.indexes i JOIN sys.index_columns k ON k.object_id=i.object_id AND k.index_id=i.index_id JOIN sys.columns c ON c.object_id=k.object_id AND c.column_id=k.column_id WHERE i.object_id=OBJECT_ID('dbo.$table') AND i.is_primary_key=1 AND k.key_ordinal>0")
        $result[$table] = Scalar $connection ("SELECT (SELECT * FROM dbo.[$table] ORDER BY $keys FOR JSON PATH,INCLUDE_NULL_VALUES)")
    }
    return ,$result
}
function Assert-Snapshot($connection, $before, $tables) {
    $after = Snapshot $connection $tables
    foreach ($table in $tables) { Assert ($before[$table] -eq $after[$table]) ('Existing rows changed: ' + $table) }
}

$cs = [Environment]::GetEnvironmentVariable('QLKS_CONNECTION')
if ([string]::IsNullOrWhiteSpace($cs)) { $cs = [IO.File]::ReadAllText((Join-Path $repo 'QuanLyKhachSan_Demo/connection.txt')).Trim() }
$cs = $cs -replace '(?i)Trust Server Certificate\s*=', 'TrustServerCertificate='
$builder = [System.Data.SqlClient.SqlConnectionStringBuilder]::new($cs)
$builder['Initial Catalog'] = 'master'
$builder['Connect Timeout'] = 5
$connection = [System.Data.SqlClient.SqlConnection]::new($builder.ConnectionString)
$base = [IO.File]::ReadAllText((Join-Path $repo 'Database_quanlykhachsan.sql'))
$schemaStart = $base.IndexOf('CREATE TABLE [dbo].[BangLuong]')
$schemaEnd = $base.LastIndexOf('USE [master]')
$schema = $base.Substring($schemaStart, $schemaEnd - $schemaStart)
$tables = @([regex]::Matches($schema, '(?im)^CREATE TABLE \[dbo\]\.\[(\w+)\]') | ForEach-Object { $_.Groups[1].Value })
$script:total = [IO.File]::ReadAllText((Join-Path $repo 'Database_QuanLyKhachSan_Tong.sql'))
try {
    Assert ($tables.Count -eq 40) 'Unexpected base table count.'
    Assert ($script:total -notmatch '(?im)^\s*DROP\s+DATABASE\b') 'Destructive database reset found.'
    Assert ($script:total -notmatch '(?im)^\s*:(r|setvar|on\s+error)\b') 'Script requires SQLCMD mode or external files.'
    $connection.Open()

    $script:stage = 'Fresh database creation'
    $fresh = 'QLKS_TongCheck_' + [Guid]::NewGuid().ToString('N')
    $created.Add($fresh)
    Run-Total $connection $fresh
    Assert ((Scalar $connection 'SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped=0') -eq 41) 'Fresh database is missing tables.'
    Assert ((Scalar $connection 'SELECT COUNT(*) FROM sys.procedures WHERE is_ms_shipped=0') -eq 12) 'Fresh database is missing procedures.'
    foreach ($table in $tables) { Assert ((Scalar $connection ("SELECT COUNT(*) FROM dbo.[$table]")) -ge 5) ('Missing sample rows: ' + $table) }
    Assert ((Scalar $connection "SELECT COUNT(*) FROM dbo.XepPhong WHERE TrangThai='DaCheckIn' AND NgayTraThucTe IS NULL") -eq 5) 'Full sample set has no five current stays.'
    Assert ((Scalar $connection "SELECT COUNT(*) FROM sys.foreign_keys WHERE name='FK_ChamCong_LichLamViec' AND is_disabled=0 AND is_not_trusted=0") -eq 1) 'HR foreign key is missing.'

    $script:stage = 'Procedure contracts'
    $calls = @('EXEC dbo.sp_HienThiDatPhong', 'EXEC dbo.sp_HienThiPhanBoPhong', 'EXEC dbo.sp_HienThiKhachLuuTru',
        'EXEC dbo.sp_HienThiLichSuLuuTru', 'EXEC dbo.sp_HienThiHoaDon', 'EXEC dbo.sp_HienThiLichSuThuTien',
        'EXEC dbo.sp_DatPhongChuaLapHoaDon', 'EXEC dbo.sp_KhoanThuDatPhong @MaDatPhong=1',
        'EXEC dbo.sp_ChiTietDatPhong @MaDatPhong=1', 'EXEC dbo.sp_HienThitaiKhoang', 'EXEC dbo.usp_HienThiBangLuong')
    foreach ($call in $calls) { Execute $connection $call; Assert $true ('Procedure failed: ' + $call) }
    $schedule = Scalar $connection "EXEC dbo.usp_ThemLichLamViec @MaNhanVien=1,@MaCa=1,@NgayLam='2099-01-01'"
    Assert ([long]$schedule -gt 0) 'New schedule could not be created.'

    $script:stage = 'Rerun with custom rows'
    Execute $connection "INSERT dbo.PhongBan(TenPhongBan,MoTa,TrangThai) VALUES(N'Custom department preserved',N'Do not reset',1); INSERT dbo.KhenThuong(MaNhanVien,LyDo,SoTien) VALUES(1,N'Custom award preserved',300000);"
    $allTables = $tables + @('KhenThuong')
    $before = Snapshot $connection $allTables
    Run-Total $connection $fresh
    Assert-Snapshot $connection $before $allTables

    $script:stage = 'Legacy database setup'
    $legacy = 'QLKS_TongCheck_' + [Guid]::NewGuid().ToString('N')
    $created.Add($legacy)
    Execute $connection ("USE master; CREATE DATABASE [$legacy];")
    Execute $connection ("USE [$legacy];")
    Batches $connection $schema
    $oldSeed = [IO.File]::ReadAllText((Join-Path $repo 'ThemDuLieu_QuanLyKhachSan.sql'))
    $oldSeed = [regex]::Replace($oldSeed, '(?im)^USE \[QuanLyKhachSan5Sao2\];\s*$', '')
    Batches $connection $oldSeed
    $legacyRows = Snapshot $connection $tables
    $oldPaperType = Scalar $connection "SELECT TYPE_NAME(system_type_id) FROM sys.columns WHERE object_id=OBJECT_ID('dbo.KhachHang') AND name='LoaiGiayTo'"

    $script:stage = 'Atomic rollback on a late upgrade failure'
    Execute $connection 'CREATE VIEW dbo.usp_HienThiBangLuong AS SELECT 1 AS Placeholder;'
    $failed = $false
    try { Run-Total $connection $legacy } catch {
        $sqlError = $_.Exception.GetBaseException()
        Assert ($sqlError -is [System.Data.SqlClient.SqlException] -and $sqlError.Number -eq 2010) ('Unexpected upgrade failure: ' + $sqlError.Message)
        $failed = $true
    }
    Assert $failed 'Incompatible procedure did not fail.'
    Assert ((Scalar $connection "SELECT OBJECT_ID('dbo.KhenThuong')") -is [DBNull]) 'Failed upgrade left new HR objects.'
    Assert ((Scalar $connection "SELECT COUNT(*) FROM sys.foreign_keys WHERE name='FK_LichLamViec_ChamCong'") -eq 1) 'Failed upgrade removed the legacy FK.'
    Assert ((Scalar $connection "SELECT TYPE_NAME(system_type_id) FROM sys.columns WHERE object_id=OBJECT_ID('dbo.KhachHang') AND name='LoaiGiayTo'") -eq $oldPaperType) 'Failed upgrade changed the customer schema.'
    Assert-Snapshot $connection $legacyRows $tables
    Execute $connection 'DROP VIEW dbo.usp_HienThiBangLuong;'

    $script:stage = 'Successful upgrade of populated legacy database'
    Run-Total $connection $legacy
    Assert-Snapshot $connection $legacyRows $tables
    Assert ((Scalar $connection 'SELECT COUNT(*) FROM dbo.DatPhong') -eq 5) 'Legacy database was reseeded.'
    Assert ((Scalar $connection 'SELECT COUNT(*) FROM dbo.KhenThuong') -eq 0) 'Unexpected HR sample insertion.'
    Assert ((Scalar $connection 'SELECT COUNT(*) FROM sys.procedures WHERE is_ms_shipped=0') -eq 12) 'Legacy upgrade is missing procedures.'

    $script:stage = 'Reject partial databases without adding tables'
    $partial = 'QLKS_TongCheck_' + [Guid]::NewGuid().ToString('N')
    $created.Add($partial)
    Execute $connection ("USE master; CREATE DATABASE [$partial];")
    Execute $connection ("USE [$partial]; CREATE TABLE dbo.PhongBan(MaPhongBan int PRIMARY KEY); INSERT dbo.PhongBan VALUES(777);")
    $rejected = $false
    try { Run-Total $connection $partial } catch {
        $sqlError = $_.Exception.GetBaseException()
        Assert ($sqlError -is [System.Data.SqlClient.SqlException] -and $sqlError.Number -eq 51202) ('Unexpected partial schema failure: ' + $sqlError.Message)
        $rejected = $true
    }
    Assert $rejected 'Partial schema was accepted.'
    Assert ((Scalar $connection 'SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped=0') -eq 1) 'Partial database was modified.'
    Assert ((Scalar $connection 'SELECT MaPhongBan FROM dbo.PhongBan') -eq 777) 'Partial database data changed.'
    Write-Output ("PASS: $script:checks combined database/schema/seed/procedure/rerun/rollback checks. Application database unchanged.")
} catch {
    Write-Output ('FAILED_STAGE=' + $script:stage)
    $sqlError = $_.Exception.GetBaseException()
    if ($sqlError -is [System.Data.SqlClient.SqlException]) {
        foreach ($item in $sqlError.Errors) { Write-Output ('SQL_ERROR: number=' + $item.Number + '; procedure=' + $item.Procedure + '; line=' + $item.LineNumber + '; ' + $item.Message) }
    }
    throw
} finally {
    if ($connection.State -eq [Data.ConnectionState]::Open) {
        foreach ($database in $created) {
            if ($database -notmatch '^QLKS_TongCheck_[0-9a-f]{32}$') { throw 'Unsafe cleanup target.' }
            Execute $connection ("USE master; IF DB_ID(N'$database') IS NOT NULL BEGIN ALTER DATABASE [$database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$database]; END;")
            Write-Output ('Removed isolated test database: ' + $database)
        }
    }
    $connection.Dispose()
}
