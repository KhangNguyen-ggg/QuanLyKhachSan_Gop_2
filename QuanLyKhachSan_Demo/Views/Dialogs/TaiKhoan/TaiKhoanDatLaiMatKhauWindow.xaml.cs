using System.Windows;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TaiKhoanDatLaiMatKhauWindow : Window
{
    public TaiKhoanDatLaiMatKhauWindow(TaiKhoan? taiKhoan = null)
    {
        InitializeComponent();
        DataContext = taiKhoan;
        if (taiKhoan != null) txtTieuDe.Text = $"Đặt lại mật khẩu — {taiKhoan.TenDangNhap}";
        btnLuu.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanDatLaiMatKhau");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanDatLaiMatKhau")) return;
        // shortcut: giữ form đặt lại mật khẩu, nối thao tác lưu khi triển khai chức năng.
        txtThongBao.Text = "Chức năng đặt lại mật khẩu chưa được triển khai. Thay đổi chưa được lưu.";
    }
}