using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;

namespace ET_QuanLyKhachSan
{
    [Table("BangLuong")]
    public class BangLuong
    {
        [Key]
        public long MaBangLuong { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public int MaNhanVien { get; set; } // SQL: int NOT NULL
        public int Thang { get; set; } // SQL: int NOT NULL
        public int Nam { get; set; } // SQL: int NOT NULL
        public decimal LuongCoBan { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienCong { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienTangCa { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienThuong { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienPhuCap { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienKhauTru { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal Thue { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TongLuong { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal ThucLinh { get; set; } // SQL: decimal(18, 2) NOT NULL
        public DateTime NgayLap { get; set; } // SQL: datetime2(0) NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }

    [Table("BaoTriPhong")]
    public class BaoTriPhong
    {
        [Key]
        public long MaBaoTri { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public int MaPhong { get; set; } // SQL: int NOT NULL
        public int? MaThietBi { get; set; } // SQL: int NULL
        public int? MaNhanVien { get; set; } // SQL: int NULL
        public int? MaNhaCungCap { get; set; } // SQL: int NULL
        public int? MaNguoiLienHe { get; set; } // SQL: int NULL
        public DateTime NgayYeuCau { get; set; } // SQL: datetime2(0) NOT NULL
        public string NoiDung { get; set; } = ""; // SQL: nvarchar(1000) NOT NULL
        public string MucDo { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public DateTime? NgayHoanThanh { get; set; } // SQL: datetime2(0) NULL
        public decimal ChiPhi { get; set; } // SQL: decimal(18, 2) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }

    [Table("CaLamViec")]
    public class CaLamViec
    {
        [Key]
        public int MaCa { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenCa { get; set; } = ""; // SQL: nvarchar(100) NOT NULL
        public TimeSpan GioBatDau { get; set; } // SQL: time(7) NOT NULL
        public TimeSpan GioKetThuc { get; set; } // SQL: time(7) NOT NULL
        public decimal SoGioLam { get; set; } // SQL: decimal(5, 2) NOT NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(300) NULL
    }

    [Table("ChamCong")]
    public class ChamCong
    {
        [Key]
        public long MaChamCong { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public long MaLich { get; set; } // SQL: bigint NOT NULL
        public int MaNhanVien { get; set; } // SQL: int NOT NULL
        public DateTime NgayChamCong { get; set; } // SQL: date NOT NULL
        public DateTime? GioVao { get; set; } // SQL: datetime2(0) NULL
        public DateTime? GioRa { get; set; } // SQL: datetime2(0) NULL
        public decimal? SoGioLam { get; set; } // SQL: decimal(5, 2) NULL
        public decimal SoGioTangCa { get; set; } // SQL: decimal(5, 2) NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(300) NULL
    }

    [Table("ChiTietDatPhong")]
    public class ChiTietDatPhong
    {
        [Key]
        public long MaChiTietDatPhong { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public long MaDatPhong { get; set; } // SQL: bigint NOT NULL
        public int MaLoaiPhong { get; set; } // SQL: int NOT NULL
        public DateTime NgayNhanDuKien { get; set; } // SQL: datetime2(0) NOT NULL
        public DateTime NgayTraDuKien { get; set; } // SQL: datetime2(0) NOT NULL
        public int SoLuongPhong { get; set; } // SQL: int NOT NULL
        public int NguoiLon { get; set; } // SQL: int NOT NULL
        public int TreEm { get; set; } // SQL: int NOT NULL
        public decimal DonGiaDuKien { get; set; } // SQL: decimal(18, 2) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }

    [Table("ChiTietGoiDichVu")]
    public class ChiTietGoiDichVu
    {
        // Khoa chinh ghep: MaGoiDichVu, MaDichVu.
        // Khong dung Contrib.Get<T>(id); truy van WHERE du tat ca cot khoa.
        [ExplicitKey]
        public int MaGoiDichVu { get; set; } // SQL: int NOT NULL
        [ExplicitKey]
        public int MaDichVu { get; set; } // SQL: int NOT NULL
        public decimal SoLuong { get; set; } // SQL: decimal(12, 2) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(300) NULL
    }

    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [Key]
        public long MaChiTietHoaDon { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public long MaHoaDon { get; set; } // SQL: bigint NOT NULL
        public string LoaiChiPhi { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public long? MaChiTietDatPhong { get; set; } // SQL: bigint NULL
        public long? MaSuDungDichVu { get; set; } // SQL: bigint NULL
        public int? MaGoiDichVu { get; set; } // SQL: int NULL
        public string NoiDung { get; set; } = ""; // SQL: nvarchar(300) NOT NULL
        public decimal SoLuong { get; set; } // SQL: decimal(12, 2) NOT NULL
        public decimal DonGia { get; set; } // SQL: decimal(18, 2) NOT NULL
        // SQL: CONVERT(decimal(18,2), SoLuong * DonGia) PERSISTED.
        // Cho phep NULL theo metadata cot tinh toan (khong khai bao NOT NULL).
        [Computed]
        public decimal? ThanhTien { get; set; }
    }

    [Table("ChucVu")]
    public class ChucVu
    {
        [Key]
        public int MaChucVu { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenChucVu { get; set; } = ""; // SQL: nvarchar(100) NOT NULL
        public decimal Chucvu { get; set; } // SQL: decimal(18, 2) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(500) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("CongViecDonPhong")]
    public class CongViecDonPhong
    {
        [Key]
        public long MaCongViec { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public int MaPhong { get; set; } // SQL: int NOT NULL
        public int? MaNhanVien { get; set; } // SQL: int NULL
        public DateTime NgayGiao { get; set; } // SQL: date NOT NULL
        public string LoaiCongViec { get; set; } = ""; // SQL: varchar(30) NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public DateTime? ThoiGianBatDau { get; set; } // SQL: datetime2(0) NULL
        public DateTime? ThoiGianKetThuc { get; set; } // SQL: datetime2(0) NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }

    [Table("DatPhong")]
    public class DatPhong
    {
        [Key]
        public long MaDatPhong { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public int MaKhachHangDat { get; set; } // SQL: int NOT NULL
        public int? MaNhanVienTao { get; set; } // SQL: int NULL
        public DateTime NgayDat { get; set; } // SQL: datetime2(0) NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(25) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(1000) NULL
    }

    [Table("DichVu")]
    public class DichVu
    {
        [Key]
        public int MaDichVu { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public int MaLoaiDichVu { get; set; } // SQL: int NOT NULL
        public string TenDichVu { get; set; } = ""; // SQL: nvarchar(150) NOT NULL
        public string DonViTinh { get; set; } = ""; // SQL: nvarchar(50) NULL
        public decimal DonGia { get; set; } // SQL: decimal(18, 2) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(1000) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("GoiDichVu")]
    public class GoiDichVu
    {
        [Key]
        public int MaGoiDichVu { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenGoiDichVu { get; set; } = ""; // SQL: nvarchar(150) NOT NULL
        public decimal DonGia { get; set; } // SQL: decimal(18, 2) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(1000) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]
        public long MaHoaDon { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public long MaDatPhong { get; set; } // SQL: bigint NOT NULL
        public int MaKhachHang { get; set; } // SQL: int NOT NULL
        public int? MaUuDai { get; set; } // SQL: int NULL
        public int? MaNhanVienLap { get; set; } // SQL: int NULL
        public DateTime NgayLap { get; set; } // SQL: datetime2(0) NOT NULL
        public decimal TongTien { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienGiam { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal ThanhTien { get; set; } // SQL: decimal(18, 2) NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }

    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string HoTen { get; set; } = ""; // SQL: nvarchar(150) NOT NULL
        public DateTime? NgaySinh { get; set; } // SQL: date NULL
        public string GioiTinh { get; set; } = ""; // SQL: char(1) NULL
        public string LoaiGiayTo { get; set; } = ""; // SQL: nvarchar(20) NULL
        public string SoGiayTo { get; set; } = ""; // Mã số giấy tờ; giữ kiểu chuỗi varchar(50) để không mất số 0 đầu. BUS kiểm tra theo LoaiGiayTo.
        public string QuocTich { get; set; } = ""; // SQL: nvarchar(80) NULL
        public string SoDienThoai { get; set; } = ""; // SQL: varchar(20) NULL
        public string Email { get; set; } = ""; // SQL: varchar(150) NULL
        public string DiaChi { get; set; } = ""; // SQL: nvarchar(300) NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
        public DateTime NgayTao { get; set; } // SQL: datetime2(0) NOT NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("Kho")]
    public class Kho
    {
        [Key]
        public int MaKho { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenKho { get; set; } = ""; // SQL: nvarchar(100) NOT NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("KhuVuc")]
    public class KhuVuc
    {
        [Key]
        public int MaKhuVuc { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenKhuVuc { get; set; } = ""; // SQL: nvarchar(100) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(500) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("LichLamViec")]
    public class LichLamViec
    {
        [ExplicitKey]
        public long MaLich { get; set; } // SQL: bigint NOT NULL
        public int MaNhanVien { get; set; } // SQL: int NOT NULL
        public int MaCa { get; set; } // SQL: int NOT NULL
        public DateTime NgayLam { get; set; } // SQL: date NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        // SQL: IDENTITY nhung khong phai khoa chinh; khoa chinh la MaLich.
        // [Computed] ngan ghi vao cot tu tang. Doc lai ban ghi de lay gia tri moi.
        [Computed]
        public long MaChamCong { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(300) NULL
    }

    [Table("LoaiDichVu")]
    public class LoaiDichVu
    {
        [Key]
        public int MaLoaiDichVu { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenLoaiDichVu { get; set; } = ""; // SQL: nvarchar(100) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(500) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("LoaiPhong")]
    public class LoaiPhong
    {
        [Key]
        public int MaLoaiPhong { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenLoaiPhong { get; set; } = ""; // SQL: nvarchar(100) NOT NULL
        public int SucChuaNguoiLon { get; set; } // SQL: int NOT NULL
        public int SucChuaTreEm { get; set; } // SQL: int NOT NULL
        public decimal GiaCoBan { get; set; } // SQL: decimal(18, 2) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(1000) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("LoaiPhong_TienNghi")]
    public class LoaiPhong_TienNghi
    {
        // Khoa chinh ghep: MaLoaiPhong, MaTienNghi.
        // Khong dung Contrib.Get<T>(id); truy van WHERE du tat ca cot khoa.
        [ExplicitKey]
        public int MaLoaiPhong { get; set; } // SQL: int NOT NULL
        [ExplicitKey]
        public int MaTienNghi { get; set; } // SQL: int NOT NULL
        public int SoLuong { get; set; } // SQL: int NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(300) NULL
    }

    [Table("NhaCungCap")]
    public class NhaCungCap
    {
        [Key]
        public int MaNhaCungCap { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenNhaCungCap { get; set; } = ""; // SQL: nvarchar(200) NOT NULL
        public string LoaiCungCap { get; set; } = ""; // SQL: nvarchar(100) NULL
        public string SoDienThoai { get; set; } = ""; // SQL: varchar(20) NULL
        public string Email { get; set; } = ""; // SQL: varchar(150) NULL
        public string DiaChi { get; set; } = ""; // SQL: nvarchar(300) NULL
        public string NguoiDaiDien { get; set; } = ""; // SQL: nvarchar(150) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }

    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        public int MaNhanVien { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string HoTen { get; set; } = ""; // SQL: nvarchar(150) NOT NULL
        public DateTime? NgaySinh { get; set; } // SQL: date NULL
        public string GioiTinh { get; set; } = ""; // SQL: char(1) NULL
        public string SoDienThoai { get; set; } = ""; // SQL: varchar(20) NULL
        public string Email { get; set; } = ""; // SQL: varchar(150) NULL
        public string DiaChi { get; set; } = ""; // SQL: nvarchar(300) NULL
        public int MaPhongBan { get; set; } // SQL: int NOT NULL
        public int MaChucVu { get; set; } // SQL: int NOT NULL
        public DateTime? NgayVaoLam { get; set; } // SQL: date NULL
        public decimal LuongCoBan { get; set; } // SQL: decimal(18, 2) NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
    }

    [Table("NhatKyThaoTac")]
    public class NhatKyThaoTac
    {
        [Key]
        public long MaNhatKy { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public int MaTaiKhoan { get; set; } // SQL: int NOT NULL
        public string HanhDong { get; set; } = ""; // SQL: varchar(50) NOT NULL
        public string DoiTuong { get; set; } = ""; // SQL: varchar(100) NULL
        public string MaBanGhi { get; set; } = ""; // SQL: varchar(50) NULL
        public DateTime ThoiGian { get; set; } // SQL: datetime2(0) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(1000) NULL
    }

    [Table("NhomVatTu")]
    public class NhomVatTu
    {
        [Key]
        public int MaNhomVatTu { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenNhomVatTu { get; set; } = ""; // SQL: nvarchar(100) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(500) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("Phong")]
    public class Phong
    {
        [Key]
        public int MaPhong { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string SoPhong { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public int MaTang { get; set; } // SQL: int NOT NULL
        public int MaLoaiPhong { get; set; } // SQL: int NOT NULL
        public string LoaiView { get; set; } = ""; // SQL: nvarchar(100) NULL
        public bool KhongHutThuoc { get; set; } // SQL: bit NOT NULL
        public string TrangThaiKhaiThac { get; set; } = ""; // SQL: varchar(30) NOT NULL
        public string TrangThaiVeSinh { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }

    [Table("PhongBan")]
    public class PhongBan
    {
        [Key]
        public int MaPhongBan { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenPhongBan { get; set; } = ""; // SQL: nvarchar(100) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(500) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("Quyen")]
    public class Quyen
    {
        [Key]
        public int MaQuyen { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string MaChucNang { get; set; } = ""; // SQL: nvarchar(100) NOT NULL
        public string TenQuyen { get; set; } = ""; // SQL: nvarchar(150) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(500) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("SuDungDichVu")]
    public class SuDungDichVu
    {
        [Key]
        public long MaSuDungDichVu { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public long MaDatPhong { get; set; } // SQL: bigint NOT NULL
        public int? MaKhachHang { get; set; } // SQL: int NULL
        public int? MaDichVu { get; set; } // SQL: int NULL
        public int? MaGoiDichVu { get; set; } // SQL: int NULL
        public int? MaNhanVien { get; set; } // SQL: int NULL
        public decimal SoLuong { get; set; } // SQL: decimal(12, 2) NOT NULL
        public decimal DonGia { get; set; } // SQL: decimal(18, 2) NOT NULL
        public DateTime ThoiGianSuDung { get; set; } // SQL: datetime2(0) NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }

    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public int MaNhanVien { get; set; } // SQL: int NOT NULL
        public string TenDangNhap { get; set; } = ""; // SQL: varchar(100) NOT NULL
        public string MatKhauHash { get; set; } = ""; // SQL: varchar(500) NOT NULL
        public int MaVaiTro { get; set; } // SQL: int NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public DateTime? LanDangNhapCuoi { get; set; } // SQL: datetime2(0) NULL
        public DateTime NgayTao { get; set; } // SQL: datetime2(0) NOT NULL
    }

    [Table("Tang")]
    public class Tang
    {
        [Key]
        public int MaTang { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public int MaKhuVuc { get; set; } // SQL: int NOT NULL
        public int SoTang { get; set; } // SQL: int NOT NULL
        public string TenTang { get; set; } = ""; // SQL: nvarchar(100) NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(500) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("ThanhToan")]
    public class ThanhToan
    {
        [Key]
        public long MaThanhToan { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public long MaHoaDon { get; set; } // SQL: bigint NOT NULL
        public int? MaNhanVien { get; set; } // SQL: int NULL
        public decimal SoTien { get; set; } // SQL: decimal(18, 2) NOT NULL
        public DateTime ThoiGianThanhToan { get; set; } // SQL: datetime2(0) NOT NULL
        public string MaGiaoDich { get; set; } = ""; // SQL: varchar(100) NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }

    [Table("ThietBi")]
    public class ThietBi
    {
        [Key]
        public int MaThietBi { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public int? MaPhong { get; set; } // SQL: int NULL
        public int? MaNhaCungCap { get; set; } // SQL: int NULL
        public string TenThietBi { get; set; } = ""; // SQL: nvarchar(150) NOT NULL
        public string MaThietBiNCC { get; set; } = ""; // SQL: varchar(100) NULL
        public DateTime? NgayMua { get; set; } // SQL: date NULL
        public DateTime? HanBaoHanh { get; set; } // SQL: date NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(30) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }

    [Table("TienNghi")]
    public class TienNghi
    {
        [Key]
        public int MaTienNghi { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenTienNghi { get; set; } = ""; // SQL: nvarchar(150) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(500) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("TieuHaoVatTu")]
    public class TieuHaoVatTu
    {
        // Khoa chinh ghep: MaDichVu, MaVatTu.
        // Khong dung Contrib.Get<T>(id); truy van WHERE du tat ca cot khoa.
        [ExplicitKey]
        public int MaDichVu { get; set; } // SQL: int NOT NULL
        [ExplicitKey]
        public int MaVatTu { get; set; } // SQL: int NOT NULL
        public decimal SoLuongTieuHao { get; set; } // SQL: decimal(12, 4) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(300) NULL
    }

    [Table("UuDai")]
    public class UuDai
    {
        [Key]
        public int MaUuDai { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string MaCode { get; set; } = ""; // SQL: varchar(50) NOT NULL
        public string TenUuDai { get; set; } = ""; // SQL: nvarchar(150) NOT NULL
        public string LoaiGiam { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public decimal GiaTriGiam { get; set; } // SQL: decimal(18, 2) NOT NULL
        public DateTime NgayBatDau { get; set; } // SQL: date NOT NULL
        public DateTime NgayKetThuc { get; set; } // SQL: date NOT NULL
        public string DieuKien { get; set; } = ""; // SQL: nvarchar(1000) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("VaiTro")]
    public class VaiTro
    {
        [Key]
        public int MaVaiTro { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public string TenVaiTro { get; set; } = ""; // SQL: nvarchar(100) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(500) NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("VaiTro_Quyen")]
    public class VaiTro_Quyen
    {
        // Khoa chinh ghep: MaVaiTro, MaQuyen.
        // Khong dung Contrib.Get<T>(id); truy van WHERE du tat ca cot khoa.
        [ExplicitKey]
        public int MaVaiTro { get; set; } // SQL: int NOT NULL
        [ExplicitKey]
        public int MaQuyen { get; set; } // SQL: int NOT NULL
    }

    [Table("VatTu")]
    public class VatTu
    {
        [Key]
        public int MaLoaiVatTu { get; set; } // SQL: int IDENTITY(1,1) NOT NULL
        public int MaNhomVatTu { get; set; } // SQL: int NOT NULL
        public int? MaKhoMacDinh { get; set; } // SQL: int NULL
        public string TenVatTu { get; set; } = ""; // SQL: nvarchar(150) NOT NULL
        public string DonViTinh { get; set; } = ""; // SQL: nvarchar(50) NOT NULL
        public string MoTa { get; set; } = ""; // SQL: nvarchar(500) NULL
        public decimal SoLuongToiThieu { get; set; } // SQL: decimal(18, 4) NOT NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
    }

    [Table("VatTu_NhaCungCap")]
    public class VatTu_NhaCungCap
    {
        // Khoa chinh ghep: MaVatTu, MaNhaCungCap.
        // Khong dung Contrib.Get<T>(id); truy van WHERE du tat ca cot khoa.
        [ExplicitKey]
        public int MaVatTu { get; set; } // SQL: int NOT NULL
        [ExplicitKey]
        public int MaNhaCungCap { get; set; } // SQL: int NOT NULL
        public string MaSanPhamNCC { get; set; } = ""; // SQL: varchar(100) NULL
        public decimal? DonGiaNhap { get; set; } // SQL: decimal(18, 2) NULL
        public DateTime? ThoiGianCungCap { get; set; } // SQL: date NULL
        public bool TrangThai { get; set; } // SQL: bit NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(300) NULL
    }

    [Table("XepPhong")]
    public class XepPhong
    {
        [Key]
        public long MaXepPhong { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public long MaChiTietDatPhong { get; set; } // SQL: bigint NOT NULL
        public int MaPhong { get; set; } // SQL: int NOT NULL
        public DateTime? NgayNhanThucTe { get; set; } // SQL: datetime2(0) NULL
        public DateTime? NgayTraThucTe { get; set; } // SQL: datetime2(0) NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
    }
}
