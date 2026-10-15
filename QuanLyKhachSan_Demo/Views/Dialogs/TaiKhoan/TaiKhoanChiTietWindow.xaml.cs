using System.Windows;
using BUS_QuanlyKhachSan.HeThong;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TaiKhoanChiTietWindow : Window
{
    private readonly BUS_Taikhoang bus = new();
    private int Ma => Convert.ToInt32(((dynamic)DataContext).MaTaiKhoan);

    public TaiKhoanChiTietWindow()
    {
        InitializeComponent();
        btnTaiKhoanThemTaiKhoan.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanThemTaiKhoan");
        btnTaiKhoanSuaTaiKhoan.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanSuaTaiKhoan");
        btnTaiKhoanGanVaiTro.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanGanVaiTro");
        btnTaiKhoanDatLaiMatKhau.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanDatLaiMatKhau");
        btnTaiKhoanKhoaTaiKhoan.IsEnabled = false;
        btnTaiKhoanMoKhoa.IsEnabled = false;
        DataContextChanged += (_, _) =>
        {
            string status = DataContext == null ? "" : Convert.ToString(((dynamic)DataContext).TrangThai);
            btnTaiKhoanKhoaTaiKhoan.IsEnabled = status == "HoatDong" && DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanKhoaTaiKhoan");
            btnTaiKhoanMoKhoa.IsEnabled = status == "Khoa" && DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanMoKhoa");
        };
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnTaiKhoanThemTaiKhoan_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanThemTaiKhoan")) return;
        try
        {
            var form = new TaiKhoanForm { Owner = this };
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnTaiKhoanSuaTaiKhoan_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanSuaTaiKhoan")) return;
        try
        {
            var form = new TaiKhoanForm(bus.HienThiTK(Ma)) { Owner = this };
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnTaiKhoanKhoaTaiKhoan_Click(object sender, RoutedEventArgs e)
    {
        if (!btnTaiKhoanKhoaTaiKhoan.IsEnabled || !DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanKhoaTaiKhoan")) return;
        try
        {
            if (MessageBox.Show("Khóa tài khoản này?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            // Ghép Gop: BUS cũ trả false khi lỗi; chỉ đóng chi tiết sau khi lưu thành công.
            if (!bus.Khoa(Ma)) throw new InvalidOperationException("Không khóa được tài khoản. Vui lòng làm mới và thử lại.");
            Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnTaiKhoanMoKhoa_Click(object sender, RoutedEventArgs e)
    {
        if (!btnTaiKhoanMoKhoa.IsEnabled || !DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanMoKhoa")) return;
        try
        {
            if (MessageBox.Show("Mở khóa tài khoản này?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            // Ghép Gop: mo trả "damo" hoặc thông báo lỗi, không trả bool như bản giao diện.
            string ketQua = bus.mo(Ma);
            if (ketQua != "damo") throw new InvalidOperationException(ketQua);
            Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnTaiKhoanGanVaiTro_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanGanVaiTro")) return;
        try
        {
            var form = new TaiKhoanGanVaiTroWindow(bus.HienThiTK(Ma), new BUS_VaiTro().hienThiVTAll()) { Owner = this };
            form.ShowDialog();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnTaiKhoanDatLaiMatKhau_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnTaiKhoanDatLaiMatKhau")) return;
        try
        {
            var form = new TaiKhoanDatLaiMatKhauWindow(bus.HienThiTK(Ma)) { Owner = this };
            form.ShowDialog();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}
