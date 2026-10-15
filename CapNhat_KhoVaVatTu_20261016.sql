/*
    NANG CAP KHO & VAT TU - QuanLyKhachSan_Gop_2 / main
    Co so doi chieu: Database_QuanLyKhachSan_Tong.sql (15/10/2026),
    commit main ae179531f49cd63d4de76d1e4ae706e27f75d627 (16/10/2026).
    SQL Server 2019+ | DB mac dinh: QuanLyKhachSan5Sao2.

    CHI CHAY SAU KHI DA CHAY Database_QuanLyKhachSan_Tong.sql.
    KHONG chay file Tong them mot lan chi de nang cap kho.
    Script bo sung bang, view, stored procedure; khong DROP TABLE, khong TRUNCATE,
    khong xoa du lieu, khong thay PK cua VatTu (MaLoaiVatTu).
    Co the chay lai. Backup DB truoc khi thuc thi tren du lieu quan trong.

    Khong tu dong tru ton khi Le Tan ghi SuDungDichVu. Viec cap phat vat tu
    do nghiep vu Kho chu dong xac nhan. UI Kho/Vat tu da noi DAL/BUS.
*/

IF DB_ID(N'QuanLyKhachSan5Sao2') IS NULL
    THROW 52000, N'Chua tim thay database QuanLyKhachSan5Sao2. Hay chay file DB Tong truoc.', 1;
GO
USE [QuanLyKhachSan5Sao2];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF DB_NAME() <> N'QuanLyKhachSan5Sao2'
    THROW 52005, N'Dang ket noi sai database. Da dung nang cap.', 1;
IF @@TRANCOUNT <> 0
    THROW 52001, N'Hay chay nang cap trong cua so khong co transaction dang mo.', 1;
IF OBJECT_ID(N'dbo.Kho', N'U') IS NULL OR
   OBJECT_ID(N'dbo.VatTu', N'U') IS NULL OR
   OBJECT_ID(N'dbo.NhaCungCap', N'U') IS NULL OR
   OBJECT_ID(N'dbo.NhanVien', N'U') IS NULL OR
   OBJECT_ID(N'dbo.DichVu', N'U') IS NULL OR
   OBJECT_ID(N'dbo.NhomVatTu', N'U') IS NULL OR
   OBJECT_ID(N'dbo.TieuHaoVatTu', N'U') IS NULL OR
   OBJECT_ID(N'dbo.VatTu_NhaCungCap', N'U') IS NULL OR
   OBJECT_ID(N'dbo.DatPhong', N'U') IS NULL OR
   OBJECT_ID(N'dbo.SuDungDichVu', N'U') IS NULL OR
   OBJECT_ID(N'dbo.Quyen', N'U') IS NULL
    THROW 52002, N'Thieu bang goc can thiet. Khong thuc hien nang cap tren DB chua day du.', 1;
GO

-- 1. BO SUNG CAU TRUC (transaction de khong de lai cac bang tao do dang).
BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @SchemaLock int;
    EXEC @SchemaLock = sys.sp_getapplock
         @Resource=N'QLKS_KHO_UPGRADE', @LockMode=N'Exclusive',
         @LockOwner=N'Transaction', @LockTimeout=15000;
    IF @SchemaLock < 0 THROW 52003, N'Co phien khac dang nang cap Kho.', 1;

    -- Form Kho co o GhiChu, bang cu chua co. Chi them cot nullable.
    IF COL_LENGTH(N'dbo.Kho', N'GhiChu') IS NULL
        ALTER TABLE dbo.Kho ADD GhiChu nvarchar(500) NULL;

    IF OBJECT_ID(N'dbo.TonKho', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.TonKho
        (
            MaKho int NOT NULL,
            MaVatTu int NOT NULL, -- Tham chieu VatTu.MaLoaiVatTu
            SoLuongTon decimal(18,4) NOT NULL CONSTRAINT DF_Kho_TonKho_SoLuong DEFAULT (0),
            CapNhatLuc datetime2(0) NOT NULL CONSTRAINT DF_Kho_TonKho_CapNhat DEFAULT (SYSDATETIME()),
            CONSTRAINT PK_Kho_TonKho PRIMARY KEY (MaKho, MaVatTu),
            CONSTRAINT CK_Kho_TonKho_KhongAm CHECK (SoLuongTon >= 0),
            CONSTRAINT FK_Kho_TonKho_Kho FOREIGN KEY (MaKho) REFERENCES dbo.Kho(MaKho),
            CONSTRAINT FK_Kho_TonKho_VatTu FOREIGN KEY (MaVatTu) REFERENCES dbo.VatTu(MaLoaiVatTu)
        );
    END;

    -- Phieu nhap / xuat / dieu chinh: cung mot bang de dung chung giao dien.
    IF OBJECT_ID(N'dbo.PhieuNhapXuat', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.PhieuNhapXuat
        (
            MaPhieu bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kho_PhieuNhapXuat PRIMARY KEY,
            LoaiPhieu varchar(20) NOT NULL,
            MaKho int NOT NULL,
            MaNhaCungCap int NULL,
            MaNhanVien int NOT NULL,
            NgayPhieu datetime2(0) NOT NULL CONSTRAINT DF_Kho_Phieu_Ngay DEFAULT (SYSDATETIME()),
            MucDich nvarchar(150) NULL,
            GhiChu nvarchar(500) NULL,
            TrangThai varchar(20) NOT NULL CONSTRAINT DF_Kho_Phieu_TrangThai DEFAULT ('MOI'),
            MaNguoiXacNhan int NULL,
            NgayXacNhan datetime2(0) NULL,
            CONSTRAINT CK_Kho_Phieu_Loai CHECK (LoaiPhieu IN ('NHAP', 'XUAT', 'DIEU_CHINH_TANG', 'DIEU_CHINH_GIAM')),
            CONSTRAINT CK_Kho_Phieu_TrangThai CHECK (TrangThai IN ('MOI', 'DA_XAC_NHAN', 'DA_HUY')),
            CONSTRAINT FK_Kho_Phieu_Kho FOREIGN KEY (MaKho) REFERENCES dbo.Kho(MaKho),
            CONSTRAINT FK_Kho_Phieu_NCC FOREIGN KEY (MaNhaCungCap) REFERENCES dbo.NhaCungCap(MaNhaCungCap),
            CONSTRAINT FK_Kho_Phieu_NV FOREIGN KEY (MaNhanVien) REFERENCES dbo.NhanVien(MaNhanVien),
            CONSTRAINT FK_Kho_Phieu_NVXN FOREIGN KEY (MaNguoiXacNhan) REFERENCES dbo.NhanVien(MaNhanVien)
        );
    END;

    IF OBJECT_ID(N'dbo.ChiTietPhieuKho', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.ChiTietPhieuKho
        (
            MaChiTietPhieuKho bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kho_ChiTietPhieu PRIMARY KEY,
            MaPhieu bigint NOT NULL,
            MaVatTu int NOT NULL,
            SoLuong decimal(18,4) NOT NULL,
            DonGia decimal(18,2) NULL,
            GhiChu nvarchar(300) NULL,
            CONSTRAINT UQ_Kho_ChiTietPhieu UNIQUE (MaPhieu, MaVatTu),
            CONSTRAINT CK_Kho_ChiTietPhieu_SoLuong CHECK (SoLuong > 0),
            CONSTRAINT CK_Kho_ChiTietPhieu_DonGia CHECK (DonGia IS NULL OR DonGia >= 0),
            CONSTRAINT FK_Kho_ChiTietPhieu_Phieu FOREIGN KEY (MaPhieu) REFERENCES dbo.PhieuNhapXuat(MaPhieu),
            CONSTRAINT FK_Kho_ChiTietPhieu_VatTu FOREIGN KEY (MaVatTu) REFERENCES dbo.VatTu(MaLoaiVatTu)
        );
    END;

    IF OBJECT_ID(N'dbo.PhieuChuyenKho', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.PhieuChuyenKho
        (
            MaChuyenKho bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kho_PhieuChuyen PRIMARY KEY,
            MaKhoXuat int NOT NULL,
            MaKhoNhap int NOT NULL,
            MaNhanVien int NOT NULL,
            NgayChuyen datetime2(0) NOT NULL CONSTRAINT DF_Kho_Chuyen_Ngay DEFAULT (SYSDATETIME()),
            TrangThai varchar(20) NOT NULL CONSTRAINT DF_Kho_Chuyen_TrangThai DEFAULT ('MOI'),
            GhiChu nvarchar(500) NULL,
            NgayXacNhan datetime2(0) NULL,
            CONSTRAINT CK_Kho_Chuyen_KhacKho CHECK (MaKhoXuat <> MaKhoNhap),
            CONSTRAINT CK_Kho_Chuyen_TrangThai CHECK (TrangThai IN ('MOI', 'DA_XAC_NHAN', 'DA_HUY')),
            CONSTRAINT FK_Kho_Chuyen_KhoXuat FOREIGN KEY (MaKhoXuat) REFERENCES dbo.Kho(MaKho),
            CONSTRAINT FK_Kho_Chuyen_KhoNhap FOREIGN KEY (MaKhoNhap) REFERENCES dbo.Kho(MaKho),
            CONSTRAINT FK_Kho_Chuyen_NV FOREIGN KEY (MaNhanVien) REFERENCES dbo.NhanVien(MaNhanVien)
        );
    END;

    IF OBJECT_ID(N'dbo.ChiTietChuyenKho', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.ChiTietChuyenKho
        (
            MaChiTietChuyenKho bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kho_ChiTietChuyen PRIMARY KEY,
            MaChuyenKho bigint NOT NULL,
            MaVatTu int NOT NULL,
            SoLuong decimal(18,4) NOT NULL,
            CONSTRAINT UQ_Kho_ChiTietChuyen UNIQUE (MaChuyenKho, MaVatTu),
            CONSTRAINT CK_Kho_ChiTietChuyen_SoLuong CHECK (SoLuong > 0),
            CONSTRAINT FK_Kho_ChiTietChuyen_Phieu FOREIGN KEY (MaChuyenKho) REFERENCES dbo.PhieuChuyenKho(MaChuyenKho),
            CONSTRAINT FK_Kho_ChiTietChuyen_VatTu FOREIGN KEY (MaVatTu) REFERENCES dbo.VatTu(MaLoaiVatTu)
        );
    END;

    IF OBJECT_ID(N'dbo.PhieuCapVatTuNoiBo', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.PhieuCapVatTuNoiBo
        (
            MaPhieuCap bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kho_PhieuCap PRIMARY KEY,
            MaKho int NOT NULL,
            MaNhanVien int NOT NULL,
            MucDich varchar(20) NOT NULL,
            MaDatPhong bigint NULL,
            MaSuDungDichVu bigint NULL,
            NgayCap datetime2(0) NOT NULL CONSTRAINT DF_Kho_Cap_Ngay DEFAULT (SYSDATETIME()),
            GhiChu nvarchar(500) NULL,
            TrangThai varchar(20) NOT NULL CONSTRAINT DF_Kho_Cap_TrangThai DEFAULT ('MOI'),
            NgayXacNhan datetime2(0) NULL,
            CONSTRAINT CK_Kho_Cap_MucDich CHECK (MucDich IN ('DON_PHONG', 'DICH_VU', 'KHAC')),
            CONSTRAINT CK_Kho_Cap_ThamChieu CHECK (MaDatPhong IS NULL OR MaSuDungDichVu IS NULL),
            CONSTRAINT CK_Kho_Cap_TrangThai CHECK (TrangThai IN ('MOI', 'DA_XAC_NHAN', 'DA_HUY')),
            CONSTRAINT FK_Kho_Cap_Kho FOREIGN KEY (MaKho) REFERENCES dbo.Kho(MaKho),
            CONSTRAINT FK_Kho_Cap_NV FOREIGN KEY (MaNhanVien) REFERENCES dbo.NhanVien(MaNhanVien),
            CONSTRAINT FK_Kho_Cap_DatPhong FOREIGN KEY (MaDatPhong) REFERENCES dbo.DatPhong(MaDatPhong),
            CONSTRAINT FK_Kho_Cap_SDDV FOREIGN KEY (MaSuDungDichVu) REFERENCES dbo.SuDungDichVu(MaSuDungDichVu)
        );
    END;

    IF OBJECT_ID(N'dbo.ChiTietCapVatTuNoiBo', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.ChiTietCapVatTuNoiBo
        (
            MaChiTietCap bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kho_ChiTietCap PRIMARY KEY,
            MaPhieuCap bigint NOT NULL,
            MaVatTu int NOT NULL,
            SoLuong decimal(18,4) NOT NULL,
            CONSTRAINT UQ_Kho_ChiTietCap UNIQUE (MaPhieuCap, MaVatTu),
            CONSTRAINT CK_Kho_ChiTietCap_SoLuong CHECK (SoLuong > 0),
            CONSTRAINT FK_Kho_ChiTietCap_Phieu FOREIGN KEY (MaPhieuCap) REFERENCES dbo.PhieuCapVatTuNoiBo(MaPhieuCap),
            CONSTRAINT FK_Kho_ChiTietCap_VatTu FOREIGN KEY (MaVatTu) REFERENCES dbo.VatTu(MaLoaiVatTu)
        );
    END;

    IF OBJECT_ID(N'dbo.PhieuKiemKe', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.PhieuKiemKe
        (
            MaKiemKe bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kho_PhieuKiemKe PRIMARY KEY,
            MaKho int NOT NULL,
            MaNhanVien int NOT NULL,
            NgayKiemKe datetime2(0) NOT NULL CONSTRAINT DF_Kho_KiemKe_Ngay DEFAULT (SYSDATETIME()),
            LyDo nvarchar(500) NULL,
            TrangThai varchar(20) NOT NULL CONSTRAINT DF_Kho_KiemKe_TrangThai DEFAULT ('MOI'),
            NgayXacNhan datetime2(0) NULL,
            CONSTRAINT CK_Kho_KiemKe_TrangThai CHECK (TrangThai IN ('MOI', 'DA_XAC_NHAN', 'DA_HUY')),
            CONSTRAINT FK_Kho_KiemKe_Kho FOREIGN KEY (MaKho) REFERENCES dbo.Kho(MaKho),
            CONSTRAINT FK_Kho_KiemKe_NV FOREIGN KEY (MaNhanVien) REFERENCES dbo.NhanVien(MaNhanVien)
        );
    END;

    IF OBJECT_ID(N'dbo.ChiTietKiemKe', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.ChiTietKiemKe
        (
            MaChiTietKiemKe bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kho_ChiTietKiemKe PRIMARY KEY,
            MaKiemKe bigint NOT NULL,
            MaVatTu int NOT NULL,
            TonSoSach decimal(18,4) NOT NULL,
            TonThucTe decimal(18,4) NOT NULL,
            GhiChu nvarchar(300) NULL,
            CONSTRAINT UQ_Kho_ChiTietKiemKe UNIQUE (MaKiemKe, MaVatTu),
            CONSTRAINT CK_Kho_ChiTietKiemKe_So CHECK (TonSoSach >= 0 AND TonThucTe >= 0),
            CONSTRAINT FK_Kho_ChiTietKiemKe_Phieu FOREIGN KEY (MaKiemKe) REFERENCES dbo.PhieuKiemKe(MaKiemKe),
            CONSTRAINT FK_Kho_ChiTietKiemKe_VatTu FOREIGN KEY (MaVatTu) REFERENCES dbo.VatTu(MaLoaiVatTu)
        );
    END;

    -- Nhat ky la bang chi them, khong sua/xoa phieu da xac nhan.
    IF OBJECT_ID(N'dbo.NhatKyKho', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.NhatKyKho
        (
            MaNhatKyKho bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kho_NhatKy PRIMARY KEY,
            MaKho int NOT NULL,
            MaVatTu int NOT NULL,
            BienDong decimal(18,4) NOT NULL,
            TonTruoc decimal(18,4) NOT NULL,
            TonSau decimal(18,4) NOT NULL,
            LoaiChungTu varchar(20) NOT NULL,
            MaChungTu bigint NOT NULL,
            MaNhanVien int NULL,
            ThoiGian datetime2(0) NOT NULL CONSTRAINT DF_Kho_NhatKy_ThoiGian DEFAULT (SYSDATETIME()),
            GhiChu nvarchar(500) NULL,
            CONSTRAINT CK_Kho_NhatKy_BienDong CHECK (BienDong <> 0 AND TonTruoc >= 0 AND TonSau >= 0 AND TonSau = TonTruoc + BienDong),
            CONSTRAINT CK_Kho_NhatKy_Loai CHECK (LoaiChungTu IN ('PHIEU_KHO', 'CHUYEN_KHO', 'CAP_NOI_BO', 'KIEM_KE')),
            CONSTRAINT FK_Kho_NhatKy_Kho FOREIGN KEY (MaKho) REFERENCES dbo.Kho(MaKho),
            CONSTRAINT FK_Kho_NhatKy_VatTu FOREIGN KEY (MaVatTu) REFERENCES dbo.VatTu(MaLoaiVatTu),
            CONSTRAINT FK_Kho_NhatKy_NV FOREIGN KEY (MaNhanVien) REFERENCES dbo.NhanVien(MaNhanVien)
        );
    END;

    -- Nếu tên bảng tồn tại nhưng thiếu trường chủ chốt, phải dừng thay vì ghi đè.
    IF COL_LENGTH(N'dbo.TonKho', N'SoLuongTon') IS NULL OR
       COL_LENGTH(N'dbo.PhieuNhapXuat', N'LoaiPhieu') IS NULL OR
       COL_LENGTH(N'dbo.ChiTietPhieuKho', N'MaVatTu') IS NULL OR
       COL_LENGTH(N'dbo.NhatKyKho', N'BienDong') IS NULL
        THROW 52004, N'Phat hien bang Kho da ton tai voi schema khac. Hay doi chieu truoc khi nang cap.', 1;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.NhatKyKho') AND name=N'IX_Kho_NhatKy_VatTu')
        CREATE INDEX IX_Kho_NhatKy_VatTu ON dbo.NhatKyKho(MaKho, MaVatTu, ThoiGian DESC);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.NhatKyKho') AND name=N'IX_Kho_NhatKy_ChungTu')
        CREATE INDEX IX_Kho_NhatKy_ChungTu ON dbo.NhatKyKho(LoaiChungTu, MaChungTu);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.PhieuNhapXuat') AND name=N'IX_Kho_Phieu_Ngay')
        CREATE INDEX IX_Kho_Phieu_Ngay ON dbo.PhieuNhapXuat(NgayPhieu DESC, TrangThai);

    -- Danh muc quyen, CHUA tu gan vao VaiTro_Quyen hoac thay DemoSession.cs.
    -- MaChucNang la ma dung rieng cho Kho, khong ghi de quyen cu.
    INSERT INTO dbo.Quyen(MaChucNang, TenQuyen, MoTa, TrangThai)
    SELECT m.MaChucNang, m.TenQuyen, m.MoTa, 1
    FROM (VALUES
        (N'KHO_XEM', N'Xem Kho va Vat tu', N'Xem danh muc kho, vat tu, ton kho'),
        (N'KHO_DANH_MUC', N'Quan ly danh muc Kho', N'Them sua danh muc kho, vat tu, nhom, nha cung cap'),
        (N'KHO_NHAP_XUAT', N'Nhap xuat kho', N'Lap va xac nhan phieu nhap xuat'),
        (N'KHO_DIEU_CHINH', N'Dieu chinh ton kho', N'Dieu chinh tang giam co chung tu'),
        (N'KHO_CHUYEN', N'Chuyen kho', N'Chuyen vat tu giua cac kho'),
        (N'KHO_KIEM_KE', N'Kiem ke kho', N'Ghi nhan va xac nhan kiem ke'),
        (N'KHO_CAP_NOI_BO', N'Cap phat vat tu noi bo', N'Cap cho don phong / dich vu')
    ) AS m(MaChucNang, TenQuyen, MoTa)
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Quyen q WITH (UPDLOCK, HOLDLOCK) WHERE q.MaChucNang=m.MaChucNang);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

-- 2. CAC VIEW PHUC VU HIEN THI, KHONG SUA DU LIEU GOC.
CREATE OR ALTER VIEW dbo.vw_Kho_TonKho
AS
-- Them: vat tu chua co so du van hien 0 o kho mac dinh de nhap ton dau ky.
WITH ViTri AS (
    SELECT MaKho, MaVatTu FROM dbo.TonKho
    UNION
    SELECT MaKhoMacDinh, MaLoaiVatTu FROM dbo.VatTu WHERE MaKhoMacDinh IS NOT NULL
)
SELECT p.MaKho, k.TenKho AS Kho, p.MaVatTu, v.TenVatTu AS VatTu,
       n.TenNhomVatTu AS Nhom, v.DonViTinh AS DonVi,
       ISNULL(t.SoLuongTon, CONVERT(decimal(18,4),0)) AS Ton, v.SoLuongToiThieu AS ToiThieu,
       CASE WHEN ISNULL(t.SoLuongTon,0) < v.SoLuongToiThieu THEN N'Dưới tối thiểu'
            ELSE N'Đủ tồn' END AS CanhBao,
       t.CapNhatLuc
FROM ViTri p
LEFT JOIN dbo.TonKho t ON t.MaKho=p.MaKho AND t.MaVatTu=p.MaVatTu
JOIN dbo.Kho k ON k.MaKho = p.MaKho
JOIN dbo.VatTu v ON v.MaLoaiVatTu = p.MaVatTu
JOIN dbo.NhomVatTu n ON n.MaNhomVatTu = v.MaNhomVatTu;
GO

CREATE OR ALTER VIEW dbo.vw_Kho_CanhBaoTon
AS
SELECT k.MaKho, k.TenKho, v.MaLoaiVatTu AS MaVatTu,
       v.TenVatTu, v.DonViTinh, COALESCE(t.SoLuongTon, 0) AS Ton,
       v.SoLuongToiThieu AS ToiThieu
FROM dbo.VatTu v
JOIN dbo.Kho k ON k.MaKho = v.MaKhoMacDinh
LEFT JOIN dbo.TonKho t ON t.MaKho = k.MaKho AND t.MaVatTu = v.MaLoaiVatTu
WHERE v.TrangThai = 1 AND k.TrangThai = 1 AND COALESCE(t.SoLuongTon, 0) < v.SoLuongToiThieu;
GO

CREATE OR ALTER VIEW dbo.vw_Kho_PhieuNhapXuat
AS
SELECT p.MaPhieu, p.LoaiPhieu,
       CASE p.LoaiPhieu
           WHEN 'NHAP' THEN N'Nhap kho'
           WHEN 'XUAT' THEN N'Xuat kho'
           WHEN 'DIEU_CHINH_TANG' THEN N'Dieu chinh tang'
           ELSE N'Dieu chinh giam' END AS Loai,
       p.MaKho, k.TenKho AS Kho,
       p.MaNhaCungCap, ncc.TenNhaCungCap AS NhaCungCap,
       p.MaNhanVien, nv.HoTen AS NhanVien,
       p.NgayPhieu AS Ngay, p.MucDich, p.GhiChu,
       p.TrangThai, p.NgayXacNhan,
       COALESCE(x.SoDong, 0) AS SoDong,
       COALESCE(x.TongSoLuong, CONVERT(decimal(38,4), 0)) AS TongSoLuong,
       COALESCE(x.TongTien, CONVERT(decimal(38,2), 0)) AS TongTien
FROM dbo.PhieuNhapXuat p
JOIN dbo.Kho k ON k.MaKho = p.MaKho
JOIN dbo.NhanVien nv ON nv.MaNhanVien = p.MaNhanVien
LEFT JOIN dbo.NhaCungCap ncc ON ncc.MaNhaCungCap = p.MaNhaCungCap
OUTER APPLY (
    SELECT COUNT_BIG(*) AS SoDong, SUM(ct.SoLuong) AS TongSoLuong,
           SUM(CONVERT(decimal(28,4),ct.SoLuong) * CONVERT(decimal(28,2),ISNULL(ct.DonGia, 0))) AS TongTien
    FROM dbo.ChiTietPhieuKho ct WHERE ct.MaPhieu = p.MaPhieu
) x;
GO

CREATE OR ALTER VIEW dbo.vw_Kho_ChiTietPhieuKho
AS
SELECT ct.MaChiTietPhieuKho, ct.MaPhieu, ct.MaVatTu,
       v.TenVatTu, v.DonViTinh, ct.SoLuong, ct.DonGia,
       CONVERT(decimal(38,2), ct.SoLuong * ISNULL(ct.DonGia, 0)) AS ThanhTien,
       ct.GhiChu
FROM dbo.ChiTietPhieuKho ct
JOIN dbo.VatTu v ON v.MaLoaiVatTu = ct.MaVatTu;
GO

CREATE OR ALTER VIEW dbo.vw_Kho_CapVatTuNoiBo
AS
SELECT p.MaPhieuCap, p.MaKho, k.TenKho AS Kho, p.MaNhanVien,
       nv.HoTen AS NhanVien, p.MucDich, p.MaDatPhong, p.MaSuDungDichVu,
       p.NgayCap, p.GhiChu, p.TrangThai, p.NgayXacNhan,
       COALESCE(ct.SoDong, 0) AS SoDong,
       COALESCE(ct.TongSoLuong, CONVERT(decimal(38,4), 0)) AS TongSoLuong
FROM dbo.PhieuCapVatTuNoiBo p
JOIN dbo.Kho k ON k.MaKho=p.MaKho
JOIN dbo.NhanVien nv ON nv.MaNhanVien=p.MaNhanVien
OUTER APPLY (
    SELECT COUNT_BIG(*) AS SoDong, SUM(SoLuong) AS TongSoLuong
    FROM dbo.ChiTietCapVatTuNoiBo c WHERE c.MaPhieuCap=p.MaPhieuCap
) ct;
GO

-- 3. HAM DUNG CHUNG THAY DOI TON + GHI NHAT KY.
-- KHONG goi ham nay truc tiep tu View. Chi goi thong qua cac usp_Kho_* nghiep vu.
CREATE OR ALTER PROCEDURE dbo.usp_Kho_BienDong
    @MaKho int,
    @MaVatTu int,
    @BienDong decimal(18,4),
    @LoaiChungTu varchar(20),
    @MaChungTu bigint,
    @MaNhanVien int = NULL,
    @GhiChu nvarchar(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @@TRANCOUNT = 0
        THROW 52100, N'Bien dong kho phai nam trong transaction cua nghiep vu.', 1;
    IF @BienDong IS NULL THROW 52105, N'Biến động không được để trống.', 1;
    IF @BienDong = 0 RETURN;

    -- Lock theo transaction: tranh 2 nguoi nhap/xuat cung luc lam sai ton.
    DECLARE @KetQuaLock int;
    EXEC @KetQuaLock = sys.sp_getapplock
        @Resource = N'QLKS_KHO_TON_KHO', @LockMode = N'Exclusive',
        @LockOwner = N'Transaction', @LockTimeout = 15000;
    IF @KetQuaLock < 0 THROW 52101, N'Khong khoa duoc giao dich kho.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Kho WHERE MaKho=@MaKho AND TrangThai=1)
        THROW 52102, N'Kho không tồn tại hoặc đã ngừng hoạt động.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.VatTu WHERE MaLoaiVatTu=@MaVatTu AND TrangThai=1)
        THROW 52103, N'Vật tư không tồn tại hoặc đã ngừng hoạt động.', 1;
    IF @MaNhanVien IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.NhanVien WHERE MaNhanVien=@MaNhanVien AND TrangThai='DangLam')
        THROW 52106, N'Nhân viên ghi nhận phải còn đang làm việc.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.TonKho WITH (UPDLOCK, HOLDLOCK)
                   WHERE MaKho=@MaKho AND MaVatTu=@MaVatTu)
        INSERT dbo.TonKho(MaKho, MaVatTu) VALUES (@MaKho, @MaVatTu);

    DECLARE @TonTruoc decimal(18,4), @TonSau decimal(18,4);
    SELECT @TonTruoc=SoLuongTon FROM dbo.TonKho WITH (UPDLOCK, HOLDLOCK)
    WHERE MaKho=@MaKho AND MaVatTu=@MaVatTu;

    IF @TonTruoc + @BienDong < 0
        THROW 52104, N'Không đủ tồn kho để xuất hoặc điều chỉnh giảm.', 1;
    IF @TonTruoc + @BienDong >= 100000000000000
        THROW 52107, N'Số tồn vượt giới hạn 14 chữ số nguyên.', 1;

    SET @TonSau=@TonTruoc+@BienDong;
    UPDATE dbo.TonKho
       SET SoLuongTon=@TonSau, CapNhatLuc=SYSDATETIME()
     WHERE MaKho=@MaKho AND MaVatTu=@MaVatTu;

    INSERT dbo.NhatKyKho(MaKho, MaVatTu, BienDong, TonTruoc, TonSau,
                         LoaiChungTu, MaChungTu, MaNhanVien, GhiChu)
    VALUES (@MaKho, @MaVatTu, @BienDong, @TonTruoc, @TonSau,
            @LoaiChungTu, @MaChungTu, @MaNhanVien, @GhiChu);
END;
GO

-- 4. XAC NHAN PHIEU NHAP / XUAT / DIEU CHINH.
-- Chinh sua phieu MOI va chi tiet truoc; chi xac nhan 1 lan.
CREATE OR ALTER PROCEDURE dbo.usp_Kho_XacNhanPhieuNhapXuat
    @MaPhieu bigint,
    @MaNguoiXacNhan int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @Lock int;
        EXEC @Lock=sys.sp_getapplock @Resource=N'QLKS_KHO_TON_KHO',
             @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
        IF @Lock < 0 THROW 52110, N'Khong khoa duoc kho.', 1;

        DECLARE @Loai varchar(20), @TrangThai varchar(20), @MaKho int, @MaNhanVien int;
        SELECT @Loai=LoaiPhieu, @TrangThai=TrangThai, @MaKho=MaKho, @MaNhanVien=MaNhanVien
          FROM dbo.PhieuNhapXuat WITH (UPDLOCK, HOLDLOCK) WHERE MaPhieu=@MaPhieu;
        IF @TrangThai IS NULL THROW 52111, N'Phieu khong ton tai.', 1;
        IF @TrangThai <> 'MOI' THROW 52112, N'Phieu da xu ly, khong the xac nhan lai.', 1;
        -- Them: danh muc co the ngung hoat dong sau khi lap phieu Moi.
        IF EXISTS (SELECT 1 FROM dbo.PhieuNhapXuat p JOIN dbo.NhaCungCap n ON n.MaNhaCungCap=p.MaNhaCungCap WHERE p.MaPhieu=@MaPhieu AND n.TrangThai=0)
            THROW 52116, N'Nhà cung cấp đã ngừng hoạt động.', 1;
        IF @MaNguoiXacNhan IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.NhanVien WHERE MaNhanVien=@MaNguoiXacNhan AND TrangThai='DangLam')
            THROW 52117, N'Nhân viên xác nhận không còn đang làm việc.', 1;
        SET @MaNhanVien=COALESCE(@MaNguoiXacNhan,@MaNhanVien);
        IF NOT EXISTS(SELECT 1 FROM dbo.ChiTietPhieuKho WHERE MaPhieu=@MaPhieu)
            THROW 52113, N'Phieu chua co chi tiet vat tu.', 1;

        DECLARE @HeSo int = CASE WHEN @Loai IN ('NHAP','DIEU_CHINH_TANG') THEN 1 ELSE -1 END;
        DECLARE @MaVatTu int, @SoLuong decimal(18,4);
        DECLARE phieu_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT MaVatTu, SoLuong FROM dbo.ChiTietPhieuKho
             WHERE MaPhieu=@MaPhieu ORDER BY MaVatTu;
        OPEN phieu_cursor;
        FETCH NEXT FROM phieu_cursor INTO @MaVatTu, @SoLuong;
        WHILE @@FETCH_STATUS=0
        BEGIN
            DECLARE @Delta decimal(18,4) = @SoLuong * @HeSo;
            EXEC dbo.usp_Kho_BienDong @MaKho=@MaKho, @MaVatTu=@MaVatTu,
                 @BienDong=@Delta, @LoaiChungTu='PHIEU_KHO',
                 @MaChungTu=@MaPhieu, @MaNhanVien=@MaNhanVien;
            FETCH NEXT FROM phieu_cursor INTO @MaVatTu, @SoLuong;
        END;
        CLOSE phieu_cursor;
        DEALLOCATE phieu_cursor;

        UPDATE dbo.PhieuNhapXuat
           SET TrangThai='DA_XAC_NHAN', MaNguoiXacNhan=@MaNguoiXacNhan,
               NgayXacNhan=SYSDATETIME()
         WHERE MaPhieu=@MaPhieu;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Kho_HuyPhieuNhapXuat
    @MaPhieu bigint
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    UPDATE dbo.PhieuNhapXuat
       SET TrangThai='DA_HUY'
     WHERE MaPhieu=@MaPhieu AND TrangThai='MOI';
    IF @@ROWCOUNT <> 1
        THROW 52115, N'Chi duoc huy phieu chua xac nhan; phieu da ghi ton phai lap chung tu dieu chinh.', 1;
END;
GO

-- Goi nhanh cho UI Nhap kho / Xuat kho: tao phieu, them 1 dong, xac nhan atomically.
CREATE OR ALTER PROCEDURE dbo.usp_Kho_NhapXuatNhanh
    @LoaiPhieu varchar(20),
    @MaKho int,
    @MaVatTu int,
    @SoLuong decimal(18,4),
    @MaNhanVien int,
    @MaNhaCungCap int = NULL,
    @DonGia decimal(18,2) = NULL,
    @MucDich nvarchar(150) = NULL,
    @GhiChu nvarchar(500) = NULL,
    @MaPhieu bigint = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @SoLuong IS NULL OR @SoLuong <= 0
        THROW 52120, N'So luong nhap/xuat phai lon hon 0.', 1;
    IF @LoaiPhieu NOT IN ('NHAP','XUAT','DIEU_CHINH_TANG','DIEU_CHINH_GIAM') OR @LoaiPhieu IS NULL
        THROW 52121, N'Loai phieu khong hop le.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        INSERT dbo.PhieuNhapXuat(LoaiPhieu, MaKho, MaNhaCungCap, MaNhanVien, MucDich, GhiChu)
        VALUES (@LoaiPhieu, @MaKho, @MaNhaCungCap, @MaNhanVien, @MucDich, @GhiChu);
        SET @MaPhieu=SCOPE_IDENTITY();
        INSERT dbo.ChiTietPhieuKho(MaPhieu, MaVatTu, SoLuong, DonGia)
        VALUES (@MaPhieu, @MaVatTu, @SoLuong, @DonGia);
        EXEC dbo.usp_Kho_XacNhanPhieuNhapXuat @MaPhieu=@MaPhieu, @MaNguoiXacNhan=@MaNhanVien;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- Dieu chinh bang SO TON THUC TE (o txtTonKiemKe cua giao dien Kho).
CREATE OR ALTER PROCEDURE dbo.usp_Kho_DieuChinhTon
    @MaKho int,
    @MaVatTu int,
    @TonThucTe decimal(18,4),
    @MaNhanVien int,
    @LyDo nvarchar(500),
    @MaPhieu bigint = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @TonThucTe IS NULL OR @TonThucTe < 0
        THROW 52130, N'Ton thuc te phai lon hon hoac bang 0.', 1;
    IF NULLIF(LTRIM(RTRIM(@LyDo)), N'') IS NULL
        THROW 52131, N'Dieu chinh ton phai co ly do.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @Lock int;
        EXEC @Lock=sys.sp_getapplock @Resource=N'QLKS_KHO_TON_KHO',
             @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
        IF @Lock < 0 THROW 52132, N'Khong khoa duoc ton kho.', 1;
        -- Them: van kiem tra danh muc khi chenh lech bang 0.
        IF NOT EXISTS(SELECT 1 FROM dbo.Kho WHERE MaKho=@MaKho AND TrangThai=1)
           OR NOT EXISTS(SELECT 1 FROM dbo.VatTu WHERE MaLoaiVatTu=@MaVatTu AND TrangThai=1)
           OR NOT EXISTS(SELECT 1 FROM dbo.NhanVien WHERE MaNhanVien=@MaNhanVien AND TrangThai='DangLam')
            THROW 52133, N'Kho, vật tư hoặc nhân viên điều chỉnh không còn hoạt động.', 1;
        DECLARE @TonHienTai decimal(18,4)=0, @ChenhLech decimal(18,4), @Loai varchar(20);
        SELECT @TonHienTai=SoLuongTon FROM dbo.TonKho WITH (UPDLOCK, HOLDLOCK)
         WHERE MaKho=@MaKho AND MaVatTu=@MaVatTu;
        SET @ChenhLech=@TonThucTe-@TonHienTai;
        IF @ChenhLech <> 0
        BEGIN
            SET @Loai=CASE WHEN @ChenhLech>0 THEN 'DIEU_CHINH_TANG' ELSE 'DIEU_CHINH_GIAM' END;
            -- Sua: EXEC chi nhan bien/gia tri, khong nhan truc tiep ABS(...).
            DECLARE @SoLuongChenhLech decimal(18,4)=ABS(@ChenhLech);
            EXEC dbo.usp_Kho_NhapXuatNhanh
                 @LoaiPhieu=@Loai, @MaKho=@MaKho, @MaVatTu=@MaVatTu,
                 @SoLuong=@SoLuongChenhLech, @MaNhanVien=@MaNhanVien,
                 @GhiChu=@LyDo, @MaPhieu=@MaPhieu OUTPUT;
        END;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- 5. DIEU CHUYEN GIUA HAI KHO: XUAT VA NHAP TRONG CUNG TRANSACTION.
CREATE OR ALTER PROCEDURE dbo.usp_Kho_XacNhanChuyenKho
    @MaChuyenKho bigint
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @Lock int;
        EXEC @Lock=sys.sp_getapplock @Resource=N'QLKS_KHO_TON_KHO',
             @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
        IF @Lock < 0 THROW 52200, N'Khong khoa duoc giao dich chuyen kho.', 1;

        DECLARE @MaKhoXuat int, @MaKhoNhap int, @MaNhanVien int, @TrangThai varchar(20);
        SELECT @MaKhoXuat=MaKhoXuat, @MaKhoNhap=MaKhoNhap,
               @MaNhanVien=MaNhanVien, @TrangThai=TrangThai
          FROM dbo.PhieuChuyenKho WITH (UPDLOCK, HOLDLOCK)
         WHERE MaChuyenKho=@MaChuyenKho;
        IF @TrangThai IS NULL THROW 52201, N'Phieu chuyen kho khong ton tai.', 1;
        IF @TrangThai <> 'MOI' THROW 52202, N'Phieu chuyen kho da duoc xu ly.', 1;
        IF NOT EXISTS(SELECT 1 FROM dbo.ChiTietChuyenKho WHERE MaChuyenKho=@MaChuyenKho)
            THROW 52203, N'Chua co vat tu de chuyen.', 1;

        DECLARE @MaVatTu int, @SoLuong decimal(18,4);
        DECLARE chuyen_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT MaVatTu, SoLuong FROM dbo.ChiTietChuyenKho
             WHERE MaChuyenKho=@MaChuyenKho ORDER BY MaVatTu;
        OPEN chuyen_cursor;
        FETCH NEXT FROM chuyen_cursor INTO @MaVatTu, @SoLuong;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @Xuat decimal(18,4) = -@SoLuong;
            EXEC dbo.usp_Kho_BienDong @MaKho=@MaKhoXuat, @MaVatTu=@MaVatTu,
                 @BienDong=@Xuat, @LoaiChungTu='CHUYEN_KHO',
                 @MaChungTu=@MaChuyenKho, @MaNhanVien=@MaNhanVien,
                 @GhiChu=N'Chuyen ra kho khac';
            EXEC dbo.usp_Kho_BienDong @MaKho=@MaKhoNhap, @MaVatTu=@MaVatTu,
                 @BienDong=@SoLuong, @LoaiChungTu='CHUYEN_KHO',
                 @MaChungTu=@MaChuyenKho, @MaNhanVien=@MaNhanVien,
                 @GhiChu=N'Nhan tu kho khac';
            FETCH NEXT FROM chuyen_cursor INTO @MaVatTu, @SoLuong;
        END;
        CLOSE chuyen_cursor;
        DEALLOCATE chuyen_cursor;

        UPDATE dbo.PhieuChuyenKho SET TrangThai='DA_XAC_NHAN', NgayXacNhan=SYSDATETIME()
         WHERE MaChuyenKho=@MaChuyenKho;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Kho_ChuyenKhoNhanh
    @MaKhoXuat int,
    @MaKhoNhap int,
    @MaVatTu int,
    @SoLuong decimal(18,4),
    @MaNhanVien int,
    @GhiChu nvarchar(500) = NULL,
    @MaChuyenKho bigint = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @MaKhoXuat = @MaKhoNhap OR @MaKhoXuat IS NULL OR @MaKhoNhap IS NULL
        THROW 52210, N'Kho xuat phai khac kho nhap.', 1;
    IF @SoLuong IS NULL OR @SoLuong <= 0
        THROW 52211, N'So luong chuyen phai lon hon 0.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        INSERT dbo.PhieuChuyenKho(MaKhoXuat, MaKhoNhap, MaNhanVien, GhiChu)
        VALUES (@MaKhoXuat, @MaKhoNhap, @MaNhanVien, @GhiChu);
        SET @MaChuyenKho = SCOPE_IDENTITY();
        INSERT dbo.ChiTietChuyenKho(MaChuyenKho, MaVatTu, SoLuong)
        VALUES (@MaChuyenKho, @MaVatTu, @SoLuong);
        EXEC dbo.usp_Kho_XacNhanChuyenKho @MaChuyenKho=@MaChuyenKho;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- 6. CAP VAT TU NOI BO: KHONG TAC DONG SO PHONG, DAT PHONG HAY HOA DON.
CREATE OR ALTER PROCEDURE dbo.usp_Kho_XacNhanCapNoiBo
    @MaPhieuCap bigint
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @Lock int;
        EXEC @Lock=sys.sp_getapplock @Resource=N'QLKS_KHO_TON_KHO',
             @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
        IF @Lock < 0 THROW 52300, N'Khong khoa duoc giao dich cap noi bo.', 1;

        DECLARE @MaKho int, @MaNhanVien int, @TrangThai varchar(20);
        SELECT @MaKho=MaKho, @MaNhanVien=MaNhanVien, @TrangThai=TrangThai
          FROM dbo.PhieuCapVatTuNoiBo WITH (UPDLOCK, HOLDLOCK)
         WHERE MaPhieuCap=@MaPhieuCap;
        IF @TrangThai IS NULL THROW 52301, N'Phieu cap noi bo khong ton tai.', 1;
        IF @TrangThai <> 'MOI' THROW 52302, N'Phieu cap noi bo da duoc xu ly.', 1;
        IF EXISTS (SELECT 1 FROM dbo.PhieuCapVatTuNoiBo WHERE MaPhieuCap=@MaPhieuCap
            AND ((MaDatPhong IS NOT NULL AND MucDich<>'DON_PHONG')
              OR (MaSuDungDichVu IS NOT NULL AND MucDich<>'DICH_VU')))
            THROW 52304, N'Tham chiếu không khớp mục đích cấp nội bộ.', 1;
        IF NOT EXISTS(SELECT 1 FROM dbo.ChiTietCapVatTuNoiBo WHERE MaPhieuCap=@MaPhieuCap)
            THROW 52303, N'Phieu cap noi bo chua co vat tu.', 1;

        DECLARE @MaVatTu int, @SoLuong decimal(18,4);
        DECLARE cap_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT MaVatTu, SoLuong FROM dbo.ChiTietCapVatTuNoiBo
             WHERE MaPhieuCap=@MaPhieuCap ORDER BY MaVatTu;
        OPEN cap_cursor;
        FETCH NEXT FROM cap_cursor INTO @MaVatTu, @SoLuong;
        WHILE @@FETCH_STATUS=0
        BEGIN
            DECLARE @Xuat decimal(18,4) = -@SoLuong;
            EXEC dbo.usp_Kho_BienDong @MaKho=@MaKho, @MaVatTu=@MaVatTu,
                 @BienDong=@Xuat, @LoaiChungTu='CAP_NOI_BO',
                 @MaChungTu=@MaPhieuCap, @MaNhanVien=@MaNhanVien;
            FETCH NEXT FROM cap_cursor INTO @MaVatTu, @SoLuong;
        END;
        CLOSE cap_cursor;
        DEALLOCATE cap_cursor;

        UPDATE dbo.PhieuCapVatTuNoiBo SET TrangThai='DA_XAC_NHAN', NgayXacNhan=SYSDATETIME()
         WHERE MaPhieuCap=@MaPhieuCap;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Kho_CapNoiBoNhanh
    @MaKho int,
    @MaVatTu int,
    @SoLuong decimal(18,4),
    @MaNhanVien int,
    @MucDich varchar(20),
    @MaDatPhong bigint = NULL,
    @MaSuDungDichVu bigint = NULL,
    @GhiChu nvarchar(500) = NULL,
    @MaPhieuCap bigint = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @SoLuong IS NULL OR @SoLuong <= 0
        THROW 52310, N'So luong cap noi bo phai lon hon 0.', 1;
    IF @MucDich IS NULL OR @MucDich NOT IN ('DON_PHONG','DICH_VU','KHAC')
        THROW 52311, N'Muc dich cap noi bo khong hop le.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        INSERT dbo.PhieuCapVatTuNoiBo(MaKho, MaNhanVien, MucDich,
                                      MaDatPhong, MaSuDungDichVu, GhiChu)
        VALUES (@MaKho, @MaNhanVien, @MucDich, @MaDatPhong, @MaSuDungDichVu, @GhiChu);
        SET @MaPhieuCap=SCOPE_IDENTITY();
        INSERT dbo.ChiTietCapVatTuNoiBo(MaPhieuCap, MaVatTu, SoLuong)
        VALUES (@MaPhieuCap, @MaVatTu, @SoLuong);
        EXEC dbo.usp_Kho_XacNhanCapNoiBo @MaPhieuCap=@MaPhieuCap;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- 7. KIEM KE: KHONG CHAP NHAN KET QUA NEU TON DA THAY DOI SAU KHI KIEM.
CREATE OR ALTER PROCEDURE dbo.usp_Kho_XacNhanKiemKe
    @MaKiemKe bigint
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @Lock int;
        EXEC @Lock=sys.sp_getapplock @Resource=N'QLKS_KHO_TON_KHO',
             @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
        IF @Lock < 0 THROW 52400, N'Khong khoa duoc giao dich kiem ke.', 1;

        DECLARE @MaKho int, @MaNhanVien int, @TrangThai varchar(20);
        SELECT @MaKho=MaKho, @MaNhanVien=MaNhanVien, @TrangThai=TrangThai
          FROM dbo.PhieuKiemKe WITH (UPDLOCK, HOLDLOCK) WHERE MaKiemKe=@MaKiemKe;
        IF @TrangThai IS NULL THROW 52401, N'Phieu kiem ke khong ton tai.', 1;
        IF @TrangThai <> 'MOI' THROW 52402, N'Phieu kiem ke da duoc xu ly.', 1;
        -- Them: van kiem tra danh muc khi chenh lech bang 0, khong goi BienDong.
        IF NOT EXISTS(SELECT 1 FROM dbo.Kho WHERE MaKho=@MaKho AND TrangThai=1)
           OR NOT EXISTS(SELECT 1 FROM dbo.NhanVien WHERE MaNhanVien=@MaNhanVien AND TrangThai='DangLam')
            THROW 52405, N'Kho hoặc nhân viên kiểm kê không còn hoạt động.', 1;
        IF EXISTS(SELECT 1 FROM dbo.ChiTietKiemKe c JOIN dbo.VatTu v ON v.MaLoaiVatTu=c.MaVatTu WHERE c.MaKiemKe=@MaKiemKe AND v.TrangThai=0)
            THROW 52406, N'Vật tư kiểm kê đã ngừng hoạt động.', 1;
        IF NOT EXISTS(SELECT 1 FROM dbo.ChiTietKiemKe WHERE MaKiemKe=@MaKiemKe)
            THROW 52403, N'Phieu kiem ke chua co vat tu.', 1;

        DECLARE @MaVatTu int, @TonSoSach decimal(18,4), @TonThucTe decimal(18,4), @HienTai decimal(18,4);
        DECLARE kiem_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT MaVatTu, TonSoSach, TonThucTe FROM dbo.ChiTietKiemKe
             WHERE MaKiemKe=@MaKiemKe ORDER BY MaVatTu;
        OPEN kiem_cursor;
        FETCH NEXT FROM kiem_cursor INTO @MaVatTu, @TonSoSach, @TonThucTe;
        WHILE @@FETCH_STATUS=0
        BEGIN
            SET @HienTai=0;
            SELECT @HienTai=SoLuongTon FROM dbo.TonKho WITH (UPDLOCK, HOLDLOCK)
             WHERE MaKho=@MaKho AND MaVatTu=@MaVatTu;
            IF @HienTai <> @TonSoSach
                THROW 52404, N'Ton kho da thay doi trong luc kiem ke. Hay kiem ke lai.', 1;
            IF @TonThucTe <> @TonSoSach
            BEGIN
                DECLARE @Delta decimal(18,4)=@TonThucTe-@TonSoSach;
                EXEC dbo.usp_Kho_BienDong @MaKho=@MaKho, @MaVatTu=@MaVatTu,
                     @BienDong=@Delta, @LoaiChungTu='KIEM_KE',
                     @MaChungTu=@MaKiemKe, @MaNhanVien=@MaNhanVien;
            END;
            FETCH NEXT FROM kiem_cursor INTO @MaVatTu, @TonSoSach, @TonThucTe;
        END;
        CLOSE kiem_cursor;
        DEALLOCATE kiem_cursor;

        UPDATE dbo.PhieuKiemKe SET TrangThai='DA_XAC_NHAN', NgayXacNhan=SYSDATETIME()
         WHERE MaKiemKe=@MaKiemKe;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Kho_KiemKeNhanh
    @MaKho int,
    @MaVatTu int,
    @TonThucTe decimal(18,4),
    @MaNhanVien int,
    @LyDo nvarchar(500) = NULL,
    @MaKiemKe bigint = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @TonThucTe IS NULL OR @TonThucTe < 0
        THROW 52410, N'So luong kiem ke phai lon hon hoac bang 0.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @Lock int;
        EXEC @Lock=sys.sp_getapplock @Resource=N'QLKS_KHO_TON_KHO',
             @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
        IF @Lock < 0 THROW 52411, N'Khong khoa duoc ton kho.', 1;
        DECLARE @TonSoSach decimal(18,4)=0;
        SELECT @TonSoSach=SoLuongTon FROM dbo.TonKho WITH (UPDLOCK, HOLDLOCK)
         WHERE MaKho=@MaKho AND MaVatTu=@MaVatTu;
        INSERT dbo.PhieuKiemKe(MaKho, MaNhanVien, LyDo)
        VALUES (@MaKho, @MaNhanVien, @LyDo);
        SET @MaKiemKe=SCOPE_IDENTITY();
        INSERT dbo.ChiTietKiemKe(MaKiemKe, MaVatTu, TonSoSach, TonThucTe)
        VALUES (@MaKiemKe, @MaVatTu, @TonSoSach, @TonThucTe);
        EXEC dbo.usp_Kho_XacNhanKiemKe @MaKiemKe=@MaKiemKe;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- 8. CHAN SUA/XOA CHUNG TU DA XAC NHAN HOAC DA HUY.
-- Phieu da ghi ton khong duoc xoa truc tiep: dung chung tu dieu chinh.
CREATE OR ALTER TRIGGER dbo.trg_Kho_KhoaPhieuNhapXuat
ON dbo.PhieuNhapXuat AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted WHERE TrangThai <> 'MOI')
        THROW 52501, N'Khong duoc sua/xoa phieu kho da xu ly.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Kho_KhoaChiTietPhieuKho
ON dbo.ChiTietPhieuKho AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted i JOIN dbo.PhieuNhapXuat p ON p.MaPhieu=i.MaPhieu
         WHERE p.TrangThai <> 'MOI'
    ) OR EXISTS (
        SELECT 1 FROM deleted d JOIN dbo.PhieuNhapXuat p ON p.MaPhieu=d.MaPhieu
         WHERE p.TrangThai <> 'MOI'
    )
        THROW 52502, N'Chi duoc them/sua/xoa vat tu tren phieu MOI.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Kho_KhoaPhieuChuyen
ON dbo.PhieuChuyenKho AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted WHERE TrangThai <> 'MOI')
        THROW 52503, N'Khong duoc sua/xoa phieu chuyen kho da xu ly.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Kho_KhoaChiTietChuyen
ON dbo.ChiTietChuyenKho AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted i JOIN dbo.PhieuChuyenKho p ON p.MaChuyenKho=i.MaChuyenKho
         WHERE p.TrangThai <> 'MOI'
    ) OR EXISTS (
        SELECT 1 FROM deleted d JOIN dbo.PhieuChuyenKho p ON p.MaChuyenKho=d.MaChuyenKho
         WHERE p.TrangThai <> 'MOI'
    )
        THROW 52504, N'Chi duoc thay doi chi tiet cua phieu chuyen kho MOI.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Kho_KhoaPhieuCap
ON dbo.PhieuCapVatTuNoiBo AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted WHERE TrangThai <> 'MOI')
        THROW 52505, N'Khong duoc sua/xoa phieu cap noi bo da xu ly.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Kho_KhoaChiTietCap
ON dbo.ChiTietCapVatTuNoiBo AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted i JOIN dbo.PhieuCapVatTuNoiBo p ON p.MaPhieuCap=i.MaPhieuCap
         WHERE p.TrangThai <> 'MOI'
    ) OR EXISTS (
        SELECT 1 FROM deleted d JOIN dbo.PhieuCapVatTuNoiBo p ON p.MaPhieuCap=d.MaPhieuCap
         WHERE p.TrangThai <> 'MOI'
    )
        THROW 52506, N'Chi duoc thay doi chi tiet cua phieu cap noi bo MOI.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Kho_KhoaPhieuKiemKe
ON dbo.PhieuKiemKe AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted WHERE TrangThai <> 'MOI')
        THROW 52507, N'Khong duoc sua/xoa phieu kiem ke da xu ly.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Kho_KhoaChiTietKiemKe
ON dbo.ChiTietKiemKe AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted i JOIN dbo.PhieuKiemKe p ON p.MaKiemKe=i.MaKiemKe
         WHERE p.TrangThai <> 'MOI'
    ) OR EXISTS (
        SELECT 1 FROM deleted d JOIN dbo.PhieuKiemKe p ON p.MaKiemKe=d.MaKiemKe
         WHERE p.TrangThai <> 'MOI'
    )
        THROW 52508, N'Chi duoc thay doi chi tiet cua phieu kiem ke MOI.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Kho_BaoVeNhatKy
ON dbo.NhatKyKho AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    THROW 52509, N'Nhat ky kho chi duoc ghi them, khong duoc sua hoac xoa.', 1;
END;
GO

-- 9. KIEM TRA KHONG SUA DU LIEU.
SELECT
    (SELECT COUNT(*) FROM dbo.TonKho) AS SoDongTonKho,
    (SELECT COUNT(*) FROM dbo.PhieuNhapXuat) AS SoPhieuNhapXuat,
    (SELECT COUNT(*) FROM dbo.PhieuChuyenKho) AS SoPhieuChuyen,
    (SELECT COUNT(*) FROM dbo.PhieuCapVatTuNoiBo) AS SoPhieuCap,
    (SELECT COUNT(*) FROM dbo.PhieuKiemKe) AS SoPhieuKiemKe,
    (SELECT COUNT(*) FROM dbo.NhatKyKho) AS SoDongNhatKy;
SELECT name AS BangKhoDaTao FROM sys.tables
WHERE schema_id=SCHEMA_ID(N'dbo')
  AND name IN (N'TonKho', N'PhieuNhapXuat', N'ChiTietPhieuKho', N'PhieuChuyenKho',
               N'ChiTietChuyenKho', N'PhieuCapVatTuNoiBo', N'ChiTietCapVatTuNoiBo',
               N'PhieuKiemKe', N'ChiTietKiemKe', N'NhatKyKho')
ORDER BY name;
GO

/*
    HUONG DAN GHI DU LIEU TU C# (khong tu dong chay vi du):

    1) Tao danh muc Kho, NhomVatTu, VatTu, NhaCungCap bang Dapper.Contrib.
       MaVatTu trong cac bang nghiep vu = VatTu.MaLoaiVatTu trong bang goc.
       MaNhanVien phai la NhanVien.MaNhanVien THUC TE (khong phai 'NV001' trong demo).

    2) Nap du lieu ton dau ky: khong UPDATE truc tiep TonKho. Dung
       EXEC dbo.usp_Kho_NhapXuatNhanh
            @LoaiPhieu='DIEU_CHINH_TANG', @MaKho=1, @MaVatTu=1,
            @SoLuong=100, @MaNhanVien=1, @GhiChu=N'Ton dau ky';
       Thay ID = ID that hien co; chi chay 1 LAN cho ton dau ky.

    3) Nhap / Xuat: EXEC dbo.usp_Kho_NhapXuatNhanh ...
       @LoaiPhieu='NHAP' hoac 'XUAT'.
       Xuat khong du ton se tu ROLLBACK, khong tao phieu do dang.

    4) Lap phieu nhieu vat tu:
       INSERT dbo.PhieuNhapXuat(LoaiPhieu,MaKho,MaNhanVien,...) VALUES (...);
       Lay SCOPE_IDENTITY() -> INSERT cac ChiTietPhieuKho.
       EXEC dbo.usp_Kho_XacNhanPhieuNhapXuat @MaPhieu=<id>;
       Chi duoc sua/xoa dong chi tiet truoc khi xac nhan.

    5) Dieu chinh dung so ton thuc te:
       EXEC dbo.usp_Kho_DieuChinhTon @MaKho=1,@MaVatTu=1,
            @TonThucTe=95,@MaNhanVien=1,@LyDo=N'Kiem ke cuoi ca';
       Neu ton bang so thuc te: khong tao phieu dieu chinh.

    6) Chuyen kho: EXEC dbo.usp_Kho_ChuyenKhoNhanh
           @MaKhoXuat=1,@MaKhoNhap=2,@MaVatTu=1,@SoLuong=5,@MaNhanVien=1;
       Hoac lap PhieuChuyenKho + ChiTietChuyenKho, sau do xac nhan.

    7) Cap vat tu noi bo: EXEC dbo.usp_Kho_CapNoiBoNhanh
           @MaKho=1,@MaVatTu=1,@SoLuong=2,@MaNhanVien=1,@MucDich='DON_PHONG';
       MaDatPhong/MaSuDungDichVu neu co phai la ID that cua bang goc.
       KHONG tu dong ghi/doi SuDungDichVu cua Le Tan.

    8) Kiem ke: EXEC dbo.usp_Kho_KiemKeNhanh
           @MaKho=1,@MaVatTu=1,@TonThucTe=100,@MaNhanVien=1;
       Hoac lap PhieuKiemKe va ChiTietKiemKe voi TonSoSach ro rang,
       sau do EXEC dbo.usp_Kho_XacNhanKiemKe @MaKiemKe=<id>.

    9) Doc danh sach bang:
       SELECT * FROM dbo.vw_Kho_TonKho;
       SELECT * FROM dbo.vw_Kho_CanhBaoTon;
       SELECT * FROM dbo.vw_Kho_PhieuNhapXuat ORDER BY Ngay DESC;
       SELECT * FROM dbo.NhatKyKho ORDER BY ThoiGian DESC;

    10) DemoSession.CanUseButton van kiem tra quyen trong C#.
        Bang Quyen duoc them ma quyen KHO_*, KHONG tu dong gan VaiTro_Quyen
        de tranh thay doi vai tro hien tai. Khi chuyen sang phan quyen DB,
        can cap quyen va cap nhat BUS/UI mot cach chu dong.

    LUU Y GIOI HAN:
    - Trigger khoa phieu/chi tiet sau xac nhan va chong sua xoa nhat ky.
    - DAL/BUS can chi su dung usp_Kho_* cho moi bien dong ton; khong cho ung
      dung sua truc tiep TonKho. Khong tu cap quyen SQL ghi truc tiep TonKho.
    - Khong seed kho, vat tu hay ton gia. Ton dau ky nhap qua phieu/kiem ke.
    - Chay toan bo file; nen dung sqlcmd -b de dung ngay khi mot batch loi.
      Cac batch view/procedure/trigger co the chay lai neu nang cap bi gian doan.
*/
