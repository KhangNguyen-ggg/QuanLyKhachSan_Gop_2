using System.Windows;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class PhongBanChiTietWindow : Window
{
    private readonly BUS_PhongBan bus = new();
    private PhongBan phongBan;
    private bool _editing;

    public PhongBanChiTietWindow(PhongBan duLieu)
    {
        InitializeComponent();
        phongBan = duLieu;
        btnPhongBanThemMoi.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnPhongBanThemMoi");
        btnPhongBanSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin");
        HienThiPhongBan();
    }

    private void HienThiPhongBan()
    {
        // Sửa trên bản nháp để Hủy sửa hoặc Đóng không đổi dữ liệu đang hiển thị.
        DataContext = new PhongBan { MaPhongBan = phongBan.MaPhongBan, TenPhongBan = phongBan.TenPhongBan,
            MoTa = phongBan.MoTa, TrangThai = phongBan.TrangThai };
        SetEditMode(false);
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin")) return;
        HienThiPhongBan();
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenPhongBan.IsReadOnly = !editing;
        txtMoTa.IsReadOnly = !editing;
        cboTrangThai.IsEnabled = editing;
        btnPhongBanThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnPhongBanSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnPhongBanXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin")) return;
        try
        {
            var duLieu = (PhongBan)DataContext;
            bus.Luu(duLieu);
            phongBan = duLieu;
            HienThiPhongBan();
            txtThongBao.Text = "Đã lưu thông tin phòng ban.";
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

    private void btnPhongBanThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnPhongBanThemMoi")) return;
        try
        {
            var form = new PhongBanForm { Owner = this };
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}
