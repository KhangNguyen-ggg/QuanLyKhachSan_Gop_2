using System.Windows;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TaiKhoanGanVaiTroWindow : Window
{
    public TaiKhoanGanVaiTroWindow(TaiKhoan? taiKhoan = null, List<VaiTro>? vaiTros = null)
    {
        InitializeComponent();
        DataContext = taiKhoan;
        cboVaiTro.ItemsSource = vaiTros ?? new List<VaiTro>();
        cboVaiTro.SelectedValue = taiKhoan?.MaVaiTro;
        if (taiKhoan != null) txtTieuDe.Text = $"Gán vai trò — {taiKhoan.TenDangNhap}";
        btnLuu.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanGanVaiTro");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanGanVaiTro")) return;
        // shortcut: giữ form gán vai trò riêng, nối thao tác lưu khi triển khai chức năng.
        txtThongBao.Text = "Chức năng gán vai trò chưa được triển khai. Thay đổi chưa được lưu.";
    }
}