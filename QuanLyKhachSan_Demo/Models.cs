using System;
using System.Collections.Generic;

namespace QuanLyKhachSan.Models
{
    // ==========================================
    // NHÓM 1: KHU VỰC, TIỆN NGHI, PHÒNG
    // ==========================================
    public class KhuVuc
    {
        public int MaKhuVuc { get; set; } = 0;
        public string TenKhuVuc { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class Tang
    {
        public int MaTang { get; set; }
        public int MaKhuVuc { get; set; }
        public int SoTang { get; set; }
        public string TenTang { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class TienNghi
    {
        public int MaTienNghi { get; set; }
        public string TenTienNghi { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class LoaiPhong
    {
        public int MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; } = string.Empty;
        public decimal GiaCoBan { get; set; }
        public int SucChuaNguoiLon { get; set; }
        public int SucChuaTreEm { get; set; }
        public decimal DienTich { get; set; }
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class LoaiPhong_TienNghi
    {
        public int MaLoaiPhong { get; set; }
        public int MaTienNghi { get; set; }
        public int SoLuong { get; set; }
        public string GhiChu { get; set; } = string.Empty;
    }

    public class Phong
    {
        public int MaPhong { get; set; }
        public int MaTang { get; set; }
        public int MaLoaiPhong { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public string LoaiView { get; set; } = string.Empty;
        public bool KhongHutThuoc { get; set; } = true;
        public string TrangThaiVeSinh { get; set; } = "SACH";
        public string TrangThaiKhaiThac { get; set; } = "DANG_HOAT_DONG";
        public string GhiChu { get; set; } = string.Empty;
    }

    // ==========================================
    // NHÓM 2: KHÁCH HÀNG & ĐẶT PHÒNG
    // ==========================================
    public class KhachHang
    {
        public int MaKhachHang { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string GioiTinh { get; set; } = "M";
        public DateTime? NgaySinh { get; set; }
        public string LoaiGiayTo { get; set; } = "CCCD";
        public string SoGiayTo { get; set; } = string.Empty;
        public string QuocTich { get; set; } = "Việt Nam";
        public string DiaChi { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HangKhachHang { get; set; } = "THUONG"; // Giữ cho UI Demo
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public bool TrangThai { get; set; } = true;
    }

    public class DatPhong
    {
        public long MaDatPhong { get; set; }
        public int MaKhachHangDat { get; set; }
        public int? MaNhanVienTao { get; set; }
        public DateTime NgayDat { get; set; } = DateTime.Now;
        public string TrangThai { get; set; } = "CHO_XAC_NHAN";
        public string GhiChu { get; set; } = string.Empty;
    }

    public class ChiTietDatPhong
    {
        public long MaChiTietDatPhong { get; set; }
        public long MaDatPhong { get; set; }
        public int MaLoaiPhong { get; set; }
        public DateTime NgayNhanDuKien { get; set; }
        public DateTime NgayTraDuKien { get; set; }
        public int SoLuongPhong { get; set; } = 1;
        public int NguoiLon { get; set; } = 2;
        public int TreEm { get; set; } = 0;
        public decimal DonGiaPhong { get; set; } // Giữ cho UI Demo tính tổng
        public string GhiChu { get; set; } = string.Empty;
    }

    public class XepPhong
    {
        public long MaXepPhong { get; set; } // Identity for mapping
        public long MaChiTietDatPhong { get; set; }
        public int MaPhong { get; set; }
        public DateTime? NgayNhanThucTe { get; set; }
        public DateTime? NgayTraThucTe { get; set; }
        public string TrangThai { get; set; } = "DA_PHAN_BO";
        public string GhiChu { get; set; } = string.Empty;
    }

    // ==========================================
    // NHÓM 3: DỊCH VỤ
    // ==========================================
    public class LoaiDichVu
    {
        public int MaLoaiDichVu { get; set; }
        public string TenLoaiDichVu { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class DichVu
    {
        public int MaDichVu { get; set; }
        public int MaLoaiDichVu { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public decimal Gia { get; set; } // Giữ cho UI Demo
        public string DonViTinh { get; set; } = "Lần";
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class GoiDichVu
    {
        public int MaGoiDichVu { get; set; }
        public string TenGoiDichVu { get; set; } = string.Empty;
        public decimal Gia { get; set; } // Giữ cho UI Demo
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class ChiTietGoiDichVu
    {
        public int MaGoiDichVu { get; set; }
        public int MaDichVu { get; set; }
        public string GhiChu { get; set; } = string.Empty;
    }

    public class SuDungDichVu
    {
        public long MaSuDungDichVu { get; set; } // Identity for mapping
        public long MaDatPhong { get; set; }
        public int? MaKhachHang { get; set; }
        public int? MaDichVu { get; set; }
        public int? MaGoiDichVu { get; set; }
        public int? MaNhanVien { get; set; }
        public DateTime ThoiGianSuDung { get; set; } = DateTime.Now;
        public string TrangThai { get; set; } = "HOAN_THANH";
        public string GhiChu { get; set; } = string.Empty;
    }

    // ==========================================
    // NHÓM 4: ƯU ĐÃI & HÓA ĐƠN & THANH TOÁN
    // ==========================================
    public class UuDai
    {
        public int MaUuDai { get; set; }
        public string TenUuDai { get; set; } = string.Empty;
        public string MaCode { get; set; } = string.Empty;
        public string LoaiGiam { get; set; } = "PHAN_TRAM";
        public decimal GiaTriGiam { get; set; } // Giữ cho UI Demo
        public decimal? GiaTriGiamToiDa { get; set; } // Giữ cho UI Demo
        public decimal? GiaTriDonToiThieu { get; set; } // Giữ cho UI Demo
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string DieuKien { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class HoaDon
    {
        public long MaHoaDon { get; set; }
        public long MaDatPhong { get; set; }
        public int MaKhachHang { get; set; }
        public int? MaUuDai { get; set; }
        public int? MaNhanVienLap { get; set; }
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public decimal TongTienHang { get; set; } // Giữ cho UI Demo
        public decimal TienGiam { get; set; } // Giữ cho UI Demo
        public decimal TongThanhToan { get; set; } // Giữ cho UI Demo
        public string TrangThai { get; set; } = "CHUA_THANH_TOAN";
        public string GhiChu { get; set; } = string.Empty;
    }

    public class ChiTietHoaDon
    {
        public long MaChiTietHoaDon { get; set; } // Identity for mapping
        public long MaHoaDon { get; set; }
        public string LoaiChiPhi { get; set; } = string.Empty;
        public long? MaChiTietDatPhong { get; set; }
        public long? MaSuDungDichVu { get; set; }
        public int? MaGoiDichVu { get; set; }
        public string NoiDung { get; set; } = string.Empty;
    }

    public class PhuongThucThanhToan
    {
        public int MaPhuongThucThanhToan { get; set; }
        public string TenPhuongThuc { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class ThanhToan
    {
        public long MaThanhToan { get; set; }
        public long MaHoaDon { get; set; }
        public int MaPhuongThucThanhToan { get; set; }
        public int? MaNhanVienThu { get; set; }
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;
        public string MaGiaoDich { get; set; } = string.Empty;
        public decimal SoTien { get; set; } // Giữ cho UI Demo
        public string PhuongThuc { get; set; } = "TIEN_MAT"; // Giữ cho UI Demo compatibility
        public string TrangThai { get; set; } = "DA_GHI_NHAN";
        public string GhiChu { get; set; } = string.Empty;
    }

    // ==========================================
    // NHÓM 5: NHÂN SỰ
    // ==========================================
    public class PhongBan
    {
        public int MaPhongBan { get; set; }
        public string TenPhongBan { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class ChucVu
    {
        public int MaChucVu { get; set; }
        public string TenChucVu { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class NhanVien
    {
        public int MaNhanVien { get; set; }
        public string MaNhanVienCode { get; set; } = string.Empty;
        public int MaPhongBan { get; set; }
        public int MaChucVu { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string GioiTinh { get; set; } = "M";
        public DateTime? NgaySinh { get; set; }
        public string SoDienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public DateTime NgayVaoLam { get; set; } = DateTime.Now;
        public decimal LuongCoBan { get; set; } // Giữ cho UI Demo
        public string TrangThai { get; set; } = "DANG_LAM";
    }

    public class CaLamViec
    {
        public int MaCa { get; set; }
        public string TenCa { get; set; } = string.Empty;
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
        public decimal SoGioLam { get; set; }
        public bool TrangThai { get; set; } = true;
        public string GhiChu { get; set; } = string.Empty;
    }

    public class LichLamViec
    {
        public long MaLich { get; set; }
        public int MaNhanVien { get; set; }
        public int MaCa { get; set; }
        public DateTime NgayLam { get; set; }
        public string TrangThai { get; set; } = "DA_LEN_LICH";
        public string GhiChu { get; set; } = string.Empty;
    }

    public class ChamCong
    {
        public long MaChamCong { get; set; }
        public long MaLich { get; set; }
        public int MaNhanVien { get; set; }
        public DateTime NgayChamCong { get; set; }
        public DateTime? GioVao { get; set; }
        public DateTime? GioRa { get; set; }
        public decimal? SoGioLam { get; set; }
        public decimal SoGioTangCa { get; set; }
        public string TrangThai { get; set; } = "DA_CHAM";
        public string GhiChu { get; set; } = string.Empty;
    }

    public class BangLuong
    {
        public long MaBangLuong { get; set; }
        public int MaNhanVien { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public decimal LuongCoBan { get; set; }
        public decimal TienCong { get; set; }
        public decimal TienTangCa { get; set; }
        public decimal TienThuong { get; set; }
        public decimal TienPhuCap { get; set; }
        public decimal TienKhauTru { get; set; }
        public decimal Thue { get; set; }
        public decimal TongLuong { get; set; }
        public decimal ThucLinh { get; set; }
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public string TrangThai { get; set; } = "CHUA_THANH_TOAN";
        public string GhiChu { get; set; } = string.Empty;
    }

    public class CongViecDonPhong
    {
        public long MaCongViec { get; set; } // Identity for mapping
        public int MaPhong { get; set; }
        public int? MaNhanVien { get; set; }
        public DateTime NgayGiao { get; set; }
        public string LoaiCongViec { get; set; } = "DON_PHONG_HANG_NGAY";
        public string TrangThai { get; set; } = "CHUA_BAT_DAU";
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string GhiChu { get; set; } = string.Empty;
    }

    // ==========================================
    // NHÓM 6: BẢO TRÍ & THIẾT BỊ
    // ==========================================
    public class ThietBi
    {
        public int MaThietBi { get; set; }
        public int? MaPhong { get; set; }
        public int? MaNhaCungCap { get; set; }
        public string TenThietBi { get; set; } = string.Empty;
        public string MaThietBiNCC { get; set; } = string.Empty;
        public DateTime? NgayMua { get; set; }
        public DateTime? HanBaoHanh { get; set; }
        public string TrangThai { get; set; } = "HOAT_DONG_TOT";
        public string GhiChu { get; set; } = string.Empty;
    }

    public class BaoTriPhong
    {
        public long MaBaoTri { get; set; } // Identity for mapping
        public int MaPhong { get; set; }
        public int? MaNhanVien { get; set; }
        public DateTime NgayYeuCau { get; set; } = DateTime.Now;
        public string NoiDung { get; set; } = string.Empty;
        public string MucDo { get; set; } = "TRUNG_BINH";
        public string TrangThai { get; set; } = "CHO_XU_LY";
        public DateTime? NgayHoanThanh { get; set; }
        public string GhiChu { get; set; } = string.Empty;
    }

    // ==========================================
    // NHÓM 7: KHO & VẬT TƯ
    // ==========================================
    public class Kho
    {
        public int MaKho { get; set; }
        public string TenKho { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class NhomVatTu
    {
        public int MaNhomVatTu { get; set; }
        public string TenNhomVatTu { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class VatTu
    {
        public int MaVatTu { get; set; }
        public int MaNhomVatTu { get; set; }
        public int? MaKhoMacDinh { get; set; }
        public string TenVatTu { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public decimal SoLuongToiThieu { get; set; }
        public bool TrangThai { get; set; } = true;
    }

    public class TieuHaoVatTu
    {
        public int MaDichVu { get; set; }
        public int MaVatTu { get; set; }
        public string GhiChu { get; set; } = string.Empty;
    }

    public class GiaoDichKho
    {
        public int MaKho { get; set; }
        public long MaGiaoDichKho { get; set; } // Header ID for mapping
        public int? MaNhanVien { get; set; }
        public string LoaiGiaoDich { get; set; } = "NHAP_KHO";
        public DateTime NgayGiaoDich { get; set; } = DateTime.Now;
        public string GhiChu { get; set; } = string.Empty;
    }

    public class ChiTietGiaoDichKho
    {
        public long MaGiaoDichKho { get; set; }
        public int MaVatTu { get; set; }
        public decimal SoLuong { get; set; }
        public string GhiChu { get; set; } = string.Empty;
    }

    public class TonKho
    {
        public int MaKho { get; set; }
        public int MaVatTu { get; set; }
        public decimal SoLuong { get; set; }
    }

    // ==========================================
    // NHÓM 8: NHÀ CUNG CẤP
    // ==========================================
    public class NhaCungCap
    {
        public int MaNhaCungCap { get; set; }
        public string TenNhaCungCap { get; set; } = string.Empty;
        public string LoaiCungCap { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string NguoiDaiDien { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
        public string GhiChu { get; set; } = string.Empty;
    }

    public class NhanVienNhaCungCap
    {
        public int MaNguoiLienHe { get; set; }
        public int MaNhaCungCap { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string ChucVu { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ChuyenMon { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
        public string GhiChu { get; set; } = string.Empty;
    }

    public class VatTu_NhaCungCap
    {
        public int MaVatTu { get; set; }
        public int MaNhaCungCap { get; set; }
        public string MaSanPhamNCC { get; set; } = string.Empty;
        public decimal? DonGiaNhap { get; set; }
        public DateTime? ThoiGianCungCap { get; set; }
        public bool TrangThai { get; set; } = true;
        public string GhiChu { get; set; } = string.Empty;
    }

    // ==========================================
    // NHÓM 9: PHÂN QUYỀN
    // ==========================================
    public class TaiKhoan
    {
        public int MaTaiKhoan { get; set; }
        public int MaNhanVien { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhauHash { get; set; } = string.Empty;
        public int MaVaiTro { get; set; }
        public string TrangThai { get; set; } = "HOAT_DONG";
        public DateTime? LanDangNhapCuoi { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }

    public class VaiTro
    {
        public int MaVaiTro { get; set; } // Identity for mapping
        public string TenVaiTro { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class Quyen
    {
        public int MaQuyen { get; set; } // Identity for mapping
        public string MaChucNang { get; set; } = string.Empty;
        public string TenQuyen { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class VaiTro_Quyen
    {
        public int MaVaiTro { get; set; }
        public int MaQuyen { get; set; }
    }

    public class NhatKyThaoTac
    {
        public long MaNhatKy { get; set; } // Identity for mapping
        public int MaTaiKhoan { get; set; }
        public string HanhDong { get; set; } = string.Empty;
        public string DoiTuong { get; set; } = string.Empty;
        public string MaBanGhi { get; set; } = string.Empty;
        public DateTime ThoiGian { get; set; } = DateTime.Now;
        public string MoTa { get; set; } = string.Empty;
    }
}
