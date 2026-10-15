using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class LichLamViecForm : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly LichLamViec duLieu;
    private readonly string quyen;

    public LichLamViecForm() : this(null) { }

    // Thêm: một form dùng lại cho Thêm và Sửa; bản nháp giúp đóng/hủy không đổi dữ liệu gốc.
    public LichLamViecForm(LichLamViec? banGoc)
    {
        InitializeComponent();
        duLieu = banGoc == null ? new LichLamViec { NgayLam = DateTime.Today, TrangThai = "DaXep" } : NhanSuGiaoDien.BanNhap(banGoc);
        quyen = duLieu.MaLich == 0 ? "btnLichLamViecThemLich" : "btnLichLamViecSuaThongTin";
        DataContext = duLieu;
        txtTieuDe.Text = (duLieu.MaLich == 0 ? "Thêm — " : "Sửa — ") + "Lịch làm việc";
        btnLuu.IsEnabled = DemoSession.CanUseButton("NhanSuView", quyen);
        if (!DemoSession.CanOpen("NhanSuView")) return;
        try
        {
            NhanSuGiaoDien.LuaChon(cboNhanVien, bus.NhanVien().Where(x => x.TrangThai == "DangLam" || x.MaNhanVien == duLieu.MaNhanVien), "LuaChon", "MaNhanVien", duLieu.MaNhanVien);
            NhanSuGiaoDien.LuaChon(cboCa, bus.CaLamViec().Where(x => x.TrangThai || x.MaCa == duLieu.MaCa), "TenCa", "MaCa", duLieu.MaCa);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("LichLamViec"), duLieu.TrangThai);
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = "Không tải được danh mục. " + ex.Message; }
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", quyen)) return;
        try
        {
            duLieu.MaNhanVien = Convert.ToInt32(cboNhanVien.SelectedValue);
            duLieu.NgayLam = NhanSuQuyUoc.Ngay(dtpNgayLam.SelectedDate, "Ngày làm");
            duLieu.MaCa = Convert.ToInt32(cboCa.SelectedValue);
            duLieu.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai);
            bus.Luu(duLieu);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được lịch làm việc. " + ex.Message; }
    }

}
