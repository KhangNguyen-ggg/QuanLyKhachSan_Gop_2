/*
    DATABASE TỔNG QUẢN LÝ KHÁCH SẠN — 15/10/2026
    Mở file trong SSMS và Execute toàn bộ. SQL Server 2019 trở lên.
    DB mặc định: QuanLyKhachSan5Sao2 (đổi tên này trong file nếu dùng DB khác).

    - Chưa có DB: tạo DB theo đường dẫn mặc định của SQL Server.
    - Chưa có bảng: tạo 40 bảng gốc, nạp bộ mẫu đầy đủ có 5 khách đang ở.
    - Đã có đủ bảng: giữ dữ liệu, bỏ qua tạo bảng và nạp mẫu.
    - Bổ sung KhenThuong, sửa quan hệ Lịch–Chấm công và cấp mã lịch bằng SEQUENCE.
    - Gộp các thủ tục Lễ tân, Hệ thống và Bảng lương.
    - Có thể chạy lại; không xóa database hoặc xóa dữ liệu đang có.
    - Nếu DB mới chỉ có một phần các bảng gốc, script dừng để tránh nâng cấp nhầm.

    Các bước trong DB dùng chung một transaction; lỗi sẽ rollback toàn bộ bước đó.
    DB vừa được tạo sẽ còn rỗng nếu khởi tạo lỗi; chạy lại file sau khi xử lý lỗi.
    Dữ liệu mẫu chỉ phục vụ minh họa; tài khoản mau_* bị khóa, không đổi đăng nhập demo.
    Các tab còn dùng dữ liệu mẫu trong C# vẫn cần nối DAL/BUS/binding.

    MaLich mới: gọi dbo.usp_ThemLichLamViec, hoặc INSERT bỏ MaLich và nhận OUTPUT INSERTED.MaLich.
    LichLamViec.MaChamCong cũ giữ tương thích; đọc chấm công qua JOIN theo MaLich.
    Không có trigger tự cộng KhenThuong vào BangLuong.TienThuong.
*/
USE [master];
GO
IF @@TRANCOUNT <> 0
    THROW 51200, N'Hãy chạy file trong cửa sổ không có transaction đang mở.', 1;
IF DB_ID(N'QuanLyKhachSan5Sao2') IS NULL
    EXEC(N'CREATE DATABASE [QuanLyKhachSan5Sao2];');
GO
USE [QuanLyKhachSan5Sao2];
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;

IF @@TRANCOUNT <> 0
    THROW 51200, N'Hãy chạy file trong cửa sổ không có transaction đang mở.', 1;

BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @DbTongKhoa int;
    EXEC @DbTongKhoa = sys.sp_getapplock @Resource=N'QLKS_DATABASE_TONG',
        @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
    IF @DbTongKhoa < 0
        THROW 51201, N'Có phiên khác đang chạy file DB tổng. Hãy thử lại sau.', 1;

    DECLARE @DbTongSoBang int = (SELECT COUNT(*) FROM sys.tables WHERE schema_id=SCHEMA_ID(N'dbo') AND name IN (N'BangLuong',N'BaoTriPhong',N'CaLamViec',N'ChamCong',N'ChiTietDatPhong',N'ChiTietGoiDichVu',N'ChiTietHoaDon',N'ChucVu',N'CongViecDonPhong',N'DatPhong',N'DichVu',N'GoiDichVu',N'HoaDon',N'KhachHang',N'Kho',N'KhuVuc',N'LichLamViec',N'LoaiDichVu',N'LoaiPhong',N'LoaiPhong_TienNghi',N'NhaCungCap',N'NhanVien',N'NhatKyThaoTac',N'NhomVatTu',N'Phong',N'PhongBan',N'Quyen',N'SuDungDichVu',N'TaiKhoan',N'Tang',N'ThanhToan',N'ThietBi',N'TienNghi',N'TieuHaoVatTu',N'UuDai',N'VaiTro',N'VaiTro_Quyen',N'VatTu',N'VatTu_NhaCungCap',N'XepPhong'));
    IF @DbTongSoBang NOT IN (0,40)
        THROW 51202, N'DB chỉ có một phần các bảng gốc. Script đã dừng, không xóa hoặc tự sửa dữ liệu.', 1;

    IF @DbTongSoBang = 0
    BEGIN
        PRINT N'Khởi tạo cấu trúc DB và dữ liệu mẫu mới...';

-- ==================== 1. CẤU TRÚC 40 BẢNG GỐC ====================
CREATE TABLE [dbo].[BangLuong](
	[MaBangLuong] [bigint] IDENTITY(1,1) NOT NULL,
	[MaNhanVien] [int] NOT NULL,
	[Thang] [int] NOT NULL,
	[Nam] [int] NOT NULL,
	[LuongCoBan] [decimal](18, 2) NOT NULL,
	[TienCong] [decimal](18, 2) NOT NULL,
	[TienTangCa] [decimal](18, 2) NOT NULL,
	[TienThuong] [decimal](18, 2) NOT NULL,
	[TienPhuCap] [decimal](18, 2) NOT NULL,
	[TienKhauTru] [decimal](18, 2) NOT NULL,
	[Thue] [decimal](18, 2) NOT NULL,
	[TongLuong] [decimal](18, 2) NOT NULL,
	[ThucLinh] [decimal](18, 2) NOT NULL,
	[NgayLap] [datetime2](0) NOT NULL,
	[TrangThai] [varchar](20) NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaBangLuong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MaNhanVien] ASC,
	[Thang] ASC,
	[Nam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[BaoTriPhong]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[BaoTriPhong](
	[MaBaoTri] [bigint] IDENTITY(1,1) NOT NULL,
	[MaPhong] [int] NOT NULL,
	[MaThietBi] [int] NULL,
	[MaNhanVien] [int] NULL,
	[MaNhaCungCap] [int] NULL,
	[MaNguoiLienHe] [int] NULL,
	[NgayYeuCau] [datetime2](0) NOT NULL,
	[NoiDung] [nvarchar](1000) NOT NULL,
	[MucDo] [varchar](20) NOT NULL,
	[TrangThai] [varchar](20) NOT NULL,
	[NgayHoanThanh] [datetime2](0) NULL,
	[ChiPhi] [decimal](18, 2) NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaBaoTri] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[CaLamViec]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[CaLamViec](
	[MaCa] [int] IDENTITY(1,1) NOT NULL,
	[TenCa] [nvarchar](100) NOT NULL,
	[GioBatDau] [time](7) NOT NULL,
	[GioKetThuc] [time](7) NOT NULL,
	[SoGioLam] [decimal](5, 2) NOT NULL,
	[TrangThai] [bit] NOT NULL,
	[GhiChu] [nvarchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaCa] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenCa] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[ChamCong]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ChamCong](
	[MaChamCong] [bigint] IDENTITY(1,1) NOT NULL,
	[MaLich] [bigint] NOT NULL,
	[MaNhanVien] [int] NOT NULL,
	[NgayChamCong] [date] NOT NULL,
	[GioVao] [datetime2](0) NULL,
	[GioRa] [datetime2](0) NULL,
	[SoGioLam] [decimal](5, 2) NULL,
	[SoGioTangCa] [decimal](5, 2) NOT NULL,
	[TrangThai] [varchar](20) NOT NULL,
	[GhiChu] [nvarchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaChamCong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MaLich] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[ChiTietDatPhong]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ChiTietDatPhong](
	[MaChiTietDatPhong] [bigint] IDENTITY(1,1) NOT NULL,
	[MaDatPhong] [bigint] NOT NULL,
	[MaLoaiPhong] [int] NOT NULL,
	[NgayNhanDuKien] [datetime2](0) NOT NULL,
	[NgayTraDuKien] [datetime2](0) NOT NULL,
	[SoLuongPhong] [int] NOT NULL,
	[NguoiLon] [int] NOT NULL,
	[TreEm] [int] NOT NULL,
	[DonGiaDuKien] [decimal](18, 2) NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaChiTietDatPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[ChiTietGoiDichVu]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ChiTietGoiDichVu](
	[MaGoiDichVu] [int] NOT NULL,
	[MaDichVu] [int] NOT NULL,
	[SoLuong] [decimal](12, 2) NOT NULL,
	[GhiChu] [nvarchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaGoiDichVu] ASC,
	[MaDichVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[ChiTietHoaDon]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ChiTietHoaDon](
	[MaChiTietHoaDon] [bigint] IDENTITY(1,1) NOT NULL,
	[MaHoaDon] [bigint] NOT NULL,
	[LoaiChiPhi] [varchar](20) NOT NULL,
	[MaChiTietDatPhong] [bigint] NULL,
	[MaSuDungDichVu] [bigint] NULL,
	[MaGoiDichVu] [int] NULL,
	[NoiDung] [nvarchar](300) NOT NULL,
	[SoLuong] [decimal](12, 2) NOT NULL,
	[DonGia] [decimal](18, 2) NOT NULL,
	[ThanhTien]  AS (CONVERT([decimal](18,2),[SoLuong]*[DonGia])) PERSISTED,
PRIMARY KEY CLUSTERED 
(
	[MaChiTietHoaDon] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[ChucVu]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ChucVu](
	[MaChucVu] [int] IDENTITY(1,1) NOT NULL,
	[TenChucVu] [nvarchar](100) NOT NULL,
	[Chucvu] [decimal](18, 2) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaChucVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenChucVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[CongViecDonPhong]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[CongViecDonPhong](
	[MaCongViec] [bigint] IDENTITY(1,1) NOT NULL,
	[MaPhong] [int] NOT NULL,
	[MaNhanVien] [int] NULL,
	[NgayGiao] [date] NOT NULL,
	[LoaiCongViec] [varchar](30) NOT NULL,
	[TrangThai] [varchar](20) NOT NULL,
	[ThoiGianBatDau] [datetime2](0) NULL,
	[ThoiGianKetThuc] [datetime2](0) NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaCongViec] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[DatPhong]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[DatPhong](
	[MaDatPhong] [bigint] IDENTITY(1,1) NOT NULL,
	[MaKhachHangDat] [int] NOT NULL,
	[MaNhanVienTao] [int] NULL,
	[NgayDat] [datetime2](0) NOT NULL,
	[TrangThai] [varchar](25) NOT NULL,
	[GhiChu] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaDatPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[DichVu]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[DichVu](
	[MaDichVu] [int] IDENTITY(1,1) NOT NULL,
	[MaLoaiDichVu] [int] NOT NULL,
	[TenDichVu] [nvarchar](150) NOT NULL,
	[DonViTinh] [nvarchar](50) NULL,
	[DonGia] [decimal](18, 2) NOT NULL,
	[MoTa] [nvarchar](1000) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaDichVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MaLoaiDichVu] ASC,
	[TenDichVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[GoiDichVu]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[GoiDichVu](
	[MaGoiDichVu] [int] IDENTITY(1,1) NOT NULL,
	[TenGoiDichVu] [nvarchar](150) NOT NULL,
	[DonGia] [decimal](18, 2) NOT NULL,
	[MoTa] [nvarchar](1000) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaGoiDichVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenGoiDichVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[HoaDon]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[HoaDon](
	[MaHoaDon] [bigint] IDENTITY(1,1) NOT NULL,
	[MaDatPhong] [bigint] NOT NULL,
	[MaKhachHang] [int] NOT NULL,
	[MaUuDai] [int] NULL,
	[MaNhanVienLap] [int] NULL,
	[NgayLap] [datetime2](0) NOT NULL,
	[TongTien] [decimal](18, 2) NOT NULL,
	[TienGiam] [decimal](18, 2) NOT NULL,
	[ThanhTien] [decimal](18, 2) NOT NULL,
	[TrangThai] [varchar](20) NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaHoaDon] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[KhachHang]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[KhachHang](
	[MaKhachHang] [int] IDENTITY(1,1) NOT NULL,
	[HoTen] [nvarchar](150) NOT NULL,
	[NgaySinh] [date] NULL,
	[GioiTinh] [char](1) NULL,
	[LoaiGiayTo] [nvarchar](20) NULL,
	[SoGiayTo] [varchar](50) NULL,
	[QuocTich] [nvarchar](80) NULL,
	[SoDienThoai] [varchar](20) NULL,
	[Email] [varchar](150) NULL,
	[DiaChi] [nvarchar](300) NULL,
	[GhiChu] [nvarchar](500) NULL,
	[NgayTao] [datetime2](0) NOT NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaKhachHang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[Kho]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Kho](
	[MaKho] [int] IDENTITY(1,1) NOT NULL,
	[TenKho] [nvarchar](100) NOT NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaKho] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenKho] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[KhuVuc]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[KhuVuc](
	[MaKhuVuc] [int] IDENTITY(1,1) NOT NULL,
	[TenKhuVuc] [nvarchar](100) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaKhuVuc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenKhuVuc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[LichLamViec]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[LichLamViec](
	[MaLich] [bigint] NOT NULL,
	[MaNhanVien] [int] NOT NULL,
	[MaCa] [int] NOT NULL,
	[NgayLam] [date] NOT NULL,
	[TrangThai] [varchar](20) NOT NULL,
	[MaChamCong] [bigint] IDENTITY(1,1) NOT NULL,
	[GhiChu] [nvarchar](300) NULL,
 CONSTRAINT [PK__LichLamV__728A9AE96C1A510C] PRIMARY KEY CLUSTERED 
(
	[MaLich] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ__LichLamV__5BCB12434ED4A6B0] UNIQUE NONCLUSTERED 
(
	[MaNhanVien] ASC,
	[NgayLam] ASC,
	[MaCa] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[LoaiDichVu]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[LoaiDichVu](
	[MaLoaiDichVu] [int] IDENTITY(1,1) NOT NULL,
	[TenLoaiDichVu] [nvarchar](100) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaLoaiDichVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenLoaiDichVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[LoaiPhong]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[LoaiPhong](
	[MaLoaiPhong] [int] IDENTITY(1,1) NOT NULL,
	[TenLoaiPhong] [nvarchar](100) NOT NULL,
	[SucChuaNguoiLon] [int] NOT NULL,
	[SucChuaTreEm] [int] NOT NULL,
	[GiaCoBan] [decimal](18, 2) NOT NULL,
	[MoTa] [nvarchar](1000) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaLoaiPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenLoaiPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[LoaiPhong_TienNghi]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[LoaiPhong_TienNghi](
	[MaLoaiPhong] [int] NOT NULL,
	[MaTienNghi] [int] NOT NULL,
	[SoLuong] [int] NOT NULL,
	[GhiChu] [nvarchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaLoaiPhong] ASC,
	[MaTienNghi] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[NhaCungCap]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[NhaCungCap](
	[MaNhaCungCap] [int] IDENTITY(1,1) NOT NULL,
	[TenNhaCungCap] [nvarchar](200) NOT NULL,
	[LoaiCungCap] [nvarchar](100) NULL,
	[SoDienThoai] [varchar](20) NULL,
	[Email] [varchar](150) NULL,
	[DiaChi] [nvarchar](300) NULL,
	[NguoiDaiDien] [nvarchar](150) NULL,
	[TrangThai] [bit] NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNhaCungCap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenNhaCungCap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[NhanVien]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[NhanVien](
	[MaNhanVien] [int] IDENTITY(1,1) NOT NULL,
	[HoTen] [nvarchar](150) NOT NULL,
	[NgaySinh] [date] NULL,
	[GioiTinh] [char](1) NULL,
	[SoDienThoai] [varchar](20) NULL,
	[Email] [varchar](150) NULL,
	[DiaChi] [nvarchar](300) NULL,
	[MaPhongBan] [int] NOT NULL,
	[MaChucVu] [int] NOT NULL,
	[NgayVaoLam] [date] NULL,
	[LuongCoBan] [decimal](18, 2) NOT NULL,
	[TrangThai] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNhanVien] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[NhatKyThaoTac]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[NhatKyThaoTac](
	[MaNhatKy] [bigint] IDENTITY(1,1) NOT NULL,
	[MaTaiKhoan] [int] NOT NULL,
	[HanhDong] [varchar](50) NOT NULL,
	[DoiTuong] [varchar](100) NULL,
	[MaBanGhi] [varchar](50) NULL,
	[ThoiGian] [datetime2](0) NOT NULL,
	[MoTa] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNhatKy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[NhomVatTu]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[NhomVatTu](
	[MaNhomVatTu] [int] IDENTITY(1,1) NOT NULL,
	[TenNhomVatTu] [nvarchar](100) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNhomVatTu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenNhomVatTu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[Phong]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Phong](
	[MaPhong] [int] IDENTITY(1,1) NOT NULL,
	[SoPhong] [varchar](20) NOT NULL,
	[MaTang] [int] NOT NULL,
	[MaLoaiPhong] [int] NOT NULL,
	[LoaiView] [nvarchar](100) NULL,
	[KhongHutThuoc] [bit] NOT NULL,
	[TrangThaiKhaiThac] [varchar](30) NOT NULL,
	[TrangThaiVeSinh] [varchar](20) NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MaTang] ASC,
	[SoPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[PhongBan]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[PhongBan](
	[MaPhongBan] [int] IDENTITY(1,1) NOT NULL,
	[TenPhongBan] [nvarchar](100) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaPhongBan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenPhongBan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[Quyen]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Quyen](
	[MaQuyen] [int] IDENTITY(1,1) NOT NULL,
	[MaChucNang] [nvarchar](100) NOT NULL,
	[TenQuyen] [nvarchar](150) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaQuyen] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MaChucNang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[SuDungDichVu]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[SuDungDichVu](
	[MaSuDungDichVu] [bigint] IDENTITY(1,1) NOT NULL,
	[MaDatPhong] [bigint] NOT NULL,
	[MaKhachHang] [int] NULL,
	[MaDichVu] [int] NULL,
	[MaGoiDichVu] [int] NULL,
	[MaNhanVien] [int] NULL,
	[SoLuong] [decimal](12, 2) NOT NULL,
	[DonGia] [decimal](18, 2) NOT NULL,
	[ThoiGianSuDung] [datetime2](0) NOT NULL,
	[TrangThai] [varchar](20) NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaSuDungDichVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[TaiKhoan]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[TaiKhoan](
	[MaTaiKhoan] [int] IDENTITY(1,1) NOT NULL,
	[MaNhanVien] [int] NOT NULL,
	[TenDangNhap] [varchar](100) NOT NULL,
	[MatKhauHash] [varchar](500) NOT NULL,
	[MaVaiTro] [int] NOT NULL,
	[TrangThai] [varchar](20) NOT NULL,
	[LanDangNhapCuoi] [datetime2](0) NULL,
	[NgayTao] [datetime2](0) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaTaiKhoan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenDangNhap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MaNhanVien] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[Tang]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Tang](
	[MaTang] [int] IDENTITY(1,1) NOT NULL,
	[MaKhuVuc] [int] NOT NULL,
	[SoTang] [int] NOT NULL,
	[TenTang] [nvarchar](100) NULL,
	[MoTa] [nvarchar](500) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaTang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MaKhuVuc] ASC,
	[SoTang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[ThanhToan]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ThanhToan](
	[MaThanhToan] [bigint] IDENTITY(1,1) NOT NULL,
	[MaHoaDon] [bigint] NOT NULL,
	[MaNhanVien] [int] NULL,
	[SoTien] [decimal](18, 2) NOT NULL,
	[ThoiGianThanhToan] [datetime2](0) NOT NULL,
	[MaGiaoDich] [varchar](100) NULL,
	[TrangThai] [varchar](20) NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaThanhToan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[ThietBi]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ThietBi](
	[MaThietBi] [int] IDENTITY(1,1) NOT NULL,
	[MaPhong] [int] NULL,
	[MaNhaCungCap] [int] NULL,
	[TenThietBi] [nvarchar](150) NOT NULL,
	[MaThietBiNCC] [varchar](100) NULL,
	[NgayMua] [date] NULL,
	[HanBaoHanh] [date] NULL,
	[TrangThai] [varchar](30) NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaThietBi] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[TienNghi]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[TienNghi](
	[MaTienNghi] [int] IDENTITY(1,1) NOT NULL,
	[TenTienNghi] [nvarchar](150) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaTienNghi] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenTienNghi] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[TieuHaoVatTu]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[TieuHaoVatTu](
	[MaDichVu] [int] NOT NULL,
	[MaVatTu] [int] NOT NULL,
	[SoLuongTieuHao] [decimal](12, 4) NOT NULL,
	[GhiChu] [nvarchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaDichVu] ASC,
	[MaVatTu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[UuDai]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[UuDai](
	[MaUuDai] [int] IDENTITY(1,1) NOT NULL,
	[MaCode] [varchar](50) NOT NULL,
	[TenUuDai] [nvarchar](150) NOT NULL,
	[LoaiGiam] [varchar](20) NOT NULL,
	[GiaTriGiam] [decimal](18, 2) NOT NULL,
	[NgayBatDau] [date] NOT NULL,
	[NgayKetThuc] [date] NOT NULL,
	[DieuKien] [nvarchar](1000) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaUuDai] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MaCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[VaiTro]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[VaiTro](
	[MaVaiTro] [int] IDENTITY(1,1) NOT NULL,
	[TenVaiTro] [nvarchar](100) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaVaiTro] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenVaiTro] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[VaiTro_Quyen]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[VaiTro_Quyen](
	[MaVaiTro] [int] NOT NULL,
	[MaQuyen] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaVaiTro] ASC,
	[MaQuyen] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[VatTu]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[VatTu](
	[MaLoaiVatTu] [int] IDENTITY(1,1) NOT NULL,
	[MaNhomVatTu] [int] NOT NULL,
	[MaKhoMacDinh] [int] NULL,
	[TenVatTu] [nvarchar](150) NOT NULL,
	[DonViTinh] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[SoLuongToiThieu] [decimal](18, 4) NOT NULL,
	[TrangThai] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaLoaiVatTu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[VatTu_NhaCungCap]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[VatTu_NhaCungCap](
	[MaVatTu] [int] NOT NULL,
	[MaNhaCungCap] [int] NOT NULL,
	[MaSanPhamNCC] [varchar](100) NULL,
	[DonGiaNhap] [decimal](18, 2) NULL,
	[ThoiGianCungCap] [date] NULL,
	[TrangThai] [bit] NOT NULL,
	[GhiChu] [nvarchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaVatTu] ASC,
	[MaNhaCungCap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[XepPhong]    Script Date: 9/25/2026 9:50:52 AM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[XepPhong](
	[MaXepPhong] [bigint] IDENTITY(1,1) NOT NULL,
	[MaChiTietDatPhong] [bigint] NOT NULL,
	[MaPhong] [int] NOT NULL,
	[NgayNhanThucTe] [datetime2](0) NULL,
	[NgayTraThucTe] [datetime2](0) NULL,
	[TrangThai] [varchar](20) NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaXepPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MaChiTietDatPhong] ASC,
	[MaPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Index [IX_BangLuong_ThangNam]    Script Date: 9/25/2026 9:50:52 AM ******/
CREATE NONCLUSTERED INDEX [IX_BangLuong_ThangNam] ON [dbo].[BangLuong]
(
	[Thang] ASC,
	[Nam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

SET ANSI_PADDING ON

/****** Object:  Index [IX_BaoTriPhong_TrangThai]    Script Date: 9/25/2026 9:50:52 AM ******/
CREATE NONCLUSTERED INDEX [IX_BaoTriPhong_TrangThai] ON [dbo].[BaoTriPhong]
(
	[TrangThai] ASC,
	[NgayYeuCau] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

/****** Object:  Index [IX_ChamCong_NhanVien_Ngay]    Script Date: 9/25/2026 9:50:52 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChamCong_NhanVien_Ngay] ON [dbo].[ChamCong]
(
	[MaNhanVien] ASC,
	[NgayChamCong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

/****** Object:  Index [IX_ChiTietDatPhong_Ngay]    Script Date: 9/25/2026 9:50:52 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChiTietDatPhong_Ngay] ON [dbo].[ChiTietDatPhong]
(
	[NgayNhanDuKien] ASC,
	[NgayTraDuKien] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

/****** Object:  Index [IX_DatPhong_KhachHang]    Script Date: 9/25/2026 9:50:52 AM ******/
CREATE NONCLUSTERED INDEX [IX_DatPhong_KhachHang] ON [dbo].[DatPhong]
(
	[MaKhachHangDat] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

/****** Object:  Index [IX_LichLamViec_NhanVien_Ngay]    Script Date: 9/25/2026 9:50:52 AM ******/
CREATE NONCLUSTERED INDEX [IX_LichLamViec_NhanVien_Ngay] ON [dbo].[LichLamViec]
(
	[MaNhanVien] ASC,
	[NgayLam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

/****** Object:  Index [IX_NhatKyThaoTac_TaiKhoan_ThoiGian]    Script Date: 9/25/2026 9:50:52 AM ******/
CREATE NONCLUSTERED INDEX [IX_NhatKyThaoTac_TaiKhoan_ThoiGian] ON [dbo].[NhatKyThaoTac]
(
	[MaTaiKhoan] ASC,
	[ThoiGian] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

/****** Object:  Index [IX_SuDungDichVu_DatPhong]    Script Date: 9/25/2026 9:50:52 AM ******/
CREATE NONCLUSTERED INDEX [IX_SuDungDichVu_DatPhong] ON [dbo].[SuDungDichVu]
(
	[MaDatPhong] ASC,
	[ThoiGianSuDung] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

/****** Object:  Index [IX_XepPhong_Phong]    Script Date: 9/25/2026 9:50:52 AM ******/
CREATE NONCLUSTERED INDEX [IX_XepPhong_Phong] ON [dbo].[XepPhong]
(
	[MaPhong] ASC,
	[NgayNhanThucTe] ASC,
	[NgayTraThucTe] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

ALTER TABLE [dbo].[BangLuong] ADD  DEFAULT ((0)) FOR [TienCong]

ALTER TABLE [dbo].[BangLuong] ADD  DEFAULT ((0)) FOR [TienTangCa]

ALTER TABLE [dbo].[BangLuong] ADD  DEFAULT ((0)) FOR [TienThuong]

ALTER TABLE [dbo].[BangLuong] ADD  DEFAULT ((0)) FOR [TienPhuCap]

ALTER TABLE [dbo].[BangLuong] ADD  DEFAULT ((0)) FOR [TienKhauTru]

ALTER TABLE [dbo].[BangLuong] ADD  DEFAULT ((0)) FOR [Thue]

ALTER TABLE [dbo].[BangLuong] ADD  DEFAULT ((0)) FOR [TongLuong]

ALTER TABLE [dbo].[BangLuong] ADD  DEFAULT ((0)) FOR [ThucLinh]

ALTER TABLE [dbo].[BangLuong] ADD  DEFAULT (sysdatetime()) FOR [NgayLap]

ALTER TABLE [dbo].[BangLuong] ADD  DEFAULT ('TamTinh') FOR [TrangThai]

ALTER TABLE [dbo].[BaoTriPhong] ADD  DEFAULT (sysdatetime()) FOR [NgayYeuCau]

ALTER TABLE [dbo].[BaoTriPhong] ADD  DEFAULT ('TrungBinh') FOR [MucDo]

ALTER TABLE [dbo].[BaoTriPhong] ADD  DEFAULT ('ChoXuLy') FOR [TrangThai]

ALTER TABLE [dbo].[BaoTriPhong] ADD  DEFAULT ((0)) FOR [ChiPhi]

ALTER TABLE [dbo].[CaLamViec] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[ChamCong] ADD  DEFAULT ((0)) FOR [SoGioTangCa]

ALTER TABLE [dbo].[ChamCong] ADD  DEFAULT ('DuCong') FOR [TrangThai]

ALTER TABLE [dbo].[ChiTietDatPhong] ADD  DEFAULT ((1)) FOR [NguoiLon]

ALTER TABLE [dbo].[ChiTietDatPhong] ADD  DEFAULT ((0)) FOR [TreEm]

ALTER TABLE [dbo].[ChiTietDatPhong] ADD  DEFAULT ((0)) FOR [DonGiaDuKien]

ALTER TABLE [dbo].[ChiTietGoiDichVu] ADD  DEFAULT ((1)) FOR [SoLuong]

ALTER TABLE [dbo].[ChiTietHoaDon] ADD  DEFAULT ((1)) FOR [SoLuong]

ALTER TABLE [dbo].[ChiTietHoaDon] ADD  DEFAULT ((0)) FOR [DonGia]

ALTER TABLE [dbo].[ChucVu] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[CongViecDonPhong] ADD  DEFAULT ('ChoLam') FOR [TrangThai]

ALTER TABLE [dbo].[DatPhong] ADD  DEFAULT (sysdatetime()) FOR [NgayDat]

ALTER TABLE [dbo].[DatPhong] ADD  DEFAULT ('ChoXacNhan') FOR [TrangThai]

ALTER TABLE [dbo].[DichVu] ADD  DEFAULT ((0)) FOR [DonGia]

ALTER TABLE [dbo].[DichVu] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[GoiDichVu] ADD  DEFAULT ((0)) FOR [DonGia]

ALTER TABLE [dbo].[GoiDichVu] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[HoaDon] ADD  DEFAULT (sysdatetime()) FOR [NgayLap]

ALTER TABLE [dbo].[HoaDon] ADD  DEFAULT ((0)) FOR [TongTien]

ALTER TABLE [dbo].[HoaDon] ADD  DEFAULT ((0)) FOR [TienGiam]

ALTER TABLE [dbo].[HoaDon] ADD  DEFAULT ((0)) FOR [ThanhTien]

ALTER TABLE [dbo].[HoaDon] ADD  DEFAULT ('ChuaThanhToan') FOR [TrangThai]

ALTER TABLE [dbo].[KhachHang] ADD  DEFAULT (sysdatetime()) FOR [NgayTao]

ALTER TABLE [dbo].[KhachHang] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[Kho] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[KhuVuc] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[LichLamViec] ADD  CONSTRAINT [DF__LichLamVi__Trang__7CD98669]  DEFAULT ('DaXep') FOR [TrangThai]

ALTER TABLE [dbo].[LoaiDichVu] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[LoaiPhong] ADD  DEFAULT ((0)) FOR [SucChuaTreEm]

ALTER TABLE [dbo].[LoaiPhong] ADD  DEFAULT ((0)) FOR [GiaCoBan]

ALTER TABLE [dbo].[LoaiPhong] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[LoaiPhong_TienNghi] ADD  DEFAULT ((1)) FOR [SoLuong]

ALTER TABLE [dbo].[NhaCungCap] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[NhanVien] ADD  DEFAULT ((0)) FOR [LuongCoBan]

ALTER TABLE [dbo].[NhanVien] ADD  DEFAULT ('DangLam') FOR [TrangThai]

ALTER TABLE [dbo].[NhatKyThaoTac] ADD  DEFAULT (sysdatetime()) FOR [ThoiGian]

ALTER TABLE [dbo].[NhomVatTu] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[Phong] ADD  DEFAULT ((1)) FOR [KhongHutThuoc]

ALTER TABLE [dbo].[Phong] ADD  DEFAULT ('SanSang') FOR [TrangThaiKhaiThac]

ALTER TABLE [dbo].[Phong] ADD  DEFAULT ('Sach') FOR [TrangThaiVeSinh]

ALTER TABLE [dbo].[PhongBan] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[Quyen] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[SuDungDichVu] ADD  DEFAULT ((1)) FOR [SoLuong]

ALTER TABLE [dbo].[SuDungDichVu] ADD  DEFAULT ((0)) FOR [DonGia]

ALTER TABLE [dbo].[SuDungDichVu] ADD  DEFAULT (sysdatetime()) FOR [ThoiGianSuDung]

ALTER TABLE [dbo].[SuDungDichVu] ADD  DEFAULT ('DaSuDung') FOR [TrangThai]

ALTER TABLE [dbo].[TaiKhoan] ADD  DEFAULT ('HoatDong') FOR [TrangThai]

ALTER TABLE [dbo].[TaiKhoan] ADD  DEFAULT (sysdatetime()) FOR [NgayTao]

ALTER TABLE [dbo].[Tang] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT (sysdatetime()) FOR [ThoiGianThanhToan]

ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT ('ThanhCong') FOR [TrangThai]

ALTER TABLE [dbo].[ThietBi] ADD  DEFAULT ('DangSuDung') FOR [TrangThai]

ALTER TABLE [dbo].[TienNghi] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[UuDai] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[VaiTro] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[VatTu] ADD  DEFAULT ((0)) FOR [SoLuongToiThieu]

ALTER TABLE [dbo].[VatTu] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[VatTu_NhaCungCap] ADD  DEFAULT ((1)) FOR [TrangThai]

ALTER TABLE [dbo].[XepPhong] ADD  DEFAULT ('DaXep') FOR [TrangThai]

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD FOREIGN KEY([MaNhanVien])
REFERENCES [dbo].[NhanVien] ([MaNhanVien])

ALTER TABLE [dbo].[BaoTriPhong]  WITH CHECK ADD FOREIGN KEY([MaNhanVien])
REFERENCES [dbo].[NhanVien] ([MaNhanVien])

ALTER TABLE [dbo].[BaoTriPhong]  WITH CHECK ADD FOREIGN KEY([MaNhaCungCap])
REFERENCES [dbo].[NhaCungCap] ([MaNhaCungCap])

ALTER TABLE [dbo].[BaoTriPhong]  WITH CHECK ADD FOREIGN KEY([MaPhong])
REFERENCES [dbo].[Phong] ([MaPhong])

ALTER TABLE [dbo].[BaoTriPhong]  WITH CHECK ADD FOREIGN KEY([MaThietBi])
REFERENCES [dbo].[ThietBi] ([MaThietBi])

ALTER TABLE [dbo].[ChamCong]  WITH CHECK ADD FOREIGN KEY([MaNhanVien])
REFERENCES [dbo].[NhanVien] ([MaNhanVien])

ALTER TABLE [dbo].[ChiTietDatPhong]  WITH CHECK ADD FOREIGN KEY([MaDatPhong])
REFERENCES [dbo].[DatPhong] ([MaDatPhong])

ALTER TABLE [dbo].[ChiTietDatPhong]  WITH CHECK ADD FOREIGN KEY([MaLoaiPhong])
REFERENCES [dbo].[LoaiPhong] ([MaLoaiPhong])

ALTER TABLE [dbo].[ChiTietGoiDichVu]  WITH CHECK ADD FOREIGN KEY([MaDichVu])
REFERENCES [dbo].[DichVu] ([MaDichVu])

ALTER TABLE [dbo].[ChiTietGoiDichVu]  WITH CHECK ADD FOREIGN KEY([MaGoiDichVu])
REFERENCES [dbo].[GoiDichVu] ([MaGoiDichVu])

ALTER TABLE [dbo].[ChiTietHoaDon]  WITH CHECK ADD FOREIGN KEY([MaChiTietDatPhong])
REFERENCES [dbo].[ChiTietDatPhong] ([MaChiTietDatPhong])

ALTER TABLE [dbo].[ChiTietHoaDon]  WITH CHECK ADD FOREIGN KEY([MaGoiDichVu])
REFERENCES [dbo].[GoiDichVu] ([MaGoiDichVu])

ALTER TABLE [dbo].[ChiTietHoaDon]  WITH CHECK ADD FOREIGN KEY([MaHoaDon])
REFERENCES [dbo].[HoaDon] ([MaHoaDon])

ALTER TABLE [dbo].[ChiTietHoaDon]  WITH CHECK ADD FOREIGN KEY([MaSuDungDichVu])
REFERENCES [dbo].[SuDungDichVu] ([MaSuDungDichVu])

ALTER TABLE [dbo].[CongViecDonPhong]  WITH CHECK ADD FOREIGN KEY([MaNhanVien])
REFERENCES [dbo].[NhanVien] ([MaNhanVien])

ALTER TABLE [dbo].[CongViecDonPhong]  WITH CHECK ADD FOREIGN KEY([MaPhong])
REFERENCES [dbo].[Phong] ([MaPhong])

ALTER TABLE [dbo].[DatPhong]  WITH CHECK ADD FOREIGN KEY([MaKhachHangDat])
REFERENCES [dbo].[KhachHang] ([MaKhachHang])

ALTER TABLE [dbo].[DichVu]  WITH CHECK ADD FOREIGN KEY([MaLoaiDichVu])
REFERENCES [dbo].[LoaiDichVu] ([MaLoaiDichVu])

ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD FOREIGN KEY([MaDatPhong])
REFERENCES [dbo].[DatPhong] ([MaDatPhong])

ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD FOREIGN KEY([MaKhachHang])
REFERENCES [dbo].[KhachHang] ([MaKhachHang])

ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD FOREIGN KEY([MaNhanVienLap])
REFERENCES [dbo].[NhanVien] ([MaNhanVien])

ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD FOREIGN KEY([MaUuDai])
REFERENCES [dbo].[UuDai] ([MaUuDai])

ALTER TABLE [dbo].[LichLamViec]  WITH CHECK ADD  CONSTRAINT [FK__LichLamVi__MaNha__7EC1CEDB] FOREIGN KEY([MaNhanVien])
REFERENCES [dbo].[NhanVien] ([MaNhanVien])

ALTER TABLE [dbo].[LichLamViec] CHECK CONSTRAINT [FK__LichLamVi__MaNha__7EC1CEDB]

ALTER TABLE [dbo].[LichLamViec]  WITH CHECK ADD  CONSTRAINT [FK__LichLamVie__MaCa__7FB5F314] FOREIGN KEY([MaCa])
REFERENCES [dbo].[CaLamViec] ([MaCa])

ALTER TABLE [dbo].[LichLamViec] CHECK CONSTRAINT [FK__LichLamVie__MaCa__7FB5F314]

ALTER TABLE [dbo].[LichLamViec]  WITH CHECK ADD  CONSTRAINT [FK_LichLamViec_ChamCong] FOREIGN KEY([MaChamCong])
REFERENCES [dbo].[ChamCong] ([MaChamCong])

ALTER TABLE [dbo].[LichLamViec] CHECK CONSTRAINT [FK_LichLamViec_ChamCong]

ALTER TABLE [dbo].[LoaiPhong_TienNghi]  WITH CHECK ADD FOREIGN KEY([MaLoaiPhong])
REFERENCES [dbo].[LoaiPhong] ([MaLoaiPhong])

ALTER TABLE [dbo].[LoaiPhong_TienNghi]  WITH CHECK ADD FOREIGN KEY([MaTienNghi])
REFERENCES [dbo].[TienNghi] ([MaTienNghi])

ALTER TABLE [dbo].[NhanVien]  WITH CHECK ADD  CONSTRAINT [FK_NhanVien_ChucVu] FOREIGN KEY([MaChucVu])
REFERENCES [dbo].[ChucVu] ([MaChucVu])

ALTER TABLE [dbo].[NhanVien] CHECK CONSTRAINT [FK_NhanVien_ChucVu]

ALTER TABLE [dbo].[NhanVien]  WITH CHECK ADD  CONSTRAINT [FK_NhanVien_PhongBan] FOREIGN KEY([MaPhongBan])
REFERENCES [dbo].[PhongBan] ([MaPhongBan])

ALTER TABLE [dbo].[NhanVien] CHECK CONSTRAINT [FK_NhanVien_PhongBan]

ALTER TABLE [dbo].[NhatKyThaoTac]  WITH CHECK ADD FOREIGN KEY([MaTaiKhoan])
REFERENCES [dbo].[TaiKhoan] ([MaTaiKhoan])

ALTER TABLE [dbo].[Phong]  WITH CHECK ADD FOREIGN KEY([MaLoaiPhong])
REFERENCES [dbo].[LoaiPhong] ([MaLoaiPhong])

ALTER TABLE [dbo].[Phong]  WITH CHECK ADD FOREIGN KEY([MaTang])
REFERENCES [dbo].[Tang] ([MaTang])

ALTER TABLE [dbo].[SuDungDichVu]  WITH CHECK ADD FOREIGN KEY([MaDatPhong])
REFERENCES [dbo].[DatPhong] ([MaDatPhong])

ALTER TABLE [dbo].[SuDungDichVu]  WITH CHECK ADD FOREIGN KEY([MaDichVu])
REFERENCES [dbo].[DichVu] ([MaDichVu])

ALTER TABLE [dbo].[SuDungDichVu]  WITH CHECK ADD FOREIGN KEY([MaGoiDichVu])
REFERENCES [dbo].[GoiDichVu] ([MaGoiDichVu])

ALTER TABLE [dbo].[SuDungDichVu]  WITH CHECK ADD FOREIGN KEY([MaKhachHang])
REFERENCES [dbo].[KhachHang] ([MaKhachHang])

ALTER TABLE [dbo].[TaiKhoan]  WITH CHECK ADD FOREIGN KEY([MaNhanVien])
REFERENCES [dbo].[NhanVien] ([MaNhanVien])

ALTER TABLE [dbo].[TaiKhoan]  WITH CHECK ADD FOREIGN KEY([MaVaiTro])
REFERENCES [dbo].[VaiTro] ([MaVaiTro])

ALTER TABLE [dbo].[Tang]  WITH CHECK ADD FOREIGN KEY([MaKhuVuc])
REFERENCES [dbo].[KhuVuc] ([MaKhuVuc])

ALTER TABLE [dbo].[ThanhToan]  WITH CHECK ADD FOREIGN KEY([MaHoaDon])
REFERENCES [dbo].[HoaDon] ([MaHoaDon])

ALTER TABLE [dbo].[ThanhToan]  WITH CHECK ADD FOREIGN KEY([MaNhanVien])
REFERENCES [dbo].[NhanVien] ([MaNhanVien])

ALTER TABLE [dbo].[ThietBi]  WITH CHECK ADD FOREIGN KEY([MaNhaCungCap])
REFERENCES [dbo].[NhaCungCap] ([MaNhaCungCap])

ALTER TABLE [dbo].[ThietBi]  WITH CHECK ADD FOREIGN KEY([MaPhong])
REFERENCES [dbo].[Phong] ([MaPhong])

ALTER TABLE [dbo].[TieuHaoVatTu]  WITH CHECK ADD FOREIGN KEY([MaDichVu])
REFERENCES [dbo].[DichVu] ([MaDichVu])

ALTER TABLE [dbo].[TieuHaoVatTu]  WITH CHECK ADD FOREIGN KEY([MaVatTu])
REFERENCES [dbo].[VatTu] ([MaLoaiVatTu])

ALTER TABLE [dbo].[VaiTro_Quyen]  WITH CHECK ADD FOREIGN KEY([MaQuyen])
REFERENCES [dbo].[Quyen] ([MaQuyen])

ALTER TABLE [dbo].[VaiTro_Quyen]  WITH CHECK ADD FOREIGN KEY([MaVaiTro])
REFERENCES [dbo].[VaiTro] ([MaVaiTro])

ALTER TABLE [dbo].[VatTu]  WITH CHECK ADD FOREIGN KEY([MaKhoMacDinh])
REFERENCES [dbo].[Kho] ([MaKho])

ALTER TABLE [dbo].[VatTu]  WITH CHECK ADD FOREIGN KEY([MaNhomVatTu])
REFERENCES [dbo].[NhomVatTu] ([MaNhomVatTu])

ALTER TABLE [dbo].[VatTu_NhaCungCap]  WITH CHECK ADD FOREIGN KEY([MaNhaCungCap])
REFERENCES [dbo].[NhaCungCap] ([MaNhaCungCap])

ALTER TABLE [dbo].[VatTu_NhaCungCap]  WITH CHECK ADD FOREIGN KEY([MaVatTu])
REFERENCES [dbo].[VatTu] ([MaLoaiVatTu])

ALTER TABLE [dbo].[XepPhong]  WITH CHECK ADD FOREIGN KEY([MaChiTietDatPhong])
REFERENCES [dbo].[ChiTietDatPhong] ([MaChiTietDatPhong])

ALTER TABLE [dbo].[XepPhong]  WITH CHECK ADD FOREIGN KEY([MaPhong])
REFERENCES [dbo].[Phong] ([MaPhong])

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([LuongCoBan]>=(0)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([Nam]>=(2000) AND [Nam]<=(2100)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([Thang]>=(1) AND [Thang]<=(12)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([ThucLinh]>=(0)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([Thue]>=(0)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([TienCong]>=(0)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([TienKhauTru]>=(0)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([TienPhuCap]>=(0)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([TienTangCa]>=(0)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([TienThuong]>=(0)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([TongLuong]>=(0)))

ALTER TABLE [dbo].[BangLuong]  WITH CHECK ADD CHECK  (([TrangThai]='DaThanhToan' OR [TrangThai]='DaDuyet' OR [TrangThai]='TamTinh'))

ALTER TABLE [dbo].[BaoTriPhong]  WITH CHECK ADD CHECK  (([ChiPhi]>=(0)))

ALTER TABLE [dbo].[BaoTriPhong]  WITH CHECK ADD CHECK  (([MucDo]='KhanCap' OR [MucDo]='Cao' OR [MucDo]='TrungBinh' OR [MucDo]='Thap'))

ALTER TABLE [dbo].[BaoTriPhong]  WITH CHECK ADD CHECK  (([TrangThai]='Huy' OR [TrangThai]='HoanThanh' OR [TrangThai]='DangXuLy' OR [TrangThai]='ChoXuLy'))

ALTER TABLE [dbo].[CaLamViec]  WITH CHECK ADD CHECK  (([SoGioLam]>(0)))

ALTER TABLE [dbo].[ChamCong]  WITH CHECK ADD CHECK  (([SoGioLam] IS NULL OR [SoGioLam]>=(0)))

ALTER TABLE [dbo].[ChamCong]  WITH CHECK ADD CHECK  (([SoGioTangCa]>=(0)))

ALTER TABLE [dbo].[ChamCong]  WITH CHECK ADD CHECK  (([TrangThai]='KhongCham' OR [TrangThai]='NghiPhep' OR [TrangThai]='Nghi' OR [TrangThai]='ThieuCong' OR [TrangThai]='DuCong'))

ALTER TABLE [dbo].[ChiTietDatPhong]  WITH CHECK ADD CHECK  (([DonGiaDuKien]>=(0)))

ALTER TABLE [dbo].[ChiTietDatPhong]  WITH CHECK ADD CHECK  (([NguoiLon]>=(0)))

ALTER TABLE [dbo].[ChiTietDatPhong]  WITH CHECK ADD CHECK  (([SoLuongPhong]>(0)))

ALTER TABLE [dbo].[ChiTietDatPhong]  WITH CHECK ADD CHECK  (([TreEm]>=(0)))

ALTER TABLE [dbo].[ChiTietDatPhong]  WITH CHECK ADD CHECK  (([NgayTraDuKien]>[NgayNhanDuKien]))

ALTER TABLE [dbo].[ChiTietGoiDichVu]  WITH CHECK ADD CHECK  (([SoLuong]>(0)))

ALTER TABLE [dbo].[ChiTietHoaDon]  WITH CHECK ADD CHECK  (([DonGia]>=(0)))

ALTER TABLE [dbo].[ChiTietHoaDon]  WITH CHECK ADD CHECK  (([LoaiChiPhi]='Khac' OR [LoaiChiPhi]='PhuThu' OR [LoaiChiPhi]='GoiDichVu' OR [LoaiChiPhi]='DichVu' OR [LoaiChiPhi]='TienPhong'))

ALTER TABLE [dbo].[ChiTietHoaDon]  WITH CHECK ADD CHECK  (([SoLuong]>(0)))

ALTER TABLE [dbo].[CongViecDonPhong]  WITH CHECK ADD CHECK  (([LoaiCongViec]='BoSungVatDung' OR [LoaiCongViec]='VeSinhDinhKy' OR [LoaiCongViec]='DonPhong'))

ALTER TABLE [dbo].[CongViecDonPhong]  WITH CHECK ADD CHECK  (([TrangThai]='Huy' OR [TrangThai]='HoanThanh' OR [TrangThai]='DangLam' OR [TrangThai]='ChoLam'))

ALTER TABLE [dbo].[DatPhong]  WITH CHECK ADD CHECK  (([TrangThai]='DaHuy' OR [TrangThai]='DaCheckOut' OR [TrangThai]='DaCheckIn' OR [TrangThai]='DaXacNhan' OR [TrangThai]='ChoXacNhan'))

ALTER TABLE [dbo].[DichVu]  WITH CHECK ADD CHECK  (([DonGia]>=(0)))

ALTER TABLE [dbo].[GoiDichVu]  WITH CHECK ADD CHECK  (([DonGia]>=(0)))

ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD CHECK  (([ThanhTien]>=(0)))

ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD CHECK  (([TienGiam]>=(0)))

ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD CHECK  (([TongTien]>=(0)))

ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD CHECK  (([TrangThai]='Huy' OR [TrangThai]='DaThanhToan' OR [TrangThai]='ThanhToanMotPhan' OR [TrangThai]='ChuaThanhToan'))

ALTER TABLE [dbo].[KhachHang]  WITH CHECK ADD CHECK  (([GioiTinh]='O' OR [GioiTinh]='F' OR [GioiTinh]='M'))

ALTER TABLE [dbo].[LichLamViec]  WITH CHECK ADD  CONSTRAINT [CK__LichLamVi__Trang__7DCDAAA2] CHECK  (([TrangThai]='Huy' OR [TrangThai]='Nghi' OR [TrangThai]='DaXep'))

ALTER TABLE [dbo].[LichLamViec] CHECK CONSTRAINT [CK__LichLamVi__Trang__7DCDAAA2]

ALTER TABLE [dbo].[LoaiPhong]  WITH CHECK ADD CHECK  (([GiaCoBan]>=(0)))

ALTER TABLE [dbo].[LoaiPhong]  WITH CHECK ADD CHECK  (([SucChuaNguoiLon]>(0)))

ALTER TABLE [dbo].[LoaiPhong]  WITH CHECK ADD CHECK  (([SucChuaTreEm]>=(0)))

ALTER TABLE [dbo].[LoaiPhong_TienNghi]  WITH CHECK ADD CHECK  (([SoLuong]>(0)))

ALTER TABLE [dbo].[NhanVien]  WITH CHECK ADD CHECK  (([GioiTinh]='O' OR [GioiTinh]='F' OR [GioiTinh]='M'))

ALTER TABLE [dbo].[NhanVien]  WITH CHECK ADD CHECK  (([LuongCoBan]>=(0)))

ALTER TABLE [dbo].[NhanVien]  WITH CHECK ADD CHECK  (([TrangThai]='NghiViec' OR [TrangThai]='TamNghi' OR [TrangThai]='DangLam'))

ALTER TABLE [dbo].[Phong]  WITH CHECK ADD CHECK  (([TrangThaiKhaiThac]='NgungKhaiThac' OR [TrangThaiKhaiThac]='BaoTri' OR [TrangThaiKhaiThac]='DangSuDung' OR [TrangThaiKhaiThac]='SanSang'))

ALTER TABLE [dbo].[Phong]  WITH CHECK ADD CHECK  (([TrangThaiVeSinh]='Khoa' OR [TrangThaiVeSinh]='DangDon' OR [TrangThaiVeSinh]='CanDon' OR [TrangThaiVeSinh]='Sach'))

ALTER TABLE [dbo].[SuDungDichVu]  WITH CHECK ADD CHECK  (([DonGia]>=(0)))

ALTER TABLE [dbo].[SuDungDichVu]  WITH CHECK ADD CHECK  (([SoLuong]>(0)))

ALTER TABLE [dbo].[SuDungDichVu]  WITH CHECK ADD CHECK  (([TrangThai]='Huy' OR [TrangThai]='DaSuDung' OR [TrangThai]='DatTruoc'))

ALTER TABLE [dbo].[SuDungDichVu]  WITH CHECK ADD CHECK  (([MaDichVu] IS NOT NULL AND [MaGoiDichVu] IS NULL OR [MaDichVu] IS NULL AND [MaGoiDichVu] IS NOT NULL))

ALTER TABLE [dbo].[TaiKhoan]  WITH CHECK ADD CHECK  (([TrangThai]='Khoa' OR [TrangThai]='HoatDong'))

ALTER TABLE [dbo].[ThanhToan]  WITH CHECK ADD CHECK  (([SoTien]>(0)))

ALTER TABLE [dbo].[ThanhToan]  WITH CHECK ADD CHECK  (([TrangThai]='Huy' OR [TrangThai]='ThanhCong' OR [TrangThai]='ChoXuLy'))

ALTER TABLE [dbo].[ThietBi]  WITH CHECK ADD CHECK  (([TrangThai]='ThanhLy' OR [TrangThai]='BaoTri' OR [TrangThai]='Hong' OR [TrangThai]='DangSuDung'))

ALTER TABLE [dbo].[TieuHaoVatTu]  WITH CHECK ADD CHECK  (([SoLuongTieuHao]>(0)))

ALTER TABLE [dbo].[UuDai]  WITH CHECK ADD CHECK  (([NgayKetThuc]>=[NgayBatDau]))

ALTER TABLE [dbo].[UuDai]  WITH CHECK ADD CHECK  (([GiaTriGiam]>=(0)))

ALTER TABLE [dbo].[UuDai]  WITH CHECK ADD CHECK  (([LoaiGiam]='SoTien' OR [LoaiGiam]='PhanTram'))

ALTER TABLE [dbo].[VatTu]  WITH CHECK ADD CHECK  (([SoLuongToiThieu]>=(0)))

ALTER TABLE [dbo].[VatTu_NhaCungCap]  WITH CHECK ADD CHECK  (([DonGiaNhap] IS NULL OR [DonGiaNhap]>=(0)))

ALTER TABLE [dbo].[XepPhong]  WITH CHECK ADD CHECK  (([TrangThai]='Huy' OR [TrangThai]='DaCheckOut' OR [TrangThai]='DaCheckIn' OR [TrangThai]='DaXep'))
-- ==================== 2. DỮ LIỆU MẪU CHỈ CHO DB VỪA KHỞI TẠO ====================
EXEC sys.sp_executesql N'/*
DỮ LIỆU MẪU QUẢN LÝ KHÁCH SẠN - BẢN ĐẦY ĐỦ CÓ 5 KHÁCH ĐANG Ở
Database: QuanLyKhachSan5Sao2. Chạy toàn bộ file trong một lần.
Gồm dữ liệu gốc của 40 bảng và thêm 5 đơn/chi tiết/lượt xếp phòng mới.
DatPhong, ChiTietDatPhong, XepPhong: mỗi bảng 10 dòng (5 cũ + 5 mới).
5 lượt cũ DaCheckOut được giữ; 5 lượt mới DaCheckIn, NgayTraThucTe = NULL.
5 phòng mẫu được chèn với trạng thái DangSuDung, tương ứng lượt check-in mới.
Chỉ nạp dữ liệu: không có proc, tạo bảng, xóa dữ liệu hoặc truy vấn báo cáo.
Giữ transaction và kiểm tra tránh nạp lại bộ mẫu (tài khoản mau_admin).
Nếu bộ mẫu đã được nạp, file dừng; dùng file bổ sung đã gửi để thêm vào bộ cũ.
Thông tin cá nhân đều giả lập; tài khoản mẫu bị khóa và chưa dùng để đăng nhập.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;

IF EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = ''mau_admin'')
BEGIN
    PRINT N''Đã có tài khoản mau_admin. Dừng để tránh thêm trùng bộ dữ liệu mẫu.'';
    RETURN;
END;

DECLARE @DangBatIdentityInsert bit = 0;
BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @KhuVuc1 int, @KhuVuc2 int, @KhuVuc3 int, @KhuVuc4 int, @KhuVuc5 int;
    DECLARE @LoaiPhong1 int, @LoaiPhong2 int, @LoaiPhong3 int, @LoaiPhong4 int, @LoaiPhong5 int;
    DECLARE @TienNghi1 int, @TienNghi2 int, @TienNghi3 int, @TienNghi4 int, @TienNghi5 int;
    DECLARE @PhongBan1 int, @PhongBan2 int, @PhongBan3 int, @PhongBan4 int, @PhongBan5 int;
    DECLARE @ChucVu1 int, @ChucVu2 int, @ChucVu3 int, @ChucVu4 int, @ChucVu5 int;
    DECLARE @CaLamViec1 int, @CaLamViec2 int, @CaLamViec3 int, @CaLamViec4 int, @CaLamViec5 int;
    DECLARE @VaiTro1 int, @VaiTro2 int, @VaiTro3 int, @VaiTro4 int, @VaiTro5 int;
    DECLARE @Quyen1 int, @Quyen2 int, @Quyen3 int, @Quyen4 int, @Quyen5 int;
    DECLARE @LoaiDichVu1 int, @LoaiDichVu2 int, @LoaiDichVu3 int, @LoaiDichVu4 int, @LoaiDichVu5 int;
    DECLARE @Kho1 int, @Kho2 int, @Kho3 int, @Kho4 int, @Kho5 int;
    DECLARE @NhomVatTu1 int, @NhomVatTu2 int, @NhomVatTu3 int, @NhomVatTu4 int, @NhomVatTu5 int;
    DECLARE @NhaCungCap1 int, @NhaCungCap2 int, @NhaCungCap3 int, @NhaCungCap4 int, @NhaCungCap5 int;
    DECLARE @UuDai1 int, @UuDai2 int, @UuDai3 int, @UuDai4 int, @UuDai5 int;
    DECLARE @Tang1 int, @Tang2 int, @Tang3 int, @Tang4 int, @Tang5 int;
    DECLARE @Phong1 int, @Phong2 int, @Phong3 int, @Phong4 int, @Phong5 int;
    DECLARE @NhanVien1 int, @NhanVien2 int, @NhanVien3 int, @NhanVien4 int, @NhanVien5 int;
    DECLARE @TaiKhoan1 int, @TaiKhoan2 int, @TaiKhoan3 int, @TaiKhoan4 int, @TaiKhoan5 int;
    DECLARE @KhachHang1 int, @KhachHang2 int, @KhachHang3 int, @KhachHang4 int, @KhachHang5 int;
    DECLARE @DichVu1 int, @DichVu2 int, @DichVu3 int, @DichVu4 int, @DichVu5 int;
    DECLARE @GoiDichVu1 int, @GoiDichVu2 int, @GoiDichVu3 int, @GoiDichVu4 int, @GoiDichVu5 int;
    DECLARE @VatTu1 int, @VatTu2 int, @VatTu3 int, @VatTu4 int, @VatTu5 int;
    DECLARE @ThietBi1 int, @ThietBi2 int, @ThietBi3 int, @ThietBi4 int, @ThietBi5 int;
    DECLARE @DatPhong1 bigint, @DatPhong2 bigint, @DatPhong3 bigint, @DatPhong4 bigint, @DatPhong5 bigint;
    DECLARE @ChiTietDatPhong1 bigint, @ChiTietDatPhong2 bigint, @ChiTietDatPhong3 bigint, @ChiTietDatPhong4 bigint, @ChiTietDatPhong5 bigint;
    DECLARE @XepPhong1 bigint, @XepPhong2 bigint, @XepPhong3 bigint, @XepPhong4 bigint, @XepPhong5 bigint;
    DECLARE @DatPhong6 bigint, @DatPhong7 bigint, @DatPhong8 bigint, @DatPhong9 bigint, @DatPhong10 bigint;
    DECLARE @ChiTietDatPhong6 bigint, @ChiTietDatPhong7 bigint, @ChiTietDatPhong8 bigint, @ChiTietDatPhong9 bigint, @ChiTietDatPhong10 bigint;
    DECLARE @XepPhong6 bigint, @XepPhong7 bigint, @XepPhong8 bigint, @XepPhong9 bigint, @XepPhong10 bigint;
    DECLARE @SuDungDichVu1 bigint, @SuDungDichVu2 bigint, @SuDungDichVu3 bigint, @SuDungDichVu4 bigint, @SuDungDichVu5 bigint;
    DECLARE @HoaDon1 bigint, @HoaDon2 bigint, @HoaDon3 bigint, @HoaDon4 bigint, @HoaDon5 bigint;
    DECLARE @ChiTietHoaDon1 bigint, @ChiTietHoaDon2 bigint, @ChiTietHoaDon3 bigint, @ChiTietHoaDon4 bigint, @ChiTietHoaDon5 bigint, @ChiTietHoaDon6 bigint, @ChiTietHoaDon7 bigint, @ChiTietHoaDon8 bigint, @ChiTietHoaDon9 bigint, @ChiTietHoaDon10 bigint;
    DECLARE @ThanhToan1 bigint, @ThanhToan2 bigint, @ThanhToan3 bigint, @ThanhToan4 bigint, @ThanhToan5 bigint;
    DECLARE @CongViecDonPhong1 bigint, @CongViecDonPhong2 bigint, @CongViecDonPhong3 bigint, @CongViecDonPhong4 bigint, @CongViecDonPhong5 bigint;
    DECLARE @BaoTriPhong1 bigint, @BaoTriPhong2 bigint, @BaoTriPhong3 bigint, @BaoTriPhong4 bigint, @BaoTriPhong5 bigint;
    DECLARE @ChamCong1 bigint, @ChamCong2 bigint, @ChamCong3 bigint, @ChamCong4 bigint, @ChamCong5 bigint;
    DECLARE @BangLuong1 bigint, @BangLuong2 bigint, @BangLuong3 bigint, @BangLuong4 bigint, @BangLuong5 bigint;
    DECLARE @NhatKyThaoTac1 bigint, @NhatKyThaoTac2 bigint, @NhatKyThaoTac3 bigint, @NhatKyThaoTac4 bigint, @NhatKyThaoTac5 bigint;
    -- Chọn mã lịch mới, tránh trùng cả lịch đã có và mã lịch trong chấm công.
    DECLARE @MaLichLonNhat bigint;
    SELECT @MaLichLonNhat = ISNULL(MAX(MaLich), 0)
    FROM (SELECT MaLich FROM dbo.LichLamViec
          UNION ALL SELECT MaLich FROM dbo.ChamCong) AS L;
    DECLARE @LichLamViec1 bigint = @MaLichLonNhat + 1,
            @LichLamViec2 bigint = @MaLichLonNhat + 2,
            @LichLamViec3 bigint = @MaLichLonNhat + 3,
            @LichLamViec4 bigint = @MaLichLonNhat + 4,
            @LichLamViec5 bigint = @MaLichLonNhat + 5;


    -- 01. KhuVuc: thêm 5 dòng
    INSERT INTO dbo.[KhuVuc] ([TenKhuVuc], [MoTa], [TrangThai])
    VALUES (N''Khu A (Mẫu)'', N''Khu trung tâm'', 1);
    SET @KhuVuc1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhuVuc] ([TenKhuVuc], [MoTa], [TrangThai])
    VALUES (N''Khu B (Mẫu)'', N''Khu hướng biển'', 1);
    SET @KhuVuc2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhuVuc] ([TenKhuVuc], [MoTa], [TrangThai])
    VALUES (N''Khu C (Mẫu)'', N''Khu sân vườn'', 1);
    SET @KhuVuc3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhuVuc] ([TenKhuVuc], [MoTa], [TrangThai])
    VALUES (N''Khu D (Mẫu)'', N''Khu gia đình'', 1);
    SET @KhuVuc4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhuVuc] ([TenKhuVuc], [MoTa], [TrangThai])
    VALUES (N''Khu E (Mẫu)'', N''Khu nghỉ dưỡng cao cấp'', 1);
    SET @KhuVuc5 = CONVERT(int, SCOPE_IDENTITY());

    -- 02. LoaiPhong: thêm 5 dòng
    INSERT INTO dbo.[LoaiPhong] ([TenLoaiPhong], [SucChuaNguoiLon], [SucChuaTreEm], [GiaCoBan], [MoTa], [TrangThai])
    VALUES (N''Standard (Mẫu)'', 2, 1, 800000, N''Phòng tiêu chuẩn, giường đôi'', 1);
    SET @LoaiPhong1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiPhong] ([TenLoaiPhong], [SucChuaNguoiLon], [SucChuaTreEm], [GiaCoBan], [MoTa], [TrangThai])
    VALUES (N''Superior (Mẫu)'', 2, 1, 1200000, N''Phòng hướng vườn'', 1);
    SET @LoaiPhong2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiPhong] ([TenLoaiPhong], [SucChuaNguoiLon], [SucChuaTreEm], [GiaCoBan], [MoTa], [TrangThai])
    VALUES (N''Deluxe (Mẫu)'', 2, 2, 1800000, N''Phòng hướng biển'', 1);
    SET @LoaiPhong3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiPhong] ([TenLoaiPhong], [SucChuaNguoiLon], [SucChuaTreEm], [GiaCoBan], [MoTa], [TrangThai])
    VALUES (N''Family (Mẫu)'', 4, 2, 2500000, N''Phòng gia đình hai giường lớn'', 1);
    SET @LoaiPhong4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiPhong] ([TenLoaiPhong], [SucChuaNguoiLon], [SucChuaTreEm], [GiaCoBan], [MoTa], [TrangThai])
    VALUES (N''Suite (Mẫu)'', 2, 2, 4000000, N''Phòng suite có phòng khách riêng'', 1);
    SET @LoaiPhong5 = CONVERT(int, SCOPE_IDENTITY());

    -- 03. TienNghi: thêm 5 dòng
    INSERT INTO dbo.[TienNghi] ([TenTienNghi], [MoTa], [TrangThai])
    VALUES (N''Điều hòa (Mẫu)'', N''Điều hòa hai chiều'', 1);
    SET @TienNghi1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TienNghi] ([TenTienNghi], [MoTa], [TrangThai])
    VALUES (N''Tivi (Mẫu)'', N''Tivi thông minh'', 1);
    SET @TienNghi2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TienNghi] ([TenTienNghi], [MoTa], [TrangThai])
    VALUES (N''Tủ lạnh (Mẫu)'', N''Tủ lạnh mini'', 1);
    SET @TienNghi3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TienNghi] ([TenTienNghi], [MoTa], [TrangThai])
    VALUES (N''Máy sấy tóc (Mẫu)'', N''Máy sấy trong phòng tắm'', 1);
    SET @TienNghi4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TienNghi] ([TenTienNghi], [MoTa], [TrangThai])
    VALUES (N''Két sắt (Mẫu)'', N''Két sắt điện tử'', 1);
    SET @TienNghi5 = CONVERT(int, SCOPE_IDENTITY());

    -- 04. PhongBan: thêm 5 dòng
    INSERT INTO dbo.[PhongBan] ([TenPhongBan], [MoTa], [TrangThai])
    VALUES (N''Ban quản lý (Mẫu)'', N''Điều hành khách sạn'', 1);
    SET @PhongBan1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[PhongBan] ([TenPhongBan], [MoTa], [TrangThai])
    VALUES (N''Lễ tân (Mẫu)'', N''Đặt phòng, nhận và trả phòng'', 1);
    SET @PhongBan2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[PhongBan] ([TenPhongBan], [MoTa], [TrangThai])
    VALUES (N''Buồng phòng (Mẫu)'', N''Vệ sinh phòng'', 1);
    SET @PhongBan3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[PhongBan] ([TenPhongBan], [MoTa], [TrangThai])
    VALUES (N''Kỹ thuật (Mẫu)'', N''Bảo trì thiết bị'', 1);
    SET @PhongBan4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[PhongBan] ([TenPhongBan], [MoTa], [TrangThai])
    VALUES (N''Kế toán (Mẫu)'', N''Hóa đơn và tiền lương'', 1);
    SET @PhongBan5 = CONVERT(int, SCOPE_IDENTITY());

    -- 05. ChucVu: thêm 5 dòng
    INSERT INTO dbo.[ChucVu] ([TenChucVu], [Chucvu], [MoTa], [TrangThai])
    VALUES (N''Quản lý (Mẫu)'', 2000000, N''Giá trị cột Chucvu được giữ đúng cấu trúc SQL gốc'', 1);
    SET @ChucVu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChucVu] ([TenChucVu], [Chucvu], [MoTa], [TrangThai])
    VALUES (N''Nhân viên lễ tân (Mẫu)'', 500000, N''Giá trị cột Chucvu được giữ đúng cấu trúc SQL gốc'', 1);
    SET @ChucVu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChucVu] ([TenChucVu], [Chucvu], [MoTa], [TrangThai])
    VALUES (N''Nhân viên buồng phòng (Mẫu)'', 300000, N''Giá trị cột Chucvu được giữ đúng cấu trúc SQL gốc'', 1);
    SET @ChucVu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChucVu] ([TenChucVu], [Chucvu], [MoTa], [TrangThai])
    VALUES (N''Nhân viên kỹ thuật (Mẫu)'', 500000, N''Giá trị cột Chucvu được giữ đúng cấu trúc SQL gốc'', 1);
    SET @ChucVu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChucVu] ([TenChucVu], [Chucvu], [MoTa], [TrangThai])
    VALUES (N''Kế toán viên (Mẫu)'', 700000, N''Giá trị cột Chucvu được giữ đúng cấu trúc SQL gốc'', 1);
    SET @ChucVu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 06. CaLamViec: thêm 5 dòng
    INSERT INTO dbo.[CaLamViec] ([TenCa], [GioBatDau], [GioKetThuc], [SoGioLam], [TrangThai], [GhiChu])
    VALUES (N''Ca sáng (Mẫu)'', N''06:00:00'', N''14:00:00'', 8, 1, NULL);
    SET @CaLamViec1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[CaLamViec] ([TenCa], [GioBatDau], [GioKetThuc], [SoGioLam], [TrangThai], [GhiChu])
    VALUES (N''Ca chiều (Mẫu)'', N''14:00:00'', N''22:00:00'', 8, 1, NULL);
    SET @CaLamViec2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[CaLamViec] ([TenCa], [GioBatDau], [GioKetThuc], [SoGioLam], [TrangThai], [GhiChu])
    VALUES (N''Ca đêm (Mẫu)'', N''22:00:00'', N''06:00:00'', 8, 1, N''Kết thúc vào ngày hôm sau'');
    SET @CaLamViec3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[CaLamViec] ([TenCa], [GioBatDau], [GioKetThuc], [SoGioLam], [TrangThai], [GhiChu])
    VALUES (N''Ca hành chính (Mẫu)'', N''08:00:00'', N''17:00:00'', 8, 1, N''Nghỉ trưa 12:00 đến 13:00'');
    SET @CaLamViec4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[CaLamViec] ([TenCa], [GioBatDau], [GioKetThuc], [SoGioLam], [TrangThai], [GhiChu])
    VALUES (N''Ca tăng cường (Mẫu)'', N''18:00:00'', N''22:00:00'', 4, 1, NULL);
    SET @CaLamViec5 = CONVERT(int, SCOPE_IDENTITY());

    -- 07. VaiTro: thêm 5 dòng
    INSERT INTO dbo.[VaiTro] ([TenVaiTro], [MoTa], [TrangThai])
    VALUES (N''Quản trị viên (Mẫu)'', N''Quản lý toàn hệ thống'', 1);
    SET @VaiTro1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VaiTro] ([TenVaiTro], [MoTa], [TrangThai])
    VALUES (N''Lễ tân (Mẫu)'', N''Quản lý đặt phòng'', 1);
    SET @VaiTro2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VaiTro] ([TenVaiTro], [MoTa], [TrangThai])
    VALUES (N''Buồng phòng (Mẫu)'', N''Quản lý dọn phòng'', 1);
    SET @VaiTro3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VaiTro] ([TenVaiTro], [MoTa], [TrangThai])
    VALUES (N''Kỹ thuật (Mẫu)'', N''Quản lý bảo trì'', 1);
    SET @VaiTro4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VaiTro] ([TenVaiTro], [MoTa], [TrangThai])
    VALUES (N''Kế toán (Mẫu)'', N''Quản lý thu chi'', 1);
    SET @VaiTro5 = CONVERT(int, SCOPE_IDENTITY());

    -- 08. Quyen: thêm 5 dòng
    INSERT INTO dbo.[Quyen] ([MaChucNang], [TenQuyen], [MoTa], [TrangThai])
    VALUES (N''MAU_HE_THONG'', N''Quản lý hệ thống'', NULL, 1);
    SET @Quyen1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Quyen] ([MaChucNang], [TenQuyen], [MoTa], [TrangThai])
    VALUES (N''MAU_DAT_PHONG'', N''Quản lý đặt phòng'', NULL, 1);
    SET @Quyen2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Quyen] ([MaChucNang], [TenQuyen], [MoTa], [TrangThai])
    VALUES (N''MAU_DON_PHONG'', N''Quản lý dọn phòng'', NULL, 1);
    SET @Quyen3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Quyen] ([MaChucNang], [TenQuyen], [MoTa], [TrangThai])
    VALUES (N''MAU_BAO_TRI'', N''Quản lý bảo trì'', NULL, 1);
    SET @Quyen4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Quyen] ([MaChucNang], [TenQuyen], [MoTa], [TrangThai])
    VALUES (N''MAU_KE_TOAN'', N''Quản lý kế toán'', NULL, 1);
    SET @Quyen5 = CONVERT(int, SCOPE_IDENTITY());

    -- 09. LoaiDichVu: thêm 5 dòng
    INSERT INTO dbo.[LoaiDichVu] ([TenLoaiDichVu], [MoTa], [TrangThai])
    VALUES (N''Ẩm thực (Mẫu)'', NULL, 1);
    SET @LoaiDichVu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiDichVu] ([TenLoaiDichVu], [MoTa], [TrangThai])
    VALUES (N''Giặt là (Mẫu)'', NULL, 1);
    SET @LoaiDichVu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiDichVu] ([TenLoaiDichVu], [MoTa], [TrangThai])
    VALUES (N''Spa (Mẫu)'', NULL, 1);
    SET @LoaiDichVu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiDichVu] ([TenLoaiDichVu], [MoTa], [TrangThai])
    VALUES (N''Đồ uống (Mẫu)'', NULL, 1);
    SET @LoaiDichVu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiDichVu] ([TenLoaiDichVu], [MoTa], [TrangThai])
    VALUES (N''Minibar (Mẫu)'', NULL, 1);
    SET @LoaiDichVu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 10. Kho: thêm 5 dòng
    INSERT INTO dbo.[Kho] ([TenKho], [TrangThai])
    VALUES (N''Kho thực phẩm (Mẫu)'', 1);
    SET @Kho1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Kho] ([TenKho], [TrangThai])
    VALUES (N''Kho giặt là (Mẫu)'', 1);
    SET @Kho2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Kho] ([TenKho], [TrangThai])
    VALUES (N''Kho spa (Mẫu)'', 1);
    SET @Kho3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Kho] ([TenKho], [TrangThai])
    VALUES (N''Kho đồ uống (Mẫu)'', 1);
    SET @Kho4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Kho] ([TenKho], [TrangThai])
    VALUES (N''Kho minibar (Mẫu)'', 1);
    SET @Kho5 = CONVERT(int, SCOPE_IDENTITY());

    -- 11. NhomVatTu: thêm 5 dòng
    INSERT INTO dbo.[NhomVatTu] ([TenNhomVatTu], [MoTa], [TrangThai])
    VALUES (N''Nguyên liệu ăn sáng (Mẫu)'', NULL, 1);
    SET @NhomVatTu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhomVatTu] ([TenNhomVatTu], [MoTa], [TrangThai])
    VALUES (N''Hóa chất giặt (Mẫu)'', NULL, 1);
    SET @NhomVatTu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhomVatTu] ([TenNhomVatTu], [MoTa], [TrangThai])
    VALUES (N''Dầu massage (Mẫu)'', NULL, 1);
    SET @NhomVatTu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhomVatTu] ([TenNhomVatTu], [MoTa], [TrangThai])
    VALUES (N''Nguyên liệu pha chế (Mẫu)'', NULL, 1);
    SET @NhomVatTu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhomVatTu] ([TenNhomVatTu], [MoTa], [TrangThai])
    VALUES (N''Nước đóng chai (Mẫu)'', NULL, 1);
    SET @NhomVatTu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 12. NhaCungCap: thêm 5 dòng
    INSERT INTO dbo.[NhaCungCap] ([TenNhaCungCap], [LoaiCungCap], [SoDienThoai], [Email], [DiaChi], [NguoiDaiDien], [TrangThai], [GhiChu])
    VALUES (N''Nhà cung cấp An Phát (Mẫu)'', N''Thực phẩm và thiết bị bếp'', N''02800000001'', N''ncc1@example.com'', N''Địa chỉ giả lập'', N''Người đại diện 1'', 1, N''Dữ liệu giả lập phục vụ học tập'');
    SET @NhaCungCap1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhaCungCap] ([TenNhaCungCap], [LoaiCungCap], [SoDienThoai], [Email], [DiaChi], [NguoiDaiDien], [TrangThai], [GhiChu])
    VALUES (N''Nhà cung cấp Sạch Việt (Mẫu)'', N''Hóa chất và thiết bị giặt'', N''02800000002'', N''ncc2@example.com'', N''Địa chỉ giả lập'', N''Người đại diện 2'', 1, N''Dữ liệu giả lập phục vụ học tập'');
    SET @NhaCungCap2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhaCungCap] ([TenNhaCungCap], [LoaiCungCap], [SoDienThoai], [Email], [DiaChi], [NguoiDaiDien], [TrangThai], [GhiChu])
    VALUES (N''Nhà cung cấp An Nhiên (Mẫu)'', N''Vật tư và thiết bị spa'', N''02800000003'', N''ncc3@example.com'', N''Địa chỉ giả lập'', N''Người đại diện 3'', 1, N''Dữ liệu giả lập phục vụ học tập'');
    SET @NhaCungCap3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhaCungCap] ([TenNhaCungCap], [LoaiCungCap], [SoDienThoai], [Email], [DiaChi], [NguoiDaiDien], [TrangThai], [GhiChu])
    VALUES (N''Nhà cung cấp Hương Việt (Mẫu)'', N''Đồ uống và thiết bị pha chế'', N''02800000004'', N''ncc4@example.com'', N''Địa chỉ giả lập'', N''Người đại diện 4'', 1, N''Dữ liệu giả lập phục vụ học tập'');
    SET @NhaCungCap4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhaCungCap] ([TenNhaCungCap], [LoaiCungCap], [SoDienThoai], [Email], [DiaChi], [NguoiDaiDien], [TrangThai], [GhiChu])
    VALUES (N''Nhà cung cấp Minh An (Mẫu)'', N''Nước uống và tủ lạnh'', N''02800000005'', N''ncc5@example.com'', N''Địa chỉ giả lập'', N''Người đại diện 5'', 1, N''Dữ liệu giả lập phục vụ học tập'');
    SET @NhaCungCap5 = CONVERT(int, SCOPE_IDENTITY());

    -- 13. UuDai: thêm 5 dòng
    INSERT INTO dbo.[UuDai] ([MaCode], [TenUuDai], [LoaiGiam], [GiaTriGiam], [NgayBatDau], [NgayKetThuc], [DieuKien], [TrangThai])
    VALUES (N''MAU2026_1'', N''Ưu đãi mẫu 1'', N''PhanTram'', 5, N''20260901'', N''20261231'', N''Áp dụng trên tổng hóa đơn; không cộng dồn ưu đãi'', 1);
    SET @UuDai1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[UuDai] ([MaCode], [TenUuDai], [LoaiGiam], [GiaTriGiam], [NgayBatDau], [NgayKetThuc], [DieuKien], [TrangThai])
    VALUES (N''MAU2026_2'', N''Ưu đãi mẫu 2'', N''PhanTram'', 10, N''20260901'', N''20261231'', N''Áp dụng trên tổng hóa đơn; không cộng dồn ưu đãi'', 1);
    SET @UuDai2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[UuDai] ([MaCode], [TenUuDai], [LoaiGiam], [GiaTriGiam], [NgayBatDau], [NgayKetThuc], [DieuKien], [TrangThai])
    VALUES (N''MAU2026_3'', N''Ưu đãi mẫu 3'', N''PhanTram'', 15, N''20260901'', N''20261231'', N''Áp dụng trên tổng hóa đơn; không cộng dồn ưu đãi'', 1);
    SET @UuDai3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[UuDai] ([MaCode], [TenUuDai], [LoaiGiam], [GiaTriGiam], [NgayBatDau], [NgayKetThuc], [DieuKien], [TrangThai])
    VALUES (N''MAU2026_4'', N''Ưu đãi mẫu 4'', N''PhanTram'', 20, N''20260901'', N''20261231'', N''Áp dụng trên tổng hóa đơn; không cộng dồn ưu đãi'', 1);
    SET @UuDai4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[UuDai] ([MaCode], [TenUuDai], [LoaiGiam], [GiaTriGiam], [NgayBatDau], [NgayKetThuc], [DieuKien], [TrangThai])
    VALUES (N''MAU2026_5'', N''Ưu đãi mẫu 5'', N''PhanTram'', 25, N''20260901'', N''20261231'', N''Áp dụng trên tổng hóa đơn; không cộng dồn ưu đãi'', 1);
    SET @UuDai5 = CONVERT(int, SCOPE_IDENTITY());

    -- 14. Tang: thêm 5 dòng
    INSERT INTO dbo.[Tang] ([MaKhuVuc], [SoTang], [TenTang], [MoTa], [TrangThai])
    VALUES (@KhuVuc1, 1, N''Tầng 1'', N''Tầng phục vụ lưu trú'', 1);
    SET @Tang1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Tang] ([MaKhuVuc], [SoTang], [TenTang], [MoTa], [TrangThai])
    VALUES (@KhuVuc2, 2, N''Tầng 2'', N''Tầng phục vụ lưu trú'', 1);
    SET @Tang2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Tang] ([MaKhuVuc], [SoTang], [TenTang], [MoTa], [TrangThai])
    VALUES (@KhuVuc3, 3, N''Tầng 3'', N''Tầng phục vụ lưu trú'', 1);
    SET @Tang3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Tang] ([MaKhuVuc], [SoTang], [TenTang], [MoTa], [TrangThai])
    VALUES (@KhuVuc4, 4, N''Tầng 4'', N''Tầng phục vụ lưu trú'', 1);
    SET @Tang4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Tang] ([MaKhuVuc], [SoTang], [TenTang], [MoTa], [TrangThai])
    VALUES (@KhuVuc5, 5, N''Tầng 5'', N''Tầng phục vụ lưu trú'', 1);
    SET @Tang5 = CONVERT(int, SCOPE_IDENTITY());

    -- 15. Phong: thêm 5 dòng
    INSERT INTO dbo.[Phong] ([SoPhong], [MaTang], [MaLoaiPhong], [LoaiView], [KhongHutThuoc], [TrangThaiKhaiThac], [TrangThaiVeSinh], [GhiChu])
    VALUES (N''M101'', @Tang1, @LoaiPhong1, N''Thành phố'', 1, N''DangSuDung'', N''Sach'', N''Phòng mẫu; đã hoàn tất vệ sinh và bảo trì'');
    SET @Phong1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Phong] ([SoPhong], [MaTang], [MaLoaiPhong], [LoaiView], [KhongHutThuoc], [TrangThaiKhaiThac], [TrangThaiVeSinh], [GhiChu])
    VALUES (N''M201'', @Tang2, @LoaiPhong2, N''Sân vườn'', 1, N''DangSuDung'', N''Sach'', N''Phòng mẫu; đã hoàn tất vệ sinh và bảo trì'');
    SET @Phong2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Phong] ([SoPhong], [MaTang], [MaLoaiPhong], [LoaiView], [KhongHutThuoc], [TrangThaiKhaiThac], [TrangThaiVeSinh], [GhiChu])
    VALUES (N''M301'', @Tang3, @LoaiPhong3, N''Biển'', 1, N''DangSuDung'', N''Sach'', N''Phòng mẫu; đã hoàn tất vệ sinh và bảo trì'');
    SET @Phong3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Phong] ([SoPhong], [MaTang], [MaLoaiPhong], [LoaiView], [KhongHutThuoc], [TrangThaiKhaiThac], [TrangThaiVeSinh], [GhiChu])
    VALUES (N''M401'', @Tang4, @LoaiPhong4, N''Hồ bơi'', 1, N''DangSuDung'', N''Sach'', N''Phòng mẫu; đã hoàn tất vệ sinh và bảo trì'');
    SET @Phong4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Phong] ([SoPhong], [MaTang], [MaLoaiPhong], [LoaiView], [KhongHutThuoc], [TrangThaiKhaiThac], [TrangThaiVeSinh], [GhiChu])
    VALUES (N''M501'', @Tang5, @LoaiPhong5, N''Biển'', 1, N''DangSuDung'', N''Sach'', N''Phòng mẫu; đã hoàn tất vệ sinh và bảo trì'');
    SET @Phong5 = CONVERT(int, SCOPE_IDENTITY());

    -- 16. LoaiPhong_TienNghi: thêm 25 dòng
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong1, @TienNghi1, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong1, @TienNghi2, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong1, @TienNghi3, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong1, @TienNghi4, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong1, @TienNghi5, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong2, @TienNghi1, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong2, @TienNghi2, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong2, @TienNghi3, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong2, @TienNghi4, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong2, @TienNghi5, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong3, @TienNghi1, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong3, @TienNghi2, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong3, @TienNghi3, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong3, @TienNghi4, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong3, @TienNghi5, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong4, @TienNghi1, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong4, @TienNghi2, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong4, @TienNghi3, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong4, @TienNghi4, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong4, @TienNghi5, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong5, @TienNghi1, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong5, @TienNghi2, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong5, @TienNghi3, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong5, @TienNghi4, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong5, @TienNghi5, 1, NULL);

    -- 17. NhanVien: thêm 5 dòng
    INSERT INTO dbo.[NhanVien] ([HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [MaPhongBan], [MaChucVu], [NgayVaoLam], [LuongCoBan], [TrangThai])
    VALUES (N''Nguyễn Minh Quân'', N''19910115'', N''M'', N''0900000001'', N''nhanvien1@example.com'', N''Địa chỉ giả lập tại TP. Hồ Chí Minh'', @PhongBan1, @ChucVu1, N''20250101'', 20000000, N''DangLam'');
    SET @NhanVien1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhanVien] ([HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [MaPhongBan], [MaChucVu], [NgayVaoLam], [LuongCoBan], [TrangThai])
    VALUES (N''Trần Thu Hà'', N''19920215'', N''F'', N''0900000002'', N''nhanvien2@example.com'', N''Địa chỉ giả lập tại TP. Hồ Chí Minh'', @PhongBan2, @ChucVu2, N''20250101'', 9000000, N''DangLam'');
    SET @NhanVien2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhanVien] ([HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [MaPhongBan], [MaChucVu], [NgayVaoLam], [LuongCoBan], [TrangThai])
    VALUES (N''Lê Thị Mai'', N''19930315'', N''F'', N''0900000003'', N''nhanvien3@example.com'', N''Địa chỉ giả lập tại TP. Hồ Chí Minh'', @PhongBan3, @ChucVu3, N''20250101'', 8000000, N''DangLam'');
    SET @NhanVien3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhanVien] ([HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [MaPhongBan], [MaChucVu], [NgayVaoLam], [LuongCoBan], [TrangThai])
    VALUES (N''Phạm Quốc Huy'', N''19940415'', N''M'', N''0900000004'', N''nhanvien4@example.com'', N''Địa chỉ giả lập tại TP. Hồ Chí Minh'', @PhongBan4, @ChucVu4, N''20250101'', 11000000, N''DangLam'');
    SET @NhanVien4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhanVien] ([HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [MaPhongBan], [MaChucVu], [NgayVaoLam], [LuongCoBan], [TrangThai])
    VALUES (N''Võ Ngọc Anh'', N''19950515'', N''F'', N''0900000005'', N''nhanvien5@example.com'', N''Địa chỉ giả lập tại TP. Hồ Chí Minh'', @PhongBan5, @ChucVu5, N''20250101'', 12000000, N''DangLam'');
    SET @NhanVien5 = CONVERT(int, SCOPE_IDENTITY());

    -- 18. TaiKhoan: thêm 5 dòng
    INSERT INTO dbo.[TaiKhoan] ([MaNhanVien], [TenDangNhap], [MatKhauHash], [MaVaiTro], [TrangThai], [LanDangNhapCuoi], [NgayTao])
    VALUES (@NhanVien1, N''mau_admin'', N''bb211e4f27037eccfd865c7d11d808b9391c04a2a694ecad5f4669b2a146b3d1'', @VaiTro1, N''Khoa'', NULL, N''2026-09-01T08:00:00'');
    SET @TaiKhoan1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TaiKhoan] ([MaNhanVien], [TenDangNhap], [MatKhauHash], [MaVaiTro], [TrangThai], [LanDangNhapCuoi], [NgayTao])
    VALUES (@NhanVien2, N''mau_letan'', N''6ebe4bbf6daaf1b3a6f944b20edd52601fc25aa7801d2aecedd8ceca5b69a35c'', @VaiTro2, N''Khoa'', NULL, N''2026-09-01T08:00:00'');
    SET @TaiKhoan2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TaiKhoan] ([MaNhanVien], [TenDangNhap], [MatKhauHash], [MaVaiTro], [TrangThai], [LanDangNhapCuoi], [NgayTao])
    VALUES (@NhanVien3, N''mau_buongphong'', N''4568d1e1584dca6c0581f107c52fe492d7bc17fdb111ba99740703ca2589327a'', @VaiTro3, N''Khoa'', NULL, N''2026-09-01T08:00:00'');
    SET @TaiKhoan3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TaiKhoan] ([MaNhanVien], [TenDangNhap], [MatKhauHash], [MaVaiTro], [TrangThai], [LanDangNhapCuoi], [NgayTao])
    VALUES (@NhanVien4, N''mau_kythuat'', N''20eeb7033e8f016431a9e6e1592e893901e036c408cca3b4a8bf0d9b662d2f3c'', @VaiTro4, N''Khoa'', NULL, N''2026-09-01T08:00:00'');
    SET @TaiKhoan4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TaiKhoan] ([MaNhanVien], [TenDangNhap], [MatKhauHash], [MaVaiTro], [TrangThai], [LanDangNhapCuoi], [NgayTao])
    VALUES (@NhanVien5, N''mau_ketoan'', N''b234c687393ec9bc357fcc4cb5cce695d0b3195c27049b6d93b5ee618a4eb89b'', @VaiTro5, N''Khoa'', NULL, N''2026-09-01T08:00:00'');
    SET @TaiKhoan5 = CONVERT(int, SCOPE_IDENTITY());

    -- 19. VaiTro_Quyen: thêm 9 dòng
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro1, @Quyen1);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro1, @Quyen2);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro1, @Quyen3);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro1, @Quyen4);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro1, @Quyen5);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro2, @Quyen2);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro3, @Quyen3);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro4, @Quyen4);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro5, @Quyen5);

    -- 20. KhachHang: thêm 5 dòng
    INSERT INTO dbo.[KhachHang] ([HoTen], [NgaySinh], [GioiTinh], [LoaiGiayTo], [SoGiayTo], [QuocTich], [SoDienThoai], [Email], [DiaChi], [GhiChu], [NgayTao], [TrangThai])
    VALUES (N''Nguyễn Văn An'', N''19910120'', N''M'', N''CCCD'', N''MAU_CCCD_0001'', N''Việt Nam'', N''0910000001'', N''khachhang1@example.com'', N''Hà Nội'', N''Thông tin giả lập, không phải giấy tờ thật'', N''2026-09-01T09:00:00'', 1);
    SET @KhachHang1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhachHang] ([HoTen], [NgaySinh], [GioiTinh], [LoaiGiayTo], [SoGiayTo], [QuocTich], [SoDienThoai], [Email], [DiaChi], [GhiChu], [NgayTao], [TrangThai])
    VALUES (N''Trần Thị Bình'', N''19920220'', N''F'', N''CCCD'', N''MAU_CCCD_0002'', N''Việt Nam'', N''0910000002'', N''khachhang2@example.com'', N''Đà Nẵng'', N''Thông tin giả lập, không phải giấy tờ thật'', N''2026-09-01T09:00:00'', 1);
    SET @KhachHang2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhachHang] ([HoTen], [NgaySinh], [GioiTinh], [LoaiGiayTo], [SoGiayTo], [QuocTich], [SoDienThoai], [Email], [DiaChi], [GhiChu], [NgayTao], [TrangThai])
    VALUES (N''Lê Hoàng Nam'', N''19930320'', N''M'', N''CCCD'', N''MAU_CCCD_0003'', N''Việt Nam'', N''0910000003'', N''khachhang3@example.com'', N''TP. Hồ Chí Minh'', N''Thông tin giả lập, không phải giấy tờ thật'', N''2026-09-01T09:00:00'', 1);
    SET @KhachHang3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhachHang] ([HoTen], [NgaySinh], [GioiTinh], [LoaiGiayTo], [SoGiayTo], [QuocTich], [SoDienThoai], [Email], [DiaChi], [GhiChu], [NgayTao], [TrangThai])
    VALUES (N''Phạm Minh Châu'', N''19940420'', N''F'', N''CCCD'', N''MAU_CCCD_0004'', N''Việt Nam'', N''0910000004'', N''khachhang4@example.com'', N''Cần Thơ'', N''Thông tin giả lập, không phải giấy tờ thật'', N''2026-09-01T09:00:00'', 1);
    SET @KhachHang4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhachHang] ([HoTen], [NgaySinh], [GioiTinh], [LoaiGiayTo], [SoGiayTo], [QuocTich], [SoDienThoai], [Email], [DiaChi], [GhiChu], [NgayTao], [TrangThai])
    VALUES (N''Võ Thanh Tùng'', N''19950520'', N''M'', N''CCCD'', N''MAU_CCCD_0005'', N''Việt Nam'', N''0910000005'', N''khachhang5@example.com'', N''Huế'', N''Thông tin giả lập, không phải giấy tờ thật'', N''2026-09-01T09:00:00'', 1);
    SET @KhachHang5 = CONVERT(int, SCOPE_IDENTITY());

    -- 21. DichVu: thêm 5 dòng
    INSERT INTO dbo.[DichVu] ([MaLoaiDichVu], [TenDichVu], [DonViTinh], [DonGia], [MoTa], [TrangThai])
    VALUES (@LoaiDichVu1, N''Buffet sáng (Mẫu)'', N''Suất'', 200000, N''Dịch vụ dành cho khách lưu trú'', 1);
    SET @DichVu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[DichVu] ([MaLoaiDichVu], [TenDichVu], [DonViTinh], [DonGia], [MoTa], [TrangThai])
    VALUES (@LoaiDichVu2, N''Giặt quần áo (Mẫu)'', N''Kg'', 80000, N''Dịch vụ dành cho khách lưu trú'', 1);
    SET @DichVu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[DichVu] ([MaLoaiDichVu], [TenDichVu], [DonViTinh], [DonGia], [MoTa], [TrangThai])
    VALUES (@LoaiDichVu3, N''Massage 60 phút (Mẫu)'', N''Lượt'', 500000, N''Dịch vụ dành cho khách lưu trú'', 1);
    SET @DichVu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[DichVu] ([MaLoaiDichVu], [TenDichVu], [DonViTinh], [DonGia], [MoTa], [TrangThai])
    VALUES (@LoaiDichVu4, N''Cà phê tại phòng (Mẫu)'', N''Ly'', 60000, N''Dịch vụ dành cho khách lưu trú'', 1);
    SET @DichVu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[DichVu] ([MaLoaiDichVu], [TenDichVu], [DonViTinh], [DonGia], [MoTa], [TrangThai])
    VALUES (@LoaiDichVu5, N''Nước suối minibar (Mẫu)'', N''Chai'', 30000, N''Dịch vụ dành cho khách lưu trú'', 1);
    SET @DichVu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 22. GoiDichVu: thêm 5 dòng
    INSERT INTO dbo.[GoiDichVu] ([TenGoiDichVu], [DonGia], [MoTa], [TrangThai])
    VALUES (N''Gói 2 suất ăn sáng (Mẫu)'', 360000, N''Giá ưu đãi khi mua theo gói'', 1);
    SET @GoiDichVu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[GoiDichVu] ([TenGoiDichVu], [DonGia], [MoTa], [TrangThai])
    VALUES (N''Gói giặt 5 kg (Mẫu)'', 350000, N''Giá ưu đãi khi mua theo gói'', 1);
    SET @GoiDichVu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[GoiDichVu] ([TenGoiDichVu], [DonGia], [MoTa], [TrangThai])
    VALUES (N''Gói 2 lượt massage (Mẫu)'', 900000, N''Giá ưu đãi khi mua theo gói'', 1);
    SET @GoiDichVu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[GoiDichVu] ([TenGoiDichVu], [DonGia], [MoTa], [TrangThai])
    VALUES (N''Gói 2 ly cà phê (Mẫu)'', 100000, N''Giá ưu đãi khi mua theo gói'', 1);
    SET @GoiDichVu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[GoiDichVu] ([TenGoiDichVu], [DonGia], [MoTa], [TrangThai])
    VALUES (N''Gói 6 chai nước (Mẫu)'', 150000, N''Giá ưu đãi khi mua theo gói'', 1);
    SET @GoiDichVu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 23. ChiTietGoiDichVu: thêm 5 dòng
    INSERT INTO dbo.[ChiTietGoiDichVu] ([MaGoiDichVu], [MaDichVu], [SoLuong], [GhiChu])
    VALUES (@GoiDichVu1, @DichVu1, 2, NULL);
    INSERT INTO dbo.[ChiTietGoiDichVu] ([MaGoiDichVu], [MaDichVu], [SoLuong], [GhiChu])
    VALUES (@GoiDichVu2, @DichVu2, 5, NULL);
    INSERT INTO dbo.[ChiTietGoiDichVu] ([MaGoiDichVu], [MaDichVu], [SoLuong], [GhiChu])
    VALUES (@GoiDichVu3, @DichVu3, 2, NULL);
    INSERT INTO dbo.[ChiTietGoiDichVu] ([MaGoiDichVu], [MaDichVu], [SoLuong], [GhiChu])
    VALUES (@GoiDichVu4, @DichVu4, 2, NULL);
    INSERT INTO dbo.[ChiTietGoiDichVu] ([MaGoiDichVu], [MaDichVu], [SoLuong], [GhiChu])
    VALUES (@GoiDichVu5, @DichVu5, 6, NULL);

    -- 24. VatTu: thêm 5 dòng
    INSERT INTO dbo.[VatTu] ([MaNhomVatTu], [MaKhoMacDinh], [TenVatTu], [DonViTinh], [MoTa], [SoLuongToiThieu], [TrangThai])
    VALUES (@NhomVatTu1, @Kho1, N''Trứng gà (Mẫu)'', N''Quả'', N''Vật tư dùng cho dịch vụ tương ứng'', 100, 1);
    SET @VatTu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VatTu] ([MaNhomVatTu], [MaKhoMacDinh], [TenVatTu], [DonViTinh], [MoTa], [SoLuongToiThieu], [TrangThai])
    VALUES (@NhomVatTu2, @Kho2, N''Nước giặt (Mẫu)'', N''Lít'', N''Vật tư dùng cho dịch vụ tương ứng'', 20, 1);
    SET @VatTu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VatTu] ([MaNhomVatTu], [MaKhoMacDinh], [TenVatTu], [DonViTinh], [MoTa], [SoLuongToiThieu], [TrangThai])
    VALUES (@NhomVatTu3, @Kho3, N''Dầu massage (Mẫu)'', N''Lít'', N''Vật tư dùng cho dịch vụ tương ứng'', 5, 1);
    SET @VatTu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VatTu] ([MaNhomVatTu], [MaKhoMacDinh], [TenVatTu], [DonViTinh], [MoTa], [SoLuongToiThieu], [TrangThai])
    VALUES (@NhomVatTu4, @Kho4, N''Cà phê hạt (Mẫu)'', N''Kg'', N''Vật tư dùng cho dịch vụ tương ứng'', 10, 1);
    SET @VatTu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VatTu] ([MaNhomVatTu], [MaKhoMacDinh], [TenVatTu], [DonViTinh], [MoTa], [SoLuongToiThieu], [TrangThai])
    VALUES (@NhomVatTu5, @Kho5, N''Nước suối 500 ml (Mẫu)'', N''Chai'', N''Vật tư dùng cho dịch vụ tương ứng'', 100, 1);
    SET @VatTu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 25. VatTu_NhaCungCap: thêm 5 dòng
    INSERT INTO dbo.[VatTu_NhaCungCap] ([MaVatTu], [MaNhaCungCap], [MaSanPhamNCC], [DonGiaNhap], [ThoiGianCungCap], [TrangThai], [GhiChu])
    VALUES (@VatTu1, @NhaCungCap1, N''MAU_VT_001'', 3000, N''20260901'', 1, NULL);
    INSERT INTO dbo.[VatTu_NhaCungCap] ([MaVatTu], [MaNhaCungCap], [MaSanPhamNCC], [DonGiaNhap], [ThoiGianCungCap], [TrangThai], [GhiChu])
    VALUES (@VatTu2, @NhaCungCap2, N''MAU_VT_002'', 40000, N''20260901'', 1, NULL);
    INSERT INTO dbo.[VatTu_NhaCungCap] ([MaVatTu], [MaNhaCungCap], [MaSanPhamNCC], [DonGiaNhap], [ThoiGianCungCap], [TrangThai], [GhiChu])
    VALUES (@VatTu3, @NhaCungCap3, N''MAU_VT_003'', 300000, N''20260901'', 1, NULL);
    INSERT INTO dbo.[VatTu_NhaCungCap] ([MaVatTu], [MaNhaCungCap], [MaSanPhamNCC], [DonGiaNhap], [ThoiGianCungCap], [TrangThai], [GhiChu])
    VALUES (@VatTu4, @NhaCungCap4, N''MAU_VT_004'', 200000, N''20260901'', 1, NULL);
    INSERT INTO dbo.[VatTu_NhaCungCap] ([MaVatTu], [MaNhaCungCap], [MaSanPhamNCC], [DonGiaNhap], [ThoiGianCungCap], [TrangThai], [GhiChu])
    VALUES (@VatTu5, @NhaCungCap5, N''MAU_VT_005'', 6000, N''20260901'', 1, NULL);

    -- 26. TieuHaoVatTu: thêm 5 dòng
    INSERT INTO dbo.[TieuHaoVatTu] ([MaDichVu], [MaVatTu], [SoLuongTieuHao], [GhiChu])
    VALUES (@DichVu1, @VatTu1, 2, N''Định mức trên một đơn vị dịch vụ'');
    INSERT INTO dbo.[TieuHaoVatTu] ([MaDichVu], [MaVatTu], [SoLuongTieuHao], [GhiChu])
    VALUES (@DichVu2, @VatTu2, 0.05, N''Định mức trên một đơn vị dịch vụ'');
    INSERT INTO dbo.[TieuHaoVatTu] ([MaDichVu], [MaVatTu], [SoLuongTieuHao], [GhiChu])
    VALUES (@DichVu3, @VatTu3, 0.03, N''Định mức trên một đơn vị dịch vụ'');
    INSERT INTO dbo.[TieuHaoVatTu] ([MaDichVu], [MaVatTu], [SoLuongTieuHao], [GhiChu])
    VALUES (@DichVu4, @VatTu4, 0.02, N''Định mức trên một đơn vị dịch vụ'');
    INSERT INTO dbo.[TieuHaoVatTu] ([MaDichVu], [MaVatTu], [SoLuongTieuHao], [GhiChu])
    VALUES (@DichVu5, @VatTu5, 1, N''Định mức trên một đơn vị dịch vụ'');

    -- 27. ThietBi: thêm 5 dòng
    INSERT INTO dbo.[ThietBi] ([MaPhong], [MaNhaCungCap], [TenThietBi], [MaThietBiNCC], [NgayMua], [HanBaoHanh], [TrangThai], [GhiChu])
    VALUES (@Phong1, @NhaCungCap5, N''Tủ lạnh mini phòng M101'', N''MAU_TL_001'', N''20250110'', N''20270110'', N''DangSuDung'', N''Đã kiểm tra sau bảo trì'');
    SET @ThietBi1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThietBi] ([MaPhong], [MaNhaCungCap], [TenThietBi], [MaThietBiNCC], [NgayMua], [HanBaoHanh], [TrangThai], [GhiChu])
    VALUES (@Phong2, @NhaCungCap5, N''Tủ lạnh mini phòng M201'', N''MAU_TL_002'', N''20250110'', N''20270110'', N''DangSuDung'', N''Đã kiểm tra sau bảo trì'');
    SET @ThietBi2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThietBi] ([MaPhong], [MaNhaCungCap], [TenThietBi], [MaThietBiNCC], [NgayMua], [HanBaoHanh], [TrangThai], [GhiChu])
    VALUES (@Phong3, @NhaCungCap5, N''Tủ lạnh mini phòng M301'', N''MAU_TL_003'', N''20250110'', N''20270110'', N''DangSuDung'', N''Đã kiểm tra sau bảo trì'');
    SET @ThietBi3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThietBi] ([MaPhong], [MaNhaCungCap], [TenThietBi], [MaThietBiNCC], [NgayMua], [HanBaoHanh], [TrangThai], [GhiChu])
    VALUES (@Phong4, @NhaCungCap5, N''Tủ lạnh mini phòng M401'', N''MAU_TL_004'', N''20250110'', N''20270110'', N''DangSuDung'', N''Đã kiểm tra sau bảo trì'');
    SET @ThietBi4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThietBi] ([MaPhong], [MaNhaCungCap], [TenThietBi], [MaThietBiNCC], [NgayMua], [HanBaoHanh], [TrangThai], [GhiChu])
    VALUES (@Phong5, @NhaCungCap5, N''Tủ lạnh mini phòng M501'', N''MAU_TL_005'', N''20250110'', N''20270110'', N''DangSuDung'', N''Đã kiểm tra sau bảo trì'');
    SET @ThietBi5 = CONVERT(int, SCOPE_IDENTITY());

    -- 28. DatPhong: thêm 5 dòng
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang1, @NhanVien2, N''2026-09-01T10:00:00'', N''DaCheckOut'', N''Đặt một phòng trong hai đêm'');
    SET @DatPhong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang2, @NhanVien2, N''2026-09-02T10:00:00'', N''DaCheckOut'', N''Đặt một phòng trong hai đêm'');
    SET @DatPhong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang3, @NhanVien2, N''2026-09-03T10:00:00'', N''DaCheckOut'', N''Đặt một phòng trong hai đêm'');
    SET @DatPhong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang4, @NhanVien2, N''2026-09-04T10:00:00'', N''DaCheckOut'', N''Đặt một phòng trong hai đêm'');
    SET @DatPhong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang5, @NhanVien2, N''2026-09-05T10:00:00'', N''DaCheckOut'', N''Đặt một phòng trong hai đêm'');
    SET @DatPhong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 29. ChiTietDatPhong: thêm 5 dòng
    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong1, @LoaiPhong1, N''2026-09-10T14:00:00'', N''2026-09-12T12:00:00'', 1, 2, 0, 800000, N''Giá phòng tính theo đêm'');
    SET @ChiTietDatPhong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong2, @LoaiPhong2, N''2026-09-10T14:00:00'', N''2026-09-12T12:00:00'', 1, 2, 0, 1200000, N''Giá phòng tính theo đêm'');
    SET @ChiTietDatPhong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong3, @LoaiPhong3, N''2026-09-10T14:00:00'', N''2026-09-12T12:00:00'', 1, 2, 0, 1800000, N''Giá phòng tính theo đêm'');
    SET @ChiTietDatPhong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong4, @LoaiPhong4, N''2026-09-10T14:00:00'', N''2026-09-12T12:00:00'', 1, 4, 0, 2500000, N''Giá phòng tính theo đêm'');
    SET @ChiTietDatPhong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong5, @LoaiPhong5, N''2026-09-10T14:00:00'', N''2026-09-12T12:00:00'', 1, 2, 0, 4000000, N''Giá phòng tính theo đêm'');
    SET @ChiTietDatPhong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 30. XepPhong: thêm 5 dòng
    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong1, @Phong1, N''2026-09-10T14:00:00'', N''2026-09-12T12:00:00'', N''DaCheckOut'', NULL);
    SET @XepPhong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong2, @Phong2, N''2026-09-10T14:00:00'', N''2026-09-12T12:00:00'', N''DaCheckOut'', NULL);
    SET @XepPhong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong3, @Phong3, N''2026-09-10T14:00:00'', N''2026-09-12T12:00:00'', N''DaCheckOut'', NULL);
    SET @XepPhong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong4, @Phong4, N''2026-09-10T14:00:00'', N''2026-09-12T12:00:00'', N''DaCheckOut'', NULL);
    SET @XepPhong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong5, @Phong5, N''2026-09-10T14:00:00'', N''2026-09-12T12:00:00'', N''DaCheckOut'', NULL);
    SET @XepPhong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 31. SuDungDichVu: thêm 5 dòng
    INSERT INTO dbo.[SuDungDichVu] ([MaDatPhong], [MaKhachHang], [MaDichVu], [MaGoiDichVu], [MaNhanVien], [SoLuong], [DonGia], [ThoiGianSuDung], [TrangThai], [GhiChu])
    VALUES (@DatPhong1, @KhachHang1, @DichVu1, NULL, @NhanVien2, 2, 200000, N''2026-09-11T10:00:00'', N''DaSuDung'', NULL);
    SET @SuDungDichVu1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[SuDungDichVu] ([MaDatPhong], [MaKhachHang], [MaDichVu], [MaGoiDichVu], [MaNhanVien], [SoLuong], [DonGia], [ThoiGianSuDung], [TrangThai], [GhiChu])
    VALUES (@DatPhong2, @KhachHang2, @DichVu2, NULL, @NhanVien2, 2, 80000, N''2026-09-11T10:00:00'', N''DaSuDung'', NULL);
    SET @SuDungDichVu2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[SuDungDichVu] ([MaDatPhong], [MaKhachHang], [MaDichVu], [MaGoiDichVu], [MaNhanVien], [SoLuong], [DonGia], [ThoiGianSuDung], [TrangThai], [GhiChu])
    VALUES (@DatPhong3, @KhachHang3, @DichVu3, NULL, @NhanVien2, 2, 500000, N''2026-09-11T10:00:00'', N''DaSuDung'', NULL);
    SET @SuDungDichVu3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[SuDungDichVu] ([MaDatPhong], [MaKhachHang], [MaDichVu], [MaGoiDichVu], [MaNhanVien], [SoLuong], [DonGia], [ThoiGianSuDung], [TrangThai], [GhiChu])
    VALUES (@DatPhong4, @KhachHang4, @DichVu4, NULL, @NhanVien2, 2, 60000, N''2026-09-11T10:00:00'', N''DaSuDung'', NULL);
    SET @SuDungDichVu4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[SuDungDichVu] ([MaDatPhong], [MaKhachHang], [MaDichVu], [MaGoiDichVu], [MaNhanVien], [SoLuong], [DonGia], [ThoiGianSuDung], [TrangThai], [GhiChu])
    VALUES (@DatPhong5, @KhachHang5, @DichVu5, NULL, @NhanVien2, 2, 30000, N''2026-09-11T10:00:00'', N''DaSuDung'', NULL);
    SET @SuDungDichVu5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 32. HoaDon: thêm 5 dòng
    INSERT INTO dbo.[HoaDon] ([MaDatPhong], [MaKhachHang], [MaUuDai], [MaNhanVienLap], [NgayLap], [TongTien], [TienGiam], [ThanhTien], [TrangThai], [GhiChu])
    VALUES (@DatPhong1, @KhachHang1, @UuDai1, @NhanVien5, N''2026-09-12T12:00:00'', 2000000, 100000, 1900000, N''DaThanhToan'', N''Hai đêm phòng và dịch vụ; giảm theo ưu đãi'');
    SET @HoaDon1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[HoaDon] ([MaDatPhong], [MaKhachHang], [MaUuDai], [MaNhanVienLap], [NgayLap], [TongTien], [TienGiam], [ThanhTien], [TrangThai], [GhiChu])
    VALUES (@DatPhong2, @KhachHang2, @UuDai2, @NhanVien5, N''2026-09-12T12:00:00'', 2560000, 256000, 2304000, N''DaThanhToan'', N''Hai đêm phòng và dịch vụ; giảm theo ưu đãi'');
    SET @HoaDon2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[HoaDon] ([MaDatPhong], [MaKhachHang], [MaUuDai], [MaNhanVienLap], [NgayLap], [TongTien], [TienGiam], [ThanhTien], [TrangThai], [GhiChu])
    VALUES (@DatPhong3, @KhachHang3, @UuDai3, @NhanVien5, N''2026-09-12T12:00:00'', 4600000, 690000, 3910000, N''DaThanhToan'', N''Hai đêm phòng và dịch vụ; giảm theo ưu đãi'');
    SET @HoaDon3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[HoaDon] ([MaDatPhong], [MaKhachHang], [MaUuDai], [MaNhanVienLap], [NgayLap], [TongTien], [TienGiam], [ThanhTien], [TrangThai], [GhiChu])
    VALUES (@DatPhong4, @KhachHang4, @UuDai4, @NhanVien5, N''2026-09-12T12:00:00'', 5120000, 1024000, 4096000, N''DaThanhToan'', N''Hai đêm phòng và dịch vụ; giảm theo ưu đãi'');
    SET @HoaDon4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[HoaDon] ([MaDatPhong], [MaKhachHang], [MaUuDai], [MaNhanVienLap], [NgayLap], [TongTien], [TienGiam], [ThanhTien], [TrangThai], [GhiChu])
    VALUES (@DatPhong5, @KhachHang5, @UuDai5, @NhanVien5, N''2026-09-12T12:00:00'', 8060000, 2015000, 6045000, N''DaThanhToan'', N''Hai đêm phòng và dịch vụ; giảm theo ưu đãi'');
    SET @HoaDon5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 33. ChiTietHoaDon: thêm 10 dòng
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon1, N''TienPhong'', @ChiTietDatPhong1, NULL, NULL, N''Tiền phòng M101: 2 đêm'', 2, 800000);
    SET @ChiTietHoaDon1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon1, N''DichVu'', NULL, @SuDungDichVu1, NULL, N''Buffet sáng'', 2, 200000);
    SET @ChiTietHoaDon2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon2, N''TienPhong'', @ChiTietDatPhong2, NULL, NULL, N''Tiền phòng M201: 2 đêm'', 2, 1200000);
    SET @ChiTietHoaDon3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon2, N''DichVu'', NULL, @SuDungDichVu2, NULL, N''Giặt quần áo'', 2, 80000);
    SET @ChiTietHoaDon4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon3, N''TienPhong'', @ChiTietDatPhong3, NULL, NULL, N''Tiền phòng M301: 2 đêm'', 2, 1800000);
    SET @ChiTietHoaDon5 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon3, N''DichVu'', NULL, @SuDungDichVu3, NULL, N''Massage 60 phút'', 2, 500000);
    SET @ChiTietHoaDon6 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon4, N''TienPhong'', @ChiTietDatPhong4, NULL, NULL, N''Tiền phòng M401: 2 đêm'', 2, 2500000);
    SET @ChiTietHoaDon7 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon4, N''DichVu'', NULL, @SuDungDichVu4, NULL, N''Cà phê tại phòng'', 2, 60000);
    SET @ChiTietHoaDon8 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon5, N''TienPhong'', @ChiTietDatPhong5, NULL, NULL, N''Tiền phòng M501: 2 đêm'', 2, 4000000);
    SET @ChiTietHoaDon9 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon5, N''DichVu'', NULL, @SuDungDichVu5, NULL, N''Nước suối minibar'', 2, 30000);
    SET @ChiTietHoaDon10 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 34. ThanhToan: thêm 5 dòng
    INSERT INTO dbo.[ThanhToan] ([MaHoaDon], [MaNhanVien], [SoTien], [ThoiGianThanhToan], [MaGiaoDich], [TrangThai], [GhiChu])
    VALUES (@HoaDon1, @NhanVien5, 1900000, N''2026-09-12T12:05:00'', N''MAU_TT_202609_001'', N''ThanhCong'', N''Thanh toán toàn bộ hóa đơn'');
    SET @ThanhToan1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThanhToan] ([MaHoaDon], [MaNhanVien], [SoTien], [ThoiGianThanhToan], [MaGiaoDich], [TrangThai], [GhiChu])
    VALUES (@HoaDon2, @NhanVien5, 2304000, N''2026-09-12T12:05:00'', N''MAU_TT_202609_002'', N''ThanhCong'', N''Thanh toán toàn bộ hóa đơn'');
    SET @ThanhToan2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThanhToan] ([MaHoaDon], [MaNhanVien], [SoTien], [ThoiGianThanhToan], [MaGiaoDich], [TrangThai], [GhiChu])
    VALUES (@HoaDon3, @NhanVien5, 3910000, N''2026-09-12T12:05:00'', N''MAU_TT_202609_003'', N''ThanhCong'', N''Thanh toán toàn bộ hóa đơn'');
    SET @ThanhToan3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThanhToan] ([MaHoaDon], [MaNhanVien], [SoTien], [ThoiGianThanhToan], [MaGiaoDich], [TrangThai], [GhiChu])
    VALUES (@HoaDon4, @NhanVien5, 4096000, N''2026-09-12T12:05:00'', N''MAU_TT_202609_004'', N''ThanhCong'', N''Thanh toán toàn bộ hóa đơn'');
    SET @ThanhToan4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThanhToan] ([MaHoaDon], [MaNhanVien], [SoTien], [ThoiGianThanhToan], [MaGiaoDich], [TrangThai], [GhiChu])
    VALUES (@HoaDon5, @NhanVien5, 6045000, N''2026-09-12T12:05:00'', N''MAU_TT_202609_005'', N''ThanhCong'', N''Thanh toán toàn bộ hóa đơn'');
    SET @ThanhToan5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 35. CongViecDonPhong: thêm 5 dòng
    INSERT INTO dbo.[CongViecDonPhong] ([MaPhong], [MaNhanVien], [NgayGiao], [LoaiCongViec], [TrangThai], [ThoiGianBatDau], [ThoiGianKetThuc], [GhiChu])
    VALUES (@Phong1, @NhanVien3, N''20260912'', N''DonPhong'', N''HoanThanh'', N''2026-09-12T13:00:00'', N''2026-09-12T13:45:00'', N''Dọn phòng sau khi khách trả phòng'');
    SET @CongViecDonPhong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[CongViecDonPhong] ([MaPhong], [MaNhanVien], [NgayGiao], [LoaiCongViec], [TrangThai], [ThoiGianBatDau], [ThoiGianKetThuc], [GhiChu])
    VALUES (@Phong2, @NhanVien3, N''20260912'', N''DonPhong'', N''HoanThanh'', N''2026-09-12T14:00:00'', N''2026-09-12T14:45:00'', N''Dọn phòng sau khi khách trả phòng'');
    SET @CongViecDonPhong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[CongViecDonPhong] ([MaPhong], [MaNhanVien], [NgayGiao], [LoaiCongViec], [TrangThai], [ThoiGianBatDau], [ThoiGianKetThuc], [GhiChu])
    VALUES (@Phong3, @NhanVien3, N''20260912'', N''DonPhong'', N''HoanThanh'', N''2026-09-12T15:00:00'', N''2026-09-12T15:45:00'', N''Dọn phòng sau khi khách trả phòng'');
    SET @CongViecDonPhong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[CongViecDonPhong] ([MaPhong], [MaNhanVien], [NgayGiao], [LoaiCongViec], [TrangThai], [ThoiGianBatDau], [ThoiGianKetThuc], [GhiChu])
    VALUES (@Phong4, @NhanVien3, N''20260912'', N''DonPhong'', N''HoanThanh'', N''2026-09-12T16:00:00'', N''2026-09-12T16:45:00'', N''Dọn phòng sau khi khách trả phòng'');
    SET @CongViecDonPhong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[CongViecDonPhong] ([MaPhong], [MaNhanVien], [NgayGiao], [LoaiCongViec], [TrangThai], [ThoiGianBatDau], [ThoiGianKetThuc], [GhiChu])
    VALUES (@Phong5, @NhanVien3, N''20260912'', N''DonPhong'', N''HoanThanh'', N''2026-09-12T17:00:00'', N''2026-09-12T17:45:00'', N''Dọn phòng sau khi khách trả phòng'');
    SET @CongViecDonPhong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 36. BaoTriPhong: thêm 5 dòng
    INSERT INTO dbo.[BaoTriPhong] ([MaPhong], [MaThietBi], [MaNhanVien], [MaNhaCungCap], [MaNguoiLienHe], [NgayYeuCau], [NoiDung], [MucDo], [TrangThai], [NgayHoanThanh], [ChiPhi], [GhiChu])
    VALUES (@Phong1, @ThietBi1, @NhanVien4, @NhaCungCap5, NULL, N''2026-09-14T08:00:00'', N''Kiểm tra và vệ sinh tủ lạnh phòng M101'', N''Thap'', N''HoanThanh'', N''2026-09-14T09:00:00'', 100000, N''Thiết bị hoạt động bình thường'');
    SET @BaoTriPhong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BaoTriPhong] ([MaPhong], [MaThietBi], [MaNhanVien], [MaNhaCungCap], [MaNguoiLienHe], [NgayYeuCau], [NoiDung], [MucDo], [TrangThai], [NgayHoanThanh], [ChiPhi], [GhiChu])
    VALUES (@Phong2, @ThietBi2, @NhanVien4, @NhaCungCap5, NULL, N''2026-09-14T08:00:00'', N''Kiểm tra và vệ sinh tủ lạnh phòng M201'', N''Thap'', N''HoanThanh'', N''2026-09-14T10:00:00'', 100000, N''Thiết bị hoạt động bình thường'');
    SET @BaoTriPhong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BaoTriPhong] ([MaPhong], [MaThietBi], [MaNhanVien], [MaNhaCungCap], [MaNguoiLienHe], [NgayYeuCau], [NoiDung], [MucDo], [TrangThai], [NgayHoanThanh], [ChiPhi], [GhiChu])
    VALUES (@Phong3, @ThietBi3, @NhanVien4, @NhaCungCap5, NULL, N''2026-09-14T08:00:00'', N''Kiểm tra và vệ sinh tủ lạnh phòng M301'', N''Thap'', N''HoanThanh'', N''2026-09-14T11:00:00'', 100000, N''Thiết bị hoạt động bình thường'');
    SET @BaoTriPhong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BaoTriPhong] ([MaPhong], [MaThietBi], [MaNhanVien], [MaNhaCungCap], [MaNguoiLienHe], [NgayYeuCau], [NoiDung], [MucDo], [TrangThai], [NgayHoanThanh], [ChiPhi], [GhiChu])
    VALUES (@Phong4, @ThietBi4, @NhanVien4, @NhaCungCap5, NULL, N''2026-09-14T08:00:00'', N''Kiểm tra và vệ sinh tủ lạnh phòng M401'', N''Thap'', N''HoanThanh'', N''2026-09-14T12:00:00'', 100000, N''Thiết bị hoạt động bình thường'');
    SET @BaoTriPhong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BaoTriPhong] ([MaPhong], [MaThietBi], [MaNhanVien], [MaNhaCungCap], [MaNguoiLienHe], [NgayYeuCau], [NoiDung], [MucDo], [TrangThai], [NgayHoanThanh], [ChiPhi], [GhiChu])
    VALUES (@Phong5, @ThietBi5, @NhanVien4, @NhaCungCap5, NULL, N''2026-09-14T08:00:00'', N''Kiểm tra và vệ sinh tủ lạnh phòng M501'', N''Thap'', N''HoanThanh'', N''2026-09-14T13:00:00'', 100000, N''Thiết bị hoạt động bình thường'');
    SET @BaoTriPhong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 37. ChamCong: thêm 5 dòng
    INSERT INTO dbo.[ChamCong] ([MaLich], [MaNhanVien], [NgayChamCong], [GioVao], [GioRa], [SoGioLam], [SoGioTangCa], [TrangThai], [GhiChu])
    VALUES (@LichLamViec1, @NhanVien1, N''20260920'', N''2026-09-20T08:00:00'', N''2026-09-20T17:00:00'', 8, 0, N''DuCong'', N''Ca hành chính; nghỉ trưa một giờ'');
    SET @ChamCong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChamCong] ([MaLich], [MaNhanVien], [NgayChamCong], [GioVao], [GioRa], [SoGioLam], [SoGioTangCa], [TrangThai], [GhiChu])
    VALUES (@LichLamViec2, @NhanVien2, N''20260920'', N''2026-09-20T08:00:00'', N''2026-09-20T17:00:00'', 8, 0, N''DuCong'', N''Ca hành chính; nghỉ trưa một giờ'');
    SET @ChamCong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChamCong] ([MaLich], [MaNhanVien], [NgayChamCong], [GioVao], [GioRa], [SoGioLam], [SoGioTangCa], [TrangThai], [GhiChu])
    VALUES (@LichLamViec3, @NhanVien3, N''20260920'', N''2026-09-20T08:00:00'', N''2026-09-20T17:00:00'', 8, 0, N''DuCong'', N''Ca hành chính; nghỉ trưa một giờ'');
    SET @ChamCong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChamCong] ([MaLich], [MaNhanVien], [NgayChamCong], [GioVao], [GioRa], [SoGioLam], [SoGioTangCa], [TrangThai], [GhiChu])
    VALUES (@LichLamViec4, @NhanVien4, N''20260920'', N''2026-09-20T08:00:00'', N''2026-09-20T17:00:00'', 8, 0, N''DuCong'', N''Ca hành chính; nghỉ trưa một giờ'');
    SET @ChamCong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChamCong] ([MaLich], [MaNhanVien], [NgayChamCong], [GioVao], [GioRa], [SoGioLam], [SoGioTangCa], [TrangThai], [GhiChu])
    VALUES (@LichLamViec5, @NhanVien5, N''20260920'', N''2026-09-20T08:00:00'', N''2026-09-20T17:00:00'', 8, 0, N''DuCong'', N''Ca hành chính; nghỉ trưa một giờ'');
    SET @ChamCong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 38. LichLamViec: thêm 5 dòng
    SET IDENTITY_INSERT dbo.LichLamViec ON;
    SET @DangBatIdentityInsert = 1;
    INSERT INTO dbo.[LichLamViec] ([MaLich], [MaNhanVien], [MaCa], [NgayLam], [TrangThai], [MaChamCong], [GhiChu])
    VALUES (@LichLamViec1, @NhanVien1, @CaLamViec4, N''20260920'', N''DaXep'', @ChamCong1, N''Lịch mẫu khớp với bản ghi chấm công'');
    INSERT INTO dbo.[LichLamViec] ([MaLich], [MaNhanVien], [MaCa], [NgayLam], [TrangThai], [MaChamCong], [GhiChu])
    VALUES (@LichLamViec2, @NhanVien2, @CaLamViec4, N''20260920'', N''DaXep'', @ChamCong2, N''Lịch mẫu khớp với bản ghi chấm công'');
    INSERT INTO dbo.[LichLamViec] ([MaLich], [MaNhanVien], [MaCa], [NgayLam], [TrangThai], [MaChamCong], [GhiChu])
    VALUES (@LichLamViec3, @NhanVien3, @CaLamViec4, N''20260920'', N''DaXep'', @ChamCong3, N''Lịch mẫu khớp với bản ghi chấm công'');
    INSERT INTO dbo.[LichLamViec] ([MaLich], [MaNhanVien], [MaCa], [NgayLam], [TrangThai], [MaChamCong], [GhiChu])
    VALUES (@LichLamViec4, @NhanVien4, @CaLamViec4, N''20260920'', N''DaXep'', @ChamCong4, N''Lịch mẫu khớp với bản ghi chấm công'');
    INSERT INTO dbo.[LichLamViec] ([MaLich], [MaNhanVien], [MaCa], [NgayLam], [TrangThai], [MaChamCong], [GhiChu])
    VALUES (@LichLamViec5, @NhanVien5, @CaLamViec4, N''20260920'', N''DaXep'', @ChamCong5, N''Lịch mẫu khớp với bản ghi chấm công'');
    SET IDENTITY_INSERT dbo.LichLamViec OFF;
    SET @DangBatIdentityInsert = 0;

    -- 39. BangLuong: thêm 5 dòng
    INSERT INTO dbo.[BangLuong] ([MaNhanVien], [Thang], [Nam], [LuongCoBan], [TienCong], [TienTangCa], [TienThuong], [TienPhuCap], [TienKhauTru], [Thue], [TongLuong], [ThucLinh], [NgayLap], [TrangThai], [GhiChu])
    VALUES (@NhanVien1, 9, 2026, 20000000, 20000000, 0, 500000, 700000, 0, 0, 21200000, 21200000, N''2026-09-30T16:00:00'', N''TamTinh'', N''Mẫu tạm tính: TienCong là lương thực tế; TongLuong = TienCong + TienTangCa + TienThuong + TienPhuCap. Thue chưa tính.'');
    SET @BangLuong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BangLuong] ([MaNhanVien], [Thang], [Nam], [LuongCoBan], [TienCong], [TienTangCa], [TienThuong], [TienPhuCap], [TienKhauTru], [Thue], [TongLuong], [ThucLinh], [NgayLap], [TrangThai], [GhiChu])
    VALUES (@NhanVien2, 9, 2026, 9000000, 9000000, 0, 500000, 700000, 0, 0, 10200000, 10200000, N''2026-09-30T16:00:00'', N''TamTinh'', N''Mẫu tạm tính: TienCong là lương thực tế; TongLuong = TienCong + TienTangCa + TienThuong + TienPhuCap. Thue chưa tính.'');
    SET @BangLuong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BangLuong] ([MaNhanVien], [Thang], [Nam], [LuongCoBan], [TienCong], [TienTangCa], [TienThuong], [TienPhuCap], [TienKhauTru], [Thue], [TongLuong], [ThucLinh], [NgayLap], [TrangThai], [GhiChu])
    VALUES (@NhanVien3, 9, 2026, 8000000, 8000000, 0, 500000, 700000, 0, 0, 9200000, 9200000, N''2026-09-30T16:00:00'', N''TamTinh'', N''Mẫu tạm tính: TienCong là lương thực tế; TongLuong = TienCong + TienTangCa + TienThuong + TienPhuCap. Thue chưa tính.'');
    SET @BangLuong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BangLuong] ([MaNhanVien], [Thang], [Nam], [LuongCoBan], [TienCong], [TienTangCa], [TienThuong], [TienPhuCap], [TienKhauTru], [Thue], [TongLuong], [ThucLinh], [NgayLap], [TrangThai], [GhiChu])
    VALUES (@NhanVien4, 9, 2026, 11000000, 11000000, 0, 500000, 700000, 0, 0, 12200000, 12200000, N''2026-09-30T16:00:00'', N''TamTinh'', N''Mẫu tạm tính: TienCong là lương thực tế; TongLuong = TienCong + TienTangCa + TienThuong + TienPhuCap. Thue chưa tính.'');
    SET @BangLuong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BangLuong] ([MaNhanVien], [Thang], [Nam], [LuongCoBan], [TienCong], [TienTangCa], [TienThuong], [TienPhuCap], [TienKhauTru], [Thue], [TongLuong], [ThucLinh], [NgayLap], [TrangThai], [GhiChu])
    VALUES (@NhanVien5, 9, 2026, 12000000, 12000000, 0, 500000, 700000, 0, 0, 13200000, 13200000, N''2026-09-30T16:00:00'', N''TamTinh'', N''Mẫu tạm tính: TienCong là lương thực tế; TongLuong = TienCong + TienTangCa + TienThuong + TienPhuCap. Thue chưa tính.'');
    SET @BangLuong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 40. NhatKyThaoTac: thêm 5 dòng
    INSERT INTO dbo.[NhatKyThaoTac] ([MaTaiKhoan], [HanhDong], [DoiTuong], [MaBanGhi], [ThoiGian], [MoTa])
    VALUES (@TaiKhoan1, N''TaoDuLieuMau'', N''NhanVien'', @NhanVien1, N''2026-09-01T08:30:00'', N''Nhật ký giả lập phục vụ kiểm thử giao diện'');
    SET @NhatKyThaoTac1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhatKyThaoTac] ([MaTaiKhoan], [HanhDong], [DoiTuong], [MaBanGhi], [ThoiGian], [MoTa])
    VALUES (@TaiKhoan2, N''TaoDuLieuMau'', N''NhanVien'', @NhanVien2, N''2026-09-01T08:30:00'', N''Nhật ký giả lập phục vụ kiểm thử giao diện'');
    SET @NhatKyThaoTac2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhatKyThaoTac] ([MaTaiKhoan], [HanhDong], [DoiTuong], [MaBanGhi], [ThoiGian], [MoTa])
    VALUES (@TaiKhoan3, N''TaoDuLieuMau'', N''NhanVien'', @NhanVien3, N''2026-09-01T08:30:00'', N''Nhật ký giả lập phục vụ kiểm thử giao diện'');
    SET @NhatKyThaoTac3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhatKyThaoTac] ([MaTaiKhoan], [HanhDong], [DoiTuong], [MaBanGhi], [ThoiGian], [MoTa])
    VALUES (@TaiKhoan4, N''TaoDuLieuMau'', N''NhanVien'', @NhanVien4, N''2026-09-01T08:30:00'', N''Nhật ký giả lập phục vụ kiểm thử giao diện'');
    SET @NhatKyThaoTac4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhatKyThaoTac] ([MaTaiKhoan], [HanhDong], [DoiTuong], [MaBanGhi], [ThoiGian], [MoTa])
    VALUES (@TaiKhoan5, N''TaoDuLieuMau'', N''NhanVien'', @NhanVien5, N''2026-09-01T08:30:00'', N''Nhật ký giả lập phục vụ kiểm thử giao diện'');
    SET @NhatKyThaoTac5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- Bổ sung 5 đơn mới đã check-in (không thay đổi các đơn đã trả phòng).
    -- Khách 1, phòng M101: nhận 10/10, dự kiến trả 12/10/2026.
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang1, @NhanVien2, ''2026-10-10T13:00:00'', ''DaCheckIn'', N''MAU_CHECKIN_BO_SUNG_20261010'');
    SET @DatPhong6 = CONVERT(bigint, SCOPE_IDENTITY());

    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong6, @LoaiPhong1, ''2026-10-10T14:00:00'', ''2026-10-12T12:00:00'', 1, 2, 0, 800000, N''Giá phòng tính theo đêm'');
    SET @ChiTietDatPhong6 = CONVERT(bigint, SCOPE_IDENTITY());

    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong6, @Phong1, ''2026-10-10T14:00:00'', NULL, ''DaCheckIn'', N''MAU_CHECKIN_BO_SUNG_20261010'');
    SET @XepPhong6 = CONVERT(bigint, SCOPE_IDENTITY());

    -- Khách 2, phòng M201: nhận 10/10, dự kiến trả 12/10/2026.
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang2, @NhanVien2, ''2026-10-10T13:00:00'', ''DaCheckIn'', N''MAU_CHECKIN_BO_SUNG_20261010'');
    SET @DatPhong7 = CONVERT(bigint, SCOPE_IDENTITY());

    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong7, @LoaiPhong2, ''2026-10-10T14:00:00'', ''2026-10-12T12:00:00'', 1, 2, 0, 1200000, N''Giá phòng tính theo đêm'');
    SET @ChiTietDatPhong7 = CONVERT(bigint, SCOPE_IDENTITY());

    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong7, @Phong2, ''2026-10-10T14:00:00'', NULL, ''DaCheckIn'', N''MAU_CHECKIN_BO_SUNG_20261010'');
    SET @XepPhong7 = CONVERT(bigint, SCOPE_IDENTITY());

    -- Khách 3, phòng M301: nhận 10/10, dự kiến trả 12/10/2026.
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang3, @NhanVien2, ''2026-10-10T13:00:00'', ''DaCheckIn'', N''MAU_CHECKIN_BO_SUNG_20261010'');
    SET @DatPhong8 = CONVERT(bigint, SCOPE_IDENTITY());

    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong8, @LoaiPhong3, ''2026-10-10T14:00:00'', ''2026-10-12T12:00:00'', 1, 2, 0, 1800000, N''Giá phòng tính theo đêm'');
    SET @ChiTietDatPhong8 = CONVERT(bigint, SCOPE_IDENTITY());

    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong8, @Phong3, ''2026-10-10T14:00:00'', NULL, ''DaCheckIn'', N''MAU_CHECKIN_BO_SUNG_20261010'');
    SET @XepPhong8 = CONVERT(bigint, SCOPE_IDENTITY());

    -- Khách 4, phòng M401: nhận 10/10, dự kiến trả 12/10/2026.
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang4, @NhanVien2, ''2026-10-10T13:00:00'', ''DaCheckIn'', N''MAU_CHECKIN_BO_SUNG_20261010'');
    SET @DatPhong9 = CONVERT(bigint, SCOPE_IDENTITY());

    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong9, @LoaiPhong4, ''2026-10-10T14:00:00'', ''2026-10-12T12:00:00'', 1, 4, 0, 2500000, N''Giá phòng tính theo đêm'');
    SET @ChiTietDatPhong9 = CONVERT(bigint, SCOPE_IDENTITY());

    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong9, @Phong4, ''2026-10-10T14:00:00'', NULL, ''DaCheckIn'', N''MAU_CHECKIN_BO_SUNG_20261010'');
    SET @XepPhong9 = CONVERT(bigint, SCOPE_IDENTITY());

    -- Khách 5, phòng M501: nhận 10/10, dự kiến trả 12/10/2026.
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang5, @NhanVien2, ''2026-10-10T13:00:00'', ''DaCheckIn'', N''MAU_CHECKIN_BO_SUNG_20261010'');
    SET @DatPhong10 = CONVERT(bigint, SCOPE_IDENTITY());

    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong10, @LoaiPhong5, ''2026-10-10T14:00:00'', ''2026-10-12T12:00:00'', 1, 2, 0, 4000000, N''Giá phòng tính theo đêm'');
    SET @ChiTietDatPhong10 = CONVERT(bigint, SCOPE_IDENTITY());

    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong10, @Phong5, ''2026-10-10T14:00:00'', NULL, ''DaCheckIn'', N''MAU_CHECKIN_BO_SUNG_20261010'');
    SET @XepPhong10 = CONVERT(bigint, SCOPE_IDENTITY());

    COMMIT TRANSACTION;
    PRINT N''Đã thêm thành công bộ dữ liệu mẫu vào 40 bảng.'';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    IF @DangBatIdentityInsert = 1 SET IDENTITY_INSERT dbo.LichLamViec OFF;
    THROW;
END CATCH;';
    END
    ELSE
        PRINT N'Đã có cấu trúc DB: giữ dữ liệu cũ và bỏ qua nạp mẫu.';

-- ==================== 3. CẤU TRÚC GIẤY TỜ KHÁCH HÀNG ====================
EXEC sys.sp_executesql N'-- Chọn đúng database trong SSMS rồi chạy toàn bộ file (SQL Server 2017 trở lên).
-- Cập nhật cấu trúc giấy tờ, sau đó tạo các proc đọc nhiều bảng.
-- Khách hàng và danh mục ComboBox dùng HienThiTatCa<T>.


-- LoaiGiayTo là tên loại được chọn; SoGiayTo lưu mã số giấy tờ dạng chuỗi; BUS kiểm tra theo loại.
-- Giữ bước chuyển đổi schema từ các bản trước; không bổ sung quy tắc duy nhất trong bản này.
-- Không xóa dữ liệu khách hàng hoặc số giấy tờ đã lưu trước đây bằng script này.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @BoRangBuoc nvarchar(max) = N'''';
SELECT @BoRangBuoc = @BoRangBuoc +
    CASE WHEN i.is_unique_constraint = 1
         THEN N''ALTER TABLE dbo.KhachHang DROP CONSTRAINT '' + QUOTENAME(i.name) + N'';''
         ELSE N''DROP INDEX '' + QUOTENAME(i.name) + N'' ON dbo.KhachHang;'' END
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID(N''dbo.KhachHang'')
  AND i.is_unique = 1 AND i.is_primary_key = 0
  AND (SELECT COUNT(*) FROM sys.index_columns ic
       WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.key_ordinal > 0) = 2
  AND (SELECT COUNT(*) FROM sys.index_columns ic
       JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
       WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.key_ordinal > 0
         AND c.name IN (N''LoaiGiayTo'', N''SoGiayTo'')) = 2;
IF @BoRangBuoc <> N'''' EXEC sys.sp_executesql @BoRangBuoc;
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N''dbo.KhachHang'') AND name = N''LoaiGiayTo''
             AND (system_type_id <> TYPE_ID(N''nvarchar'') OR max_length <> 40))
    ALTER TABLE dbo.KhachHang ALTER COLUMN LoaiGiayTo nvarchar(20) NULL;
COMMIT TRANSACTION;';
-- ==================== 4. THỦ TỤC LỄ TÂN VÀ HỆ THỐNG ====================
IF OBJECT_ID(N'dbo.sp_HienThiDatPhong',N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.sp_HienThiDatPhong AS RETURN;');
EXEC sys.sp_executesql N'ALTER PROCEDURE dbo.sp_HienThiDatPhong
AS
BEGIN
    SET NOCOUNT ON;

    SELECT dp.MaDatPhong,
           kh.HoTen AS HoTen,
           MIN(ct.NgayNhanDuKien) AS NgayNhanDuKien,
           MAX(ct.NgayTraDuKien) AS NgayTraDuKien,
           STRING_AGG(p.SoPhong, N'', '') AS SoPhong,
           CONCAT(
               ISNULL(SUM(ct.NguoiLon), 0), N'' lớn + '',
               ISNULL(SUM(ct.TreEm), 0), N'' trẻ''
           ) AS SoKhach,
           dp.TrangThai
    FROM dbo.DatPhong dp
    LEFT JOIN dbo.KhachHang kh ON dp.MaKhachHangDat = kh.MaKhachHang
    LEFT JOIN dbo.ChiTietDatPhong ct ON dp.MaDatPhong = ct.MaDatPhong
    LEFT JOIN
    (
        SELECT xp.MaChiTietDatPhong,
               STRING_AGG(CAST(ph.SoPhong AS nvarchar(max)), N'', '') AS SoPhong
        FROM dbo.XepPhong xp
        JOIN dbo.Phong ph ON xp.MaPhong = ph.MaPhong
        WHERE xp.TrangThai <> ''Huy''
        GROUP BY xp.MaChiTietDatPhong
    ) p ON ct.MaChiTietDatPhong = p.MaChiTietDatPhong
    GROUP BY dp.MaDatPhong, kh.HoTen, dp.TrangThai
    ORDER BY dp.MaDatPhong DESC;
END;';
IF OBJECT_ID(N'dbo.sp_HienThiPhanBoPhong',N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.sp_HienThiPhanBoPhong AS RETURN;');
EXEC sys.sp_executesql N'-- 2. Phân bổ phòng. Số khách thuộc chi tiết đặt phòng, có thể lặp giữa các phòng.
ALTER PROCEDURE dbo.sp_HienThiPhanBoPhong
AS
BEGIN
    SET NOCOUNT ON;

    SELECT xp.MaXepPhong, xp.MaChiTietDatPhong, xp.MaPhong, ct.MaDatPhong,
           p.SoPhong,
           lp.TenLoaiPhong,
           ct.NguoiLon,
           ct.TreEm,
           ct.DonGiaDuKien,
           xp.TrangThai
    FROM dbo.XepPhong xp
    JOIN dbo.ChiTietDatPhong ct ON xp.MaChiTietDatPhong = ct.MaChiTietDatPhong
    JOIN dbo.DatPhong dp ON ct.MaDatPhong = dp.MaDatPhong
    JOIN dbo.Phong p ON xp.MaPhong = p.MaPhong
    JOIN dbo.LoaiPhong lp ON p.MaLoaiPhong = lp.MaLoaiPhong
    WHERE dp.TrangThai <> ''DaHuy'' AND xp.TrangThai <> ''Huy''
    ORDER BY xp.MaXepPhong DESC;
END;';
IF OBJECT_ID(N'dbo.sp_HienThiKhachLuuTru',N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.sp_HienThiKhachLuuTru AS RETURN;');
EXEC sys.sp_executesql N'-- 3. Khách theo phòng: chỉ lượt đang ở. Họ tên/giấy tờ là của người đặt phòng.
ALTER PROCEDURE dbo.sp_HienThiKhachLuuTru
AS
BEGIN
    SET NOCOUNT ON;

    SELECT xp.MaXepPhong, xp.MaChiTietDatPhong, xp.MaPhong,
           ct.MaDatPhong,
           p.SoPhong,
           kh.HoTen,
           kh.LoaiGiayTo,
           kh.SoGiayTo,
           xp.NgayNhanThucTe,
           xp.NgayTraThucTe
    FROM dbo.XepPhong xp
    JOIN dbo.ChiTietDatPhong ct ON xp.MaChiTietDatPhong = ct.MaChiTietDatPhong
    JOIN dbo.DatPhong dp ON ct.MaDatPhong = dp.MaDatPhong
    JOIN dbo.KhachHang kh ON dp.MaKhachHangDat = kh.MaKhachHang
    JOIN dbo.Phong p ON xp.MaPhong = p.MaPhong
    WHERE xp.TrangThai = ''DaCheckIn''
    ORDER BY xp.NgayNhanThucTe DESC, xp.MaXepPhong DESC;
END;';
IF OBJECT_ID(N'dbo.sp_HienThiLichSuLuuTru',N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.sp_HienThiLichSuLuuTru AS RETURN;');
EXEC sys.sp_executesql N'-- 4. Lịch sử xếp phòng: mọi trạng thái, ngày thực tế có thể NULL.
ALTER PROCEDURE dbo.sp_HienThiLichSuLuuTru
AS
BEGIN
    SET NOCOUNT ON;

    SELECT xp.MaXepPhong, xp.MaChiTietDatPhong, xp.MaPhong, xp.TrangThai, kh.HoTen,
           ct.MaDatPhong,
           p.SoPhong,
           xp.NgayNhanThucTe,
           xp.NgayTraThucTe
    FROM dbo.XepPhong xp
    JOIN dbo.ChiTietDatPhong ct ON xp.MaChiTietDatPhong = ct.MaChiTietDatPhong
    JOIN dbo.DatPhong dp ON ct.MaDatPhong = dp.MaDatPhong
    JOIN dbo.KhachHang kh ON dp.MaKhachHangDat = kh.MaKhachHang
    JOIN dbo.Phong p ON xp.MaPhong = p.MaPhong
    ORDER BY xp.MaXepPhong DESC;
END;';
IF OBJECT_ID(N'dbo.sp_HienThiHoaDon',N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.sp_HienThiHoaDon AS RETURN;');
EXEC sys.sp_executesql N'-- 5. Hóa đơn: chỉ cộng phiếu thu ThanhCong. Hóa đơn hủy hoặc đã thu đủ có ConThieu = 0.
ALTER PROCEDURE dbo.sp_HienThiHoaDon
AS
BEGIN
    SET NOCOUNT ON;

    SELECT hd.MaHoaDon,
           hd.MaDatPhong,
           kh.HoTen,
           hd.TongTien,
           hd.TienGiam,
           hd.ThanhTien,
           ISNULL(tt.DaThu, 0) AS DaThu,
           CASE
               WHEN hd.TrangThai = ''Huy'' THEN 0
               WHEN hd.ThanhTien > ISNULL(tt.DaThu, 0)
                   THEN hd.ThanhTien - ISNULL(tt.DaThu, 0)
               ELSE 0
           END AS ConThieu,
           hd.TrangThai
    FROM dbo.HoaDon hd
    JOIN dbo.KhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
    LEFT JOIN
    (
        SELECT MaHoaDon, SUM(SoTien) AS DaThu
        FROM dbo.ThanhToan
        WHERE TrangThai = ''ThanhCong''
        GROUP BY MaHoaDon
    ) tt ON hd.MaHoaDon = tt.MaHoaDon
    ORDER BY hd.NgayLap DESC, hd.MaHoaDon DESC;
END;';
IF OBJECT_ID(N'dbo.sp_HienThiLichSuThuTien',N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.sp_HienThiLichSuThuTien AS RETURN;');
EXEC sys.sp_executesql N'-- 6. Lịch sử thu tiền: mọi trạng thái; HoTen là nhân viên thu tiền.
ALTER PROCEDURE dbo.sp_HienThiLichSuThuTien
AS
BEGIN
    SET NOCOUNT ON;

    SELECT tt.MaThanhToan,
           tt.MaHoaDon, tt.MaNhanVien, tt.TrangThai,
           tt.ThoiGianThanhToan,
           tt.SoTien,
           nv.HoTen,
           tt.GhiChu
    FROM dbo.ThanhToan tt
    LEFT JOIN dbo.NhanVien nv ON tt.MaNhanVien = nv.MaNhanVien
    ORDER BY tt.ThoiGianThanhToan DESC, tt.MaThanhToan DESC;
END;';
IF OBJECT_ID(N'dbo.sp_DatPhongChuaLapHoaDon',N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.sp_DatPhongChuaLapHoaDon AS RETURN;');
EXEC sys.sp_executesql N'-- Đơn chưa hủy và chưa từng lập hóa đơn: đối chiếu DatPhong với HoaDon.
ALTER PROCEDURE dbo.sp_DatPhongChuaLapHoaDon
AS
BEGIN
    SET NOCOUNT ON;
    SELECT dp.*
    FROM dbo.DatPhong dp
    WHERE dp.TrangThai <> ''DaHuy''
      AND NOT EXISTS (SELECT 1 FROM dbo.HoaDon hd WHERE hd.MaDatPhong = dp.MaDatPhong)
    ORDER BY dp.MaDatPhong DESC;
END;';
IF OBJECT_ID(N'dbo.sp_KhoanThuDatPhong',N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.sp_KhoanThuDatPhong AS RETURN;');
EXEC sys.sp_executesql N'-- Khoản thu: tiền phòng và các dịch vụ đã sử dụng của đơn đặt phòng.
ALTER PROCEDURE dbo.sp_KhoanThuDatPhong @MaDatPhong bigint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CAST(''TienPhong'' AS varchar(20)) AS LoaiChiPhi,
           ct.MaChiTietDatPhong, CAST(NULL AS bigint) AS MaSuDungDichVu,
           CAST(NULL AS int) AS MaGoiDichVu,
           CAST(CONCAT(lp.TenLoaiPhong, N'' — '', ct.SoLuongPhong, N'' phòng / '',
                DATEDIFF(day, ct.NgayNhanDuKien, ct.NgayTraDuKien), N'' đêm'') AS nvarchar(300)) AS NoiDung,
           CAST(ct.SoLuongPhong * CASE WHEN DATEDIFF(day, ct.NgayNhanDuKien, ct.NgayTraDuKien) < 1
                     THEN 1 ELSE DATEDIFF(day, ct.NgayNhanDuKien, ct.NgayTraDuKien) END AS decimal(12,2)) AS SoLuong,
           ct.DonGiaDuKien AS DonGia
    FROM dbo.ChiTietDatPhong ct JOIN dbo.LoaiPhong lp ON ct.MaLoaiPhong = lp.MaLoaiPhong
    WHERE ct.MaDatPhong = @MaDatPhong
    UNION ALL
    SELECT CASE WHEN sd.MaGoiDichVu IS NULL THEN ''DichVu'' ELSE ''GoiDichVu'' END,
           NULL, sd.MaSuDungDichVu, sd.MaGoiDichVu,
           CAST(COALESCE(dv.TenDichVu, gd.TenGoiDichVu) AS nvarchar(300)), sd.SoLuong, sd.DonGia
    FROM dbo.SuDungDichVu sd
    LEFT JOIN dbo.DichVu dv ON sd.MaDichVu = dv.MaDichVu
    LEFT JOIN dbo.GoiDichVu gd ON sd.MaGoiDichVu = gd.MaGoiDichVu
    WHERE sd.MaDatPhong = @MaDatPhong AND sd.TrangThai = ''DaSuDung'';
END;';
IF OBJECT_ID(N'dbo.sp_ChiTietDatPhong',N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.sp_ChiTietDatPhong AS RETURN;');
EXEC sys.sp_executesql N'-- Các lệnh xem thử (bỏ dấu -- nếu cần):
-- EXEC dbo.sp_HienThiDatPhong;
-- EXEC dbo.sp_HienThiPhanBoPhong;
-- EXEC dbo.sp_HienThiKhachLuuTru;
-- EXEC dbo.sp_HienThiLichSuLuuTru;
-- EXEC dbo.sp_HienThiHoaDon;
-- EXEC dbo.sp_HienThiLichSuThuTien;

ALTER PROCEDURE dbo.sp_ChiTietDatPhong @MaDatPhong bigint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ct.*, lp.TenLoaiPhong
    FROM dbo.ChiTietDatPhong ct
    JOIN dbo.LoaiPhong lp ON ct.MaLoaiPhong = lp.MaLoaiPhong
    WHERE ct.MaDatPhong = @MaDatPhong
    ORDER BY ct.MaChiTietDatPhong;
END;';
IF OBJECT_ID(N'dbo.sp_HienThitaiKhoang',N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.sp_HienThitaiKhoang AS RETURN;');
EXEC sys.sp_executesql N'ALTER PROCEDURE dbo.sp_HienThitaiKhoang
AS
BEGIN
    SET NOCOUNT ON;
    Select TK.MaTaiKhoan, NV.HoTen, TK.TenDangNhap ,Tk.LanDangNhapCuoi,TK.TrangThai,VT.TenVaiTro
    from TaiKhoan TK 
    join NhanVien NV on TK.MaNhanVien=NV.MaNhanVien
    join VaiTro VT on TK.MaVaiTro=VT.MaVaiTro
END;';
-- ==================== 5. NÂNG CẤP NHÂN SỰ ====================
/*
    Nâng cấp trực tiếp DB hiện có, không tạo lại database và không xóa dữ liệu.
    Mở trong SSMS rồi Execute toàn bộ file. Đổi dòng USE nếu DB có tên khác.
    Có thể chạy lại. Mọi thay đổi cấu trúc được thực hiện trong một transaction.

    MaLich cũ được giữ nguyên; mã mới dùng DEFAULT + SEQUENCE để tránh dựng lại bảng.
    Cột LichLamViec.MaChamCong cũ chỉ giữ tương thích dữ liệu/ET, không dùng làm liên kết.
    Quan hệ thực tế: ChamCong.MaLich -> LichLamViec.MaLich.
    Khi thêm lịch: gọi usp_ThemLichLamViec hoặc INSERT bỏ qua MaLich để DEFAULT sinh mã.
    Không dùng SCOPE_IDENTITY() để lấy MaLich; dùng OUTPUT INSERTED.MaLich.

    KhenThuong lưu chi tiết. BangLuong.TienThuong vẫn là tổng thưởng đã chốt của kỳ lương;
    script không tự cộng tiền thưởng hoặc sửa các bảng lương cũ.
    Script chuẩn bị DB; các tab đang dùng dữ liệu mẫu vẫn cần nối DAL/BUS/giao diện.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;


BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Khoa int;
    EXEC @Khoa = sys.sp_getapplock
        @Resource = N'QLKS_CapNhat_NhanSu', @LockMode = N'Exclusive',
        @LockOwner = N'Transaction', @LockTimeout = 15000;
    IF @Khoa < 0
        THROW 51001, N'Có phiên khác đang nâng cấp Nhân sự. Hãy chạy lại sau.', 1;

    IF OBJECT_ID(N'dbo.PhongBan', N'U') IS NULL
        OR OBJECT_ID(N'dbo.ChucVu', N'U') IS NULL
        OR OBJECT_ID(N'dbo.NhanVien', N'U') IS NULL
        OR OBJECT_ID(N'dbo.CaLamViec', N'U') IS NULL
        OR OBJECT_ID(N'dbo.LichLamViec', N'U') IS NULL
        OR OBJECT_ID(N'dbo.ChamCong', N'U') IS NULL
        OR OBJECT_ID(N'dbo.BangLuong', N'U') IS NULL
        THROW 51002, N'DB thiếu các bảng Nhân sự gốc. Kiểm tra lại database ở dòng USE.', 1;

    DECLARE @MaLichLonNhat bigint, @SoChamCong bigint;
    SELECT @MaLichLonNhat = ISNULL(MAX(MaLich), 0)
    FROM dbo.LichLamViec WITH (TABLOCKX, HOLDLOCK);
    SELECT @SoChamCong = COUNT_BIG(*) FROM dbo.ChamCong WITH (TABLOCKX, HOLDLOCK);

    IF EXISTS (
        SELECT 1 FROM dbo.ChamCong c
        LEFT JOIN dbo.LichLamViec l ON l.MaLich = c.MaLich
        WHERE l.MaLich IS NULL
            OR c.MaNhanVien <> l.MaNhanVien OR c.NgayChamCong <> l.NgayLam
    )
        THROW 51003, N'Có chấm công thiếu lịch hoặc khác nhân viên/ngày của lịch. Chưa thay đổi DB; cần đối chiếu các dòng này trước.', 1;

    IF EXISTS (SELECT 1 FROM dbo.ChamCong
        WHERE GioRa IS NOT NULL AND (GioVao IS NULL OR GioRa < GioVao))
        THROW 51004, N'Có giờ ra trước giờ vào hoặc thiếu giờ vào. Chưa thay đổi DB; cần kiểm tra dữ liệu chấm công.', 1;

    -- Bỏ liên kết ngược, kể cả khi tên khóa ngoại đã được SQL Server tự sinh.
    DECLARE @Lenh nvarchar(max) = N'';
    SELECT @Lenh = @Lenh + N'ALTER TABLE dbo.LichLamViec DROP CONSTRAINT ' + QUOTENAME(f.name) + N';'
    FROM sys.foreign_keys f
    JOIN sys.foreign_key_columns k ON k.constraint_object_id = f.object_id
    JOIN sys.columns c ON c.object_id = k.parent_object_id AND c.column_id = k.parent_column_id
    WHERE f.parent_object_id = OBJECT_ID(N'dbo.LichLamViec')
        AND f.referenced_object_id = OBJECT_ID(N'dbo.ChamCong') AND c.name = N'MaChamCong';
    IF @Lenh <> N'' EXEC sys.sp_executesql @Lenh;

    IF COLUMNPROPERTY(OBJECT_ID(N'dbo.LichLamViec'), N'MaLich', 'IsIdentity') = 0
    BEGIN
        IF @MaLichLonNhat = 9223372036854775807
            THROW 51005, N'Mã lịch đã đạt giới hạn bigint.', 1;

        DECLARE @MaKeTiep bigint = @MaLichLonNhat + 1;
        IF OBJECT_ID(N'dbo.Seq_LichLamViec', N'SO') IS NULL
        BEGIN
            SET @Lenh = N'CREATE SEQUENCE dbo.Seq_LichLamViec AS bigint START WITH '
                + CONVERT(nvarchar(20), @MaKeTiep) + N' INCREMENT BY 1 NO CYCLE;';
            EXEC sys.sp_executesql @Lenh;
        END
        ELSE
        BEGIN
            IF EXISTS (SELECT 1 FROM sys.sequences
                WHERE object_id = OBJECT_ID(N'dbo.Seq_LichLamViec')
                    AND (CONVERT(bigint, increment) <> 1 OR is_cycling = 1))
                THROW 51006, N'Seq_LichLamViec phải tăng 1 và không quay vòng.', 1;

            IF EXISTS (SELECT 1 FROM sys.sequences
                WHERE object_id = OBJECT_ID(N'dbo.Seq_LichLamViec')
                    AND CONVERT(bigint, current_value) <= @MaLichLonNhat)
            BEGIN
                SET @Lenh = N'ALTER SEQUENCE dbo.Seq_LichLamViec RESTART WITH '
                    + CONVERT(nvarchar(20), @MaKeTiep) + N';';
                EXEC sys.sp_executesql @Lenh;
            END
        END

        IF NOT EXISTS (SELECT 1 FROM sys.default_constraints d
            JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
            WHERE d.parent_object_id = OBJECT_ID(N'dbo.LichLamViec') AND c.name = N'MaLich')
            EXEC(N'ALTER TABLE dbo.LichLamViec ADD CONSTRAINT DF_LichLamViec_MaLich
                DEFAULT (NEXT VALUE FOR dbo.Seq_LichLamViec) FOR MaLich;');
        ELSE IF EXISTS (SELECT 1 FROM sys.default_constraints d
            JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
            WHERE d.parent_object_id = OBJECT_ID(N'dbo.LichLamViec') AND c.name = N'MaLich'
                AND d.definition NOT LIKE N'%Seq_LichLamViec%')
            THROW 51007, N'MaLich đã có DEFAULT khác. Script không tự ghi đè cấu hình này.', 1;
    END

    -- Khóa ghép ngăn chấm công cho sai nhân viên hoặc sai ngày của lịch.
    IF NOT EXISTS (SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.LichLamViec') AND name = N'UX_LichLamViec_LichNhanVienNgay')
        CREATE UNIQUE INDEX UX_LichLamViec_LichNhanVienNgay
            ON dbo.LichLamViec(MaLich, MaNhanVien, NgayLam);

    IF OBJECT_ID(N'dbo.FK_ChamCong_LichLamViec', N'F') IS NULL
        ALTER TABLE dbo.ChamCong WITH CHECK ADD CONSTRAINT FK_ChamCong_LichLamViec
            FOREIGN KEY (MaLich, MaNhanVien, NgayChamCong)
            REFERENCES dbo.LichLamViec(MaLich, MaNhanVien, NgayLam);
    ALTER TABLE dbo.ChamCong WITH CHECK CHECK CONSTRAINT FK_ChamCong_LichLamViec;

    IF OBJECT_ID(N'dbo.CK_ChamCong_ThuTuGio', N'C') IS NULL
        ALTER TABLE dbo.ChamCong WITH CHECK ADD CONSTRAINT CK_ChamCong_ThuTuGio
            CHECK (GioRa IS NULL OR (GioVao IS NOT NULL AND GioRa >= GioVao));
    ALTER TABLE dbo.ChamCong WITH CHECK CHECK CONSTRAINT CK_ChamCong_ThuTuGio;

    IF OBJECT_ID(N'dbo.KhenThuong', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.KhenThuong (
            MaKhenThuong bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_KhenThuong PRIMARY KEY,
            MaNhanVien int NOT NULL,
            NgayKhenThuong date NOT NULL CONSTRAINT DF_KhenThuong_Ngay DEFAULT (CONVERT(date, SYSDATETIME())),
            LyDo nvarchar(500) NOT NULL,
            HinhThuc varchar(20) NOT NULL CONSTRAINT DF_KhenThuong_HinhThuc DEFAULT ('TienThuong'),
            SoTien decimal(18,2) NOT NULL CONSTRAINT DF_KhenThuong_SoTien DEFAULT (0),
            TrangThai varchar(20) NOT NULL CONSTRAINT DF_KhenThuong_TrangThai DEFAULT ('ChoDuyet'),
            GhiChu nvarchar(500) NULL,
            CONSTRAINT FK_KhenThuong_NhanVien FOREIGN KEY (MaNhanVien) REFERENCES dbo.NhanVien(MaNhanVien),
            CONSTRAINT CK_KhenThuong_LyDo CHECK (LEN(LTRIM(RTRIM(LyDo))) > 0),
            CONSTRAINT CK_KhenThuong_SoTien CHECK (SoTien >= 0),
            CONSTRAINT CK_KhenThuong_HinhThuc CHECK (HinhThuc IN ('TienThuong', 'BieuDuong', 'GiayKhen')),
            CONSTRAINT CK_KhenThuong_TrangThai CHECK (TrangThai IN ('ChoDuyet', 'DaDuyet', 'TuChoi'))
        );
        CREATE INDEX IX_KhenThuong_NhanVienNgay ON dbo.KhenThuong(MaNhanVien, NgayKhenThuong);
    END

    -- MaLich dùng SEQUENCE nên trả khóa bằng OUTPUT, không dùng SCOPE_IDENTITY.
    EXEC(N'CREATE OR ALTER PROCEDURE dbo.usp_ThemLichLamViec
        @MaNhanVien int, @MaCa int, @NgayLam date,
        @TrangThai varchar(20) = ''DaXep'', @GhiChu nvarchar(max) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        IF @MaNhanVien IS NULL OR @MaNhanVien <= 0 OR @MaCa IS NULL OR @MaCa <= 0 OR @NgayLam IS NULL
            THROW 51100, N''Vui lòng chọn nhân viên, ca và ngày làm hợp lệ.'', 1;
        IF DATALENGTH(@GhiChu) > 600
            THROW 51101, N''Ghi chú chỉ được tối đa 300 ký tự.'', 1;
        INSERT INTO dbo.LichLamViec(MaNhanVien, MaCa, NgayLam, TrangThai, GhiChu)
        OUTPUT INSERTED.MaLich
        VALUES (@MaNhanVien, @MaCa, @NgayLam, @TrangThai, @GhiChu);
    END;');

    -- DAL_BangLuong.HienThiBangLuongAll hiện đã gọi đúng tên thủ tục này.
    EXEC(N'CREATE OR ALTER PROCEDURE dbo.usp_HienThiBangLuong
        @MaNhanVien int = NULL, @Thang int = NULL, @Nam int = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT b.*, n.HoTen AS NhanVien
        FROM dbo.BangLuong b JOIN dbo.NhanVien n ON n.MaNhanVien = b.MaNhanVien
        WHERE (@MaNhanVien IS NULL OR b.MaNhanVien = @MaNhanVien)
            AND (@Thang IS NULL OR b.Thang = @Thang) AND (@Nam IS NULL OR b.Nam = @Nam)
        ORDER BY b.Nam DESC, b.Thang DESC, b.MaNhanVien;
    END;');

    COMMIT TRANSACTION;
    PRINT N'Nâng cấp Nhân sự thành công. Đã giữ nguyên dữ liệu cũ; có thể chạy lại script.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
    COMMIT TRANSACTION;
    PRINT N'Hoàn tất DB tổng. Có thể chạy lại file mà không nạp trùng dữ liệu mẫu.';
    SELECT DB_NAME() AS DatabaseSuDung,
        (SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped=0) AS SoBang,
        (SELECT COUNT(*) FROM sys.procedures WHERE is_ms_shipped=0) AS SoThuTuc;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;