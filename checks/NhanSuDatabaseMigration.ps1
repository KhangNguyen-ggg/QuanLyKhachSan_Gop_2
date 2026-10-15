# Run from the repository root: powershell -ExecutionPolicy Bypass -File checks/NhanSuDatabaseMigration.ps1
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data
$repo = Split-Path $PSScriptRoot -Parent
$script:checks = 0

function Assert([bool]$ok, [string]$message) {
    if (-not $ok) { throw $message }
    $script:checks++
}
function Execute($connection, [string]$sql) {
    $script:lastSql = $sql
    $command = $connection.CreateCommand()
    $command.CommandTimeout = 30
    $command.CommandText = $sql
    try { [void]$command.ExecuteNonQuery() } finally { $command.Dispose() }
}
function Scalar($connection, [string]$sql) {
    $script:lastSql = $sql
    $command = $connection.CreateCommand()
    $command.CommandTimeout = 30
    $command.CommandText = $sql
    try { $command.ExecuteScalar() } finally { $command.Dispose() }
}
function Reject($connection, [string]$sql, [int]$number = 0) {
    try { Execute $connection $sql } catch {
        $errorSql = $_.Exception.GetBaseException()
        Assert ($errorSql -is [System.Data.SqlClient.SqlException] -and ($number -eq 0 -or $errorSql.Number -eq $number)) ('Unexpected SQL failure: ' + $errorSql.Message)
        return
    }
    throw 'Invalid SQL operation unexpectedly succeeded.'
}

$connectionText = [Environment]::GetEnvironmentVariable('QLKS_CONNECTION')
if ([string]::IsNullOrWhiteSpace($connectionText)) {
    $connectionText = [IO.File]::ReadAllText((Join-Path $repo 'QuanLyKhachSan_Demo/connection.txt')).Trim()
}
$connectionText = $connectionText -replace '(?i)Trust Server Certificate\s*=', 'TrustServerCertificate='
$builder = [System.Data.SqlClient.SqlConnectionStringBuilder]::new($connectionText)
$builder['Initial Catalog'] = 'master'
$builder['Connect Timeout'] = 5
$admin = [System.Data.SqlClient.SqlConnection]::new($builder.ConnectionString)
$test = $null
$database = 'QLKS_NhanSuMigration_' + [Guid]::NewGuid().ToString('N')
$created = $false
try {
    Assert ($database -match '^QLKS_NhanSuMigration_[0-9a-f]{32}$') 'Unsafe test database name.'
    $admin.Open()
    Execute $admin ("CREATE DATABASE [$database]")
    $created = $true
    $builder['Initial Catalog'] = $database
    $test = [System.Data.SqlClient.SqlConnection]::new($builder.ConnectionString)
    $test.Open()

    $baseSchema = [IO.File]::ReadAllText((Join-Path $repo 'Database_quanlykhachsan.sql'))
    $tables = @('PhongBan', 'ChucVu', 'NhanVien', 'CaLamViec', 'LichLamViec', 'ChamCong', 'BangLuong')
    foreach ($table in $tables) {
        $pattern = '(?ms)^CREATE TABLE \[dbo\]\.\[' + [regex]::Escape($table) + '\]\(.*?^\) ON \[PRIMARY\]\s*$'
        $definition = [regex]::Match($baseSchema, $pattern)
        Assert $definition.Success ('Missing original table definition: ' + $table)
        Execute $test $definition.Value
    }
    foreach ($batch in [regex]::Split($baseSchema, '(?im)^\s*GO\s*$')) {
        if ($batch -match 'ALTER TABLE \[dbo\]\.\[(PhongBan|ChucVu|NhanVien|CaLamViec|LichLamViec|ChamCong|BangLuong)\]') {
            Execute $test $batch
        }
    }

    Execute $test @'
INSERT dbo.PhongBan(TenPhongBan,MoTa,TrangThai) VALUES(N'Test department',N'Preserve this row',1);
INSERT dbo.ChucVu(TenChucVu,Chucvu,MoTa,TrangThai) VALUES(N'Test role',9000000,N'Preserve this row',1);
INSERT dbo.NhanVien(HoTen,MaPhongBan,MaChucVu,LuongCoBan,TrangThai)
VALUES(N'Test employee A',1,1,9000000,'DangLam'),(N'Test employee B',1,1,8000000,'DangLam');
INSERT dbo.CaLamViec(TenCa,GioBatDau,GioKetThuc,SoGioLam) VALUES(N'Test shift','22:00','06:00',8);
SET IDENTITY_INSERT dbo.ChamCong ON;
INSERT dbo.ChamCong(MaChamCong,MaLich,MaNhanVien,NgayChamCong,GioVao,GioRa,SoGioLam)
VALUES(41,1001,1,'2026-10-01','2026-10-01T22:00:00','2026-10-02T06:00:00',8),
      (42,1002,2,'2026-10-02','2026-10-02T22:00:00','2026-10-03T06:00:00',8);
SET IDENTITY_INSERT dbo.ChamCong OFF;
SET IDENTITY_INSERT dbo.LichLamViec ON;
INSERT dbo.LichLamViec(MaLich,MaNhanVien,MaCa,NgayLam,MaChamCong,GhiChu)
VALUES(1001,1,1,'2026-10-01',41,N'Existing schedule A'),(1002,2,1,'2026-10-02',42,N'Existing schedule B');
SET IDENTITY_INSERT dbo.LichLamViec OFF;
INSERT dbo.BangLuong(MaNhanVien,Thang,Nam,LuongCoBan,TienCong,TienThuong,TongLuong,ThucLinh)
VALUES(1,10,2026,9000000,9000000,300000,9300000,9300000);
'@

    $snapshot = @{}
    foreach ($table in $tables) {
        $snapshot[$table] = Scalar $test ("SELECT (SELECT * FROM dbo.[$table] ORDER BY 1 FOR JSON PATH, INCLUDE_NULL_VALUES)")
    }
    $migration = [IO.File]::ReadAllText((Join-Path $repo 'CapNhat_NhanSu.sql'))
    Assert ([regex]::Matches($migration, '(?im)^\s*USE\b').Count -eq 1) 'Unexpected database switching in migration.'
    $migration = [regex]::Replace($migration, '(?im)^USE \[QuanLyKhachSan5Sao2\];$', "USE [$database];")

    # Force a late DDL failure to prove that earlier schema changes are rolled back.
    Execute $test 'CREATE VIEW dbo.usp_HienThiBangLuong AS SELECT 1 AS Placeholder;'
    Reject $test $migration
    Assert ((Scalar $test "SELECT OBJECT_ID('dbo.KhenThuong')") -is [DBNull]) 'Failed migration left a new table behind.'
    Assert ((Scalar $test "SELECT OBJECT_ID('dbo.Seq_LichLamViec')") -is [DBNull]) 'Failed migration left a sequence behind.'
    Assert ((Scalar $test "SELECT COUNT(*) FROM sys.foreign_keys WHERE name='FK_LichLamViec_ChamCong'") -eq 1) 'Failed migration removed the old FK.'
    Execute $test 'DROP VIEW dbo.usp_HienThiBangLuong;'

    Execute $test "INSERT dbo.ChamCong(MaLich,MaNhanVien,NgayChamCong) VALUES(999999,1,'2026-10-04');"
    Reject $test $migration 51003
    Execute $test 'DELETE dbo.ChamCong WHERE MaLich=999999;'

    Execute $test $migration
    Execute $test $migration
    foreach ($table in $tables) {
        $after = Scalar $test ("SELECT (SELECT * FROM dbo.[$table] ORDER BY 1 FOR JSON PATH, INCLUDE_NULL_VALUES)")
        Assert ($snapshot[$table] -eq $after) ('Migration changed existing data: ' + $table)
    }
    Assert ((Scalar $test "SELECT COUNT(*) FROM sys.foreign_keys WHERE name='FK_LichLamViec_ChamCong'") -eq 0) 'Reverse FK remains.'
    Assert ((Scalar $test "SELECT COUNT(*) FROM sys.foreign_keys WHERE name='FK_ChamCong_LichLamViec' AND is_disabled=0 AND is_not_trusted=0") -eq 1) 'New FK is missing or untrusted.'

    $newSchedule = [long](Scalar $test "EXEC dbo.usp_ThemLichLamViec @MaNhanVien=1,@MaCa=1,@NgayLam='2026-10-16';")
    Assert ($newSchedule -gt 1002) 'Schedule key was not generated after existing keys.'
    Assert ((Scalar $test "SELECT COUNT(*) FROM dbo.ChamCong WHERE MaLich=$newSchedule") -eq 0) 'A schedule required a pre-created attendance row.'
    Reject $test "EXEC dbo.usp_ThemLichLamViec @MaNhanVien=1,@MaCa=1,@NgayLam='2026-10-16';" 2627
    Reject $test "EXEC dbo.usp_ThemLichLamViec @MaNhanVien=0,@MaCa=1,@NgayLam='2026-10-17';" 51100
    Reject $test "DECLARE @Note nvarchar(max)=REPLICATE(N'a',301); EXEC dbo.usp_ThemLichLamViec 1,1,'2026-10-17',@GhiChu=@Note;" 51101
    Execute $test "INSERT dbo.ChamCong(MaLich,MaNhanVien,NgayChamCong,GioVao,GioRa,SoGioLam) VALUES($newSchedule,1,'2026-10-16','2026-10-16T22:00:00','2026-10-17T06:00:00',8);"
    Reject $test "DELETE dbo.LichLamViec WHERE MaLich=$newSchedule;" 547
    Reject $test "UPDATE dbo.LichLamViec SET MaNhanVien=2 WHERE MaLich=$newSchedule;" 547
    Reject $test "INSERT dbo.ChamCong(MaLich,MaNhanVien,NgayChamCong) VALUES($newSchedule,1,'2026-10-16');" 2627
    Reject $test "INSERT dbo.ChamCong(MaLich,MaNhanVien,NgayChamCong) VALUES(999999,1,'2026-10-18');" 547
    $secondSchedule = [long](Scalar $test "EXEC dbo.usp_ThemLichLamViec 1,1,'2026-10-18';")
    Reject $test "INSERT dbo.ChamCong(MaLich,MaNhanVien,NgayChamCong) VALUES($secondSchedule,2,'2026-10-18');" 547
    Reject $test "INSERT dbo.ChamCong(MaLich,MaNhanVien,NgayChamCong) VALUES($secondSchedule,1,'2026-10-19');" 547
    Reject $test "INSERT dbo.ChamCong(MaLich,MaNhanVien,NgayChamCong,GioVao,GioRa) VALUES($secondSchedule,1,'2026-10-18','2026-10-18T22:00:00','2026-10-18T06:00:00');" 547
    Execute $test "UPDATE dbo.LichLamViec SET GhiChu=N'Updated schedule' WHERE MaLich=$secondSchedule; DELETE dbo.LichLamViec WHERE MaLich=$secondSchedule;"
    Assert ((Scalar $test "SELECT COUNT(*) FROM dbo.LichLamViec WHERE MaLich=$secondSchedule") -eq 0) 'Unreferenced schedule CRUD failed.'

    $award = [long](Scalar $test "INSERT dbo.KhenThuong(MaNhanVien,LyDo,SoTien) OUTPUT INSERTED.MaKhenThuong VALUES(1,N'Good work',200000);")
    Execute $test "UPDATE dbo.KhenThuong SET TrangThai='DaDuyet',SoTien=500000 WHERE MaKhenThuong=$award;"
    Assert ((Scalar $test "SELECT SoTien FROM dbo.KhenThuong WHERE MaKhenThuong=$award AND TrangThai='DaDuyet'") -eq 500000) 'Award insert/update failed.'
    Reject $test "INSERT dbo.KhenThuong(MaNhanVien,LyDo,SoTien) VALUES(1,N'Invalid',-1);" 547
    Reject $test "INSERT dbo.KhenThuong(MaNhanVien,LyDo) VALUES(1,N'   ');" 547
    Reject $test "INSERT dbo.KhenThuong(MaNhanVien,LyDo,HinhThuc) VALUES(1,N'Invalid','Invalid');" 547
    Reject $test "INSERT dbo.KhenThuong(MaNhanVien,LyDo,TrangThai) VALUES(1,N'Invalid','Invalid');" 547
    Reject $test "INSERT dbo.KhenThuong(MaNhanVien,LyDo) VALUES(999999,N'Invalid');" 547
    Assert ((Scalar $test 'SELECT TienThuong FROM dbo.BangLuong WHERE MaNhanVien=1') -eq 300000) 'Award editing unexpectedly changed settled payroll.'
    Assert ((Scalar $test 'EXEC dbo.usp_HienThiBangLuong;') -gt 0) 'Existing DAL payroll procedure contract failed.'
    Execute $test "DELETE dbo.KhenThuong WHERE MaKhenThuong=$award;"
    Assert ((Scalar $test 'SELECT COUNT(*) FROM dbo.KhenThuong') -eq 0) 'Award deletion failed.'

    Execute $test "INSERT dbo.LichLamViec(MaLich,MaNhanVien,MaCa,NgayLam) VALUES(5000,1,1,'2026-10-20');"
    Execute $test $migration
    $afterManualId = [long](Scalar $test "EXEC dbo.usp_ThemLichLamViec 1,1,'2026-10-21';")
    Assert ($afterManualId -gt 5000) 'Rerun did not advance the sequence past manually inserted IDs.'
    $reserved = [long](Scalar $test 'SELECT NEXT VALUE FOR dbo.Seq_LichLamViec;')
    Execute $test $migration
    $afterRerun = [long](Scalar $test "EXEC dbo.usp_ThemLichLamViec 1,1,'2026-10-22';")
    Assert ($afterRerun -gt $reserved) 'Rerun rewound the sequence.'
    Write-Output ("PASS: $script:checks migration/schema/CRUD/rollback checks. Application database unchanged.")
} catch {
    Write-Output ('FAILED_SQL=' + $script:lastSql.Substring(0, [Math]::Min(450, $script:lastSql.Length)))
    throw
} finally {
    if ($null -ne $test) { $test.Dispose() }
    if ($created) {
        Assert ($database -match '^QLKS_NhanSuMigration_[0-9a-f]{32}$') 'Unsafe cleanup target.'
        Execute $admin ("ALTER DATABASE [$database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$database];")
        Write-Output ('Removed isolated test database: ' + $database)
    }
    $admin.Dispose()
}
