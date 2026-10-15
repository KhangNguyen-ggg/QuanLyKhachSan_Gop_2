using System.Windows.Controls;
using System.Windows;
using System.Globalization;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Helpers;

public static class KhoVatTuGiaoDien
{
    // Thêm: mở khóa ngay trong cửa sổ chi tiết; Tag="Khoa" giữ nguyên mã và trường dẫn xuất.
    public static void MoKhoa(DependencyObject vung, bool sua)
        => NhanSuGiaoDien.MoKhoa(vung, sua);
    // Thêm: dùng lại cách binding ComboBox của Nhân sự, không dùng tên/mã giả làm khóa SQL.
    public static void Kho(ComboBox chon, BUS_KhoVatTu bus, int ma = 0) =>
        LuaChon(chon, bus.DanhMuc<ET_QuanLyKhachSan.Kho>().Where(x => x.TrangThai), "TenKho", "MaKho", ma);
    public static void VatTu(ComboBox chon, BUS_KhoVatTu bus, int ma = 0) =>
        LuaChon(chon, bus.DanhMuc<ET_QuanLyKhachSan.VatTu>().Where(x => x.TrangThai), "TenVatTu", "MaLoaiVatTu", ma);
    public static void NhanVien(ComboBox chon, BUS_KhoVatTu bus, int ma = 0) =>
        LuaChon(chon, bus.DanhMuc<ET_QuanLyKhachSan.NhanVien>().Where(x => x.TrangThai == "DangLam"), "HoTen", "MaNhanVien", ma);
    public static void NhaCungCap(ComboBox chon, BUS_KhoVatTu bus, int? ma = null) =>
        LuaChon(chon, new[] { new ET_QuanLyKhachSan.NhaCungCap { TenNhaCungCap = "— Không chọn —" } }.Concat(bus.DanhMuc<ET_QuanLyKhachSan.NhaCungCap>().Where(x => x.TrangThai)), "TenNhaCungCap", "MaNhaCungCap", ma ?? 0);
    public static int Ma(ComboBox chon) => chon.SelectedValue is int ma ? ma : 0;
    public static DateTime? Ngay(DatePicker chon, string ten, bool batBuoc = false)
    {
        if (string.IsNullOrWhiteSpace(chon.Text) && !batBuoc) return null;
        // Thêm: không âm thầm lưu ngày cũ khi người dùng gõ một ngày không hợp lệ.
        BUS_KiemTra.YeuCau(DateTime.TryParse(chon.Text, CultureInfo.CurrentCulture, DateTimeStyles.None, out var ngay), "Vui lòng nhập đúng " + ten + ".");
        return ngay.Date;
    }
}
