using System.Collections.ObjectModel;
namespace QuanLyKhachSan.Data
{
    // Dữ liệu trình bày; SqlRow giữ bản ghi thật cho các biểu mẫu nghiệp vụ.
    public class SqlRecord
    {
        public long SqlId { get; set; }
        public object? SqlRow { get; set; }
    }
    public class PhongMau
    {
        public string Ma { get; set; } = "";
        public string SoPhong { get; set; } = "";
        public string LoaiPhong { get; set; } = "";
        public string Tang { get; set; } = "";
        public string SucChua { get; set; } = "";
        public string GiaDem { get; set; } = "";
        public string VeSinh { get; set; } = "";
        public string KhaiThac { get; set; } = "";
        public string TinhTrang { get; set; } = "Chưa cập nhật";
        public string GhiChu { get; set; } = "";
    }
    public class TinhTrangPhongMau
    {
        public string Phong { get; set; } = "";
        public string TinhTrang { get; set; } = "";
        public string KhachDatPhong { get; set; } = "";
        public string NhanPhong { get; set; } = "";
        public string TraPhong { get; set; } = "";
        public string VeSinh { get; set; } = "";
    }
    public class DatPhongMau : SqlRecord
    {
        public ObservableCollection<PhanBoPhongMau> PhanBos { get; } = new();
        public string Ma { get; set; } = "";
        public string KhachHang { get; set; } = "";
        public string NhanPhong { get; set; } = "";
        public string TraPhong { get; set; } = "";
        public string Phong { get; set; } = "";
        public string SoKhach { get; set; } = "";
        public string TrangThai { get; set; } = "";

        public void ChonPhong(IEnumerable<PhanBoPhongMau> rooms)
        {
            var selected = rooms.ToArray();
            foreach (var room in PhanBos.Where(room => !selected.Any(s => s.Phong == room.Phong)).ToArray())
                PhanBos.Remove(room);
            foreach (var room in selected.Where(room => !PhanBos.Any(s => s.Phong == room.Phong)))
            {
                room.DatPhong = Ma;
                room.TrangThai = TrangThai == "Đang ở" ? "Đang ở" : "Đã phân bổ";
                PhanBos.Add(room);
            }
            CapNhatDanhSachPhong();
        }

        public void CapNhatDanhSachPhong() => Phong = string.Join(", ", PhanBos.Select(room => room.Phong));
    }
    public class PhanBoPhongMau : SqlRecord
    {
        public string DatPhong { get; set; } = "";
        public string Phong { get; set; } = "";
        public string LoaiPhong { get; set; } = "";
        public string NguoiLon { get; set; } = "";
        public string TreEm { get; set; } = "";
        public string GiaApDung { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class KhachLuuTruMau : SqlRecord
    {
        public string Ma { get; set; } = "";
        public string DatPhong { get; set; } = "";
        public string Phong { get; set; } = "";
        public string KhachHang { get; set; } = "";
        public string GiayTo { get; set; } = "";
        public string NhomKhach { get; set; } = "";
        public string NgayVao { get; set; } = "";
        public string NgayRa { get; set; } = "";
    }
    public class LichSuLuuTruMau : SqlRecord
    {
        public string KhachHang { get; set; } = "";
        public string DatPhong { get; set; } = "";
        public string Phong { get; set; } = "";
        public string TuNgay { get; set; } = "";
        public string DenNgay { get; set; } = "";
        public string DichVu { get; set; } = "";
        public string HoaDon { get; set; } = "";
    }
    public class KhachHangMau : SqlRecord
    {
        public string Ma { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string GiayTo { get; set; } = "";
        public string DienThoai { get; set; } = "";
        public string QuocTich { get; set; } = "";
        public string Email { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class DichVuMau
    {
        public string Ma { get; set; } = "";
        public string TenDichVu { get; set; } = "";
        public string LoaiDichVu { get; set; } = "";
        public string DonVi { get; set; } = "";
        public string Gia { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class SuDungDichVuMau
    {
        public string Ma { get; set; } = "";
        public string DatPhong { get; set; } = "";
        public string KhachHang { get; set; } = "";
        public string DichVuGoi { get; set; } = "";
        public string SoLuong { get; set; } = "";
        public string DonGia { get; set; } = "";
        public string ThanhTien { get; set; } = "";
        public string ThoiDiem { get; set; } = "";
    }
    public class LoaiDichVuMau
    {
        public string Ma { get; set; } = "";
        public string TenLoai { get; set; } = "";
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class GoiDichVuMau
    {
        public ObservableCollection<ThanhPhanGoiMau> ThanhPhans { get; } = new();
        public string Ma { get; set; } = "";
        public string TenGoi { get; set; } = "";
        public string GiaGoi { get; set; } = "";
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class ThanhPhanGoiMau
    {
        public string Goi { get; set; } = "";
        public string DichVu { get; set; } = "";
        public string SoLuong { get; set; } = "";
        public string DonGiaThanhPhan { get; set; } = "";
        public string GhiChu { get; set; } = "";
    }
    public class UuDaiMau
    {
        public string Ma { get; set; } = "";
        public string TenUuDai { get; set; } = "";
        public string MaApDung { get; set; } = "";
        public string LoaiGiam { get; set; } = "";
        public string GiaTri { get; set; } = "";
        public string TuNgay { get; set; } = "";
        public string DenNgay { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class HoaDonMau : SqlRecord
    {
        public string Ma { get; set; } = "";
        public string DatPhong { get; set; } = "";
        public string KhachHang { get; set; } = "";
        public string TienHang { get; set; } = "";
        public string GiamGia { get; set; } = "";
        public string PhaiThu { get; set; } = "";
        public string DaThu { get; set; } = "";
        public string ConThieu { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class LichSuThuTienMau : SqlRecord
    {
        public string PhieuThu { get; set; } = "";
        public string HoaDon { get; set; } = "";
        public string ThoiDiem { get; set; } = "";
        public string SoTien { get; set; } = "";
        public string PhuongThuc { get; set; } = "";
        public string NguoiThu { get; set; } = "";
        public string GhiChu { get; set; } = "";
    }
    public class TonKhoMau
    {
        public string MaVatTu { get; set; } = "";
        public string VatTu { get; set; } = "";
        public string Nhom { get; set; } = "";
        public string Kho { get; set; } = "";
        public string DonVi { get; set; } = "";
        public string Ton { get; set; } = "";
        public string ToiThieu { get; set; } = "";
        public string CanhBao { get; set; } = "";
    }
    public class PhieuNhapXuatMau
    {
        public ObservableCollection<ChiTietPhieuKhoMau> ChiTiets { get; } = new();
        public string MaPhieu { get; set; } = "";
        public string Loai { get; set; } = "";
        public string Kho { get; set; } = "";
        public string NhaCungCap { get; set; } = "";
        public string Ngay { get; set; } = "";
        public string NhanVien { get; set; } = "";
        public string GhiChu { get; set; } = "";
    }
    public class ChiTietPhieuKhoMau
    {
        public string Phieu { get; set; } = "";
        public string VatTu { get; set; } = "";
        public string SoLuong { get; set; } = "";
        public string DonVi { get; set; } = "";
        public string DonGia { get; set; } = "";
        public string ThanhTien { get; set; } = "";
    }
    public class KhoMau
    {
        public string Ma { get; set; } = "";
        public string TenKho { get; set; } = "";
        public string GhiChu { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class VatTuMau
    {
        public string Ma { get; set; } = "";
        public string TenVatTu { get; set; } = "";
        public string Nhom { get; set; } = "";
        public string DonVi { get; set; } = "";
        public string KhoMacDinh { get; set; } = "";
        public string TonToiThieu { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class NhomVatTuMau
    {
        public string Ma { get; set; } = "";
        public string TenNhom { get; set; } = "";
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class NhaCungCapMau
    {
        public ObservableCollection<VatTuNhaCungCapMau> VatTus { get; } = new();
        public string Ma { get; set; } = "";
        public string TenNhaCungCap { get; set; } = "";
        public string NguoiDaiDien { get; set; } = "";
        public string DienThoai { get; set; } = "";
        public string Email { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class VatTuNhaCungCapMau
    {
        public string VatTu { get; set; } = "";
        public string NhaCungCap { get; set; } = "";
        public string MaSanPhamNCC { get; set; } = "";
        public string GiaNhap { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class DinhMucTieuHaoMau
    {
        public string DichVu { get; set; } = "";
        public string VatTu { get; set; } = "";
        public string SoLuongLan { get; set; } = "";
        public string DonVi { get; set; } = "";
        public string GhiChu { get; set; } = "";
    }
    public class CapVatTuNoiBoMau
    {
        public string Ma { get; set; } = "";
        public string MucDich { get; set; } = "";
        public string ThamChieu { get; set; } = "";
        public string Kho { get; set; } = "";
        public string VatTu { get; set; } = "";
        public string SoLuong { get; set; } = "";
        public string ThoiDiem { get; set; } = "";
    }
    public class NhanVienMau
    {
        public string Ma { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string PhongBan { get; set; } = "";
        public string ChucVu { get; set; } = "";
        public string DienThoai { get; set; } = "";
        public string LuongCoBan { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class PhongBanMau
    {
        public string Ma { get; set; } = "";
        public string TenPhongBan { get; set; } = "";
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class ChucVuMau
    {
        public string Ma { get; set; } = "";
        public string TenChucVu { get; set; } = "";
        public string MoTa { get; set; } = "";
        public string Luong { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class CaLamViecMau
    {
        public string Ma { get; set; } = "";
        public string TenCa { get; set; } = "";
        public string GioBatDau { get; set; } = "";
        public string GioKetThuc { get; set; } = "";
        public string SoGio { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class LichLamViecMau
    {
        public string Ma { get; set; } = "";
        public string NhanVien { get; set; } = "";
        public string NgayLam { get; set; } = "";
        public string Ca { get; set; } = "";
        public string TrangThai { get; set; } = "";
        public string GhiChu { get; set; } = "";
    }
    public class ChamCongMau
    {
        public string Ma { get; set; } = "";
        public string Lich { get; set; } = "";
        public string NhanVien { get; set; } = "";
        public string Ngay { get; set; } = "";
        public string GioVao { get; set; } = "";
        public string GioRa { get; set; } = "";
        public string GioThuong { get; set; } = "";
        public string TangCa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class BangLuongMau
    {
        public string Ma { get; set; } = "";
        public string NhanVien { get; set; } = "";
        public string Ky { get; set; } = "";
        public string TienCong { get; set; } = "";
        public string TangCa { get; set; } = "";
        public string Thuong { get; set; } = "";
        public string PhuCap { get; set; } = "";
        public string KhauTru { get; set; } = "";
        public string ThucLinh { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class KhenThuongMau
    {
        public string Ma { get; set; } = "";
        public string NhanVien { get; set; } = "";
        public string Ngay { get; set; } = "";
        public string LyDo { get; set; } = "";
        public string HinhThuc { get; set; } = "";
        public string SoTien { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class DonPhongMau
    {
        public string Ma { get; set; } = "";
        public string Phong { get; set; } = "";
        public string NhanVien { get; set; } = "";
        public string NgayGiao { get; set; } = "";
        public string LoaiViec { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class BaoTriMau
    {
        public string Ma { get; set; } = "";
        public string Phong { get; set; } = "";
        public string ThietBi { get; set; } = "";
        public string NoiDung { get; set; } = "";
        public string MucDo { get; set; } = "";
        public string NguoiPhuTrach { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class ThietBiMau
    {
        public string Ma { get; set; } = "";
        public string TenThietBi { get; set; } = "";
        public string Phong { get; set; } = "";
        public string NhaCungCap { get; set; } = "";
        public string NgayMua { get; set; } = "";
        public string HanBaoHanh { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class KhuVucMau
    {
        public ObservableCollection<TangMau> Tangs { get; } = new();
        public string Ma { get; set; } = "";
        public string TenKhuVuc { get; set; } = "";
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class TangMau
    {
        public string Ma { get; set; } = "";
        public string TenTang { get; set; } = "";
        public string SoTang { get; set; } = "";
        public string KhuVuc { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class LoaiPhongMau
    {
        public ObservableCollection<TienNghiTheoLoaiMau> TienNghis { get; } = new();
        public string Ma { get; set; } = "";
        public string TenLoai { get; set; } = "";
        public string GiaDem { get; set; } = "";
        public string NguoiLon { get; set; } = "";
        public string TreEm { get; set; } = "";
        public string DienTich { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class TienNghiMau
    {
        public string Ma { get; set; } = "";
        public string TenTienNghi { get; set; } = "";
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class TienNghiTheoLoaiMau
    {
        public string LoaiPhong { get; set; } = "";
        public string TienNghi { get; set; } = "";
        public string SoLuong { get; set; } = "";
        public string GhiChu { get; set; } = "";
    }
    public class TaiKhoanMau : SqlRecord
    {
        public string Ma { get; set; } = "";
        public string NhanVien { get; set; } = "";
        public string TenDangNhap { get; set; } = "";
        public string VaiTro { get; set; } = "";
        public string TrangThai { get; set; } = "";
        public string DangNhapGanNhat { get; set; } = "";
    }
    public class VaiTroQuyenMau
    {
        public string Ma { get; set; } = "";
        public string TenVaiTro { get; set; } = "";
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class NhatKyThaoTacMau
    {
        public string Ma { get; set; } = "";
        public string ThoiDiem { get; set; } = "";
        public string TaiKhoan { get; set; } = "";
        public string HanhDong { get; set; } = "";
        public string DoiTuong { get; set; } = "";
        public string MaBanGhi { get; set; } = "";
        public string NoiDung { get; set; } = "";
    }
}
