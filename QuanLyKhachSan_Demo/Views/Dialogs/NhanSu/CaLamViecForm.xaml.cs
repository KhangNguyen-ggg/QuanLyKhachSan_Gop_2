using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class CaLamViecForm : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly CaLamViec duLieu;
    private readonly string quyen;

    public CaLamViecForm() : this(null) { }

    // Thêm: một form dùng lại cho Thêm và Sửa; bản nháp giúp đóng/hủy không đổi dữ liệu gốc.
    public CaLamViecForm(CaLamViec? banGoc)
    {
        InitializeComponent();
        duLieu = banGoc == null ? new CaLamViec { TrangThai = true, GioBatDau = TimeSpan.FromHours(6), GioKetThuc = TimeSpan.FromHours(14) } : NhanSuGiaoDien.BanNhap(banGoc);
        quyen = duLieu.MaCa == 0 ? "btnCaLamViecThemMoi" : "btnCaLamViecSuaThongTin";
        DataContext = duLieu;
        txtTieuDe.Text = (duLieu.MaCa == 0 ? "Thêm — " : "Sửa — ") + "Ca làm việc";
        btnLuu.IsEnabled = DemoSession.CanUseButton("NhanSuView", quyen);
        if (!DemoSession.CanOpen("NhanSuView")) return;
        try
        {
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("CaLamViec"), duLieu.TrangThai.ToString());
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = "Không tải được danh mục. " + ex.Message; }
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", quyen)) return;
        try
        {
            duLieu.GioBatDau = NhanSuQuyUoc.Gio(txtGioBatDau.Text, "Giờ bắt đầu");
            duLieu.GioKetThuc = NhanSuQuyUoc.Gio(txtGioKetThuc.Text, "Giờ kết thúc");
            duLieu.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai) == "True";
            bus.Luu(duLieu);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được ca làm việc. " + ex.Message; }
    }

}
