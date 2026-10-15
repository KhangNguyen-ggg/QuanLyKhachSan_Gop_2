using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhanVienForm : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly NhanVien duLieu;
    private readonly string quyen;

    public NhanVienForm() : this(null) { }

    // Thêm: một form dùng lại cho Thêm và Sửa; bản nháp giúp đóng/hủy không đổi dữ liệu gốc.
    public NhanVienForm(NhanVien? banGoc)
    {
        InitializeComponent();
        duLieu = banGoc == null ? new NhanVien { TrangThai = "DangLam", GioiTinh = "M", NgayVaoLam = DateTime.Today } : NhanSuGiaoDien.BanNhap(banGoc);
        quyen = duLieu.MaNhanVien == 0 ? "btnNhanVienThemMoi" : "btnNhanVienSuaThongTin";
        DataContext = duLieu;
        txtTieuDe.Text = (duLieu.MaNhanVien == 0 ? "Thêm — " : "Sửa — ") + "Nhân viên";
        btnLuu.IsEnabled = DemoSession.CanUseButton("NhanVienView", quyen);
        if (!DemoSession.CanOpen("NhanVienView")) return;
        try
        {
            NhanSuGiaoDien.DanhMuc(cboGioiTinh, new KeyValuePair<string, string>[] { new("M", "Nam"), new("F", "Nữ"), new("O", "Khác") }, duLieu.GioiTinh ?? "");
            NhanSuGiaoDien.LuaChon(cboPhongBan, bus.PhongBan().Where(x => x.TrangThai || x.MaPhongBan == duLieu.MaPhongBan), "TenPhongBan", "MaPhongBan", duLieu.MaPhongBan);
            NhanSuGiaoDien.LuaChon(cboChucVu, bus.ChucVu().Where(x => x.TrangThai || x.MaChucVu == duLieu.MaChucVu), "TenChucVu", "MaChucVu", duLieu.MaChucVu);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("NhanVien"), duLieu.TrangThai);
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = "Không tải được danh mục. " + ex.Message; }
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", quyen)) return;
        try
        {
            duLieu.GioiTinh = NhanSuGiaoDien.TrangThai(cboGioiTinh);
            duLieu.NgaySinh = dtpNgaySinh.SelectedDate?.Date;
            duLieu.MaPhongBan = Convert.ToInt32(cboPhongBan.SelectedValue);
            duLieu.MaChucVu = Convert.ToInt32(cboChucVu.SelectedValue);
            duLieu.NgayVaoLam = dtpNgayVaoLam.SelectedDate?.Date;
            duLieu.LuongCoBan = BUS_KiemTra.SoTien(txtLuongCoBanD.Text, "Lương cơ bản");
            duLieu.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai);
            bus.Luu(duLieu);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được nhân viên. " + ex.Message; }
    }

}
