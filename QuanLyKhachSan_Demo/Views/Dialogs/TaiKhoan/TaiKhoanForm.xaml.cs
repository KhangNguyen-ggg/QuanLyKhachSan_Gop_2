using System.Windows;
using System.Security.Cryptography;
using BUS_QuanlyKhachSan.HeThong;
using BUS_QuanlyKhachSan.LeTanValuuTru;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TaiKhoanForm : Window
{
    private readonly BUS_Taikhoang bus = new();
    private readonly TaiKhoan taiKhoan;
    private readonly string actionId;

    public TaiKhoanForm(TaiKhoan? duLieu = null)
    {
        InitializeComponent();
        taiKhoan = duLieu ?? new TaiKhoan { TrangThai = "HoatDong" };
        actionId = taiKhoan.MaTaiKhoan == 0 ? "btnTaiKhoanThemTaiKhoan" : "btnTaiKhoanSuaTaiKhoan";
        btnLuu.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", actionId);
        DataContext = taiKhoan;
        cboNhanVien.ItemsSource = bus.HienThiNVALL();
        cboVaiTro.ItemsSource = new BUS_VaiTro().hienThiVTAll();
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", actionId)) return;
        try
        {
            // Ghép Hệ thống: gọi ThemTK/SuaTK của Gop; đoạn băm dưới giữ đúng thuật toán, định dạng và số vòng của 5Sao/BUS/HeThong/BUS_Taikhoang.cs.
            string matKhau = pwdMatKhau.Password;
            BUS_KiemTra.YeuCau(matKhau == pwdNhapLaiMatKhau.Password, "Mật khẩu xác nhận không khớp.");
            BUS_KiemTra.YeuCau(taiKhoan.MaTaiKhoan != 0 || matKhau.Length > 0, "Vui lòng nhập mật khẩu.");
            BUS_KiemTra.YeuCau(matKhau.Length <= 1000, "Mật khẩu chỉ được tối đa 1000 ký tự.");
            BUS_KiemTra.Ma(taiKhoan.MaVaiTro, "vai trò");
            if (matKhau.Length > 0)
            {
                byte[] salt = RandomNumberGenerator.GetBytes(16);
                byte[] hash = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, 210000, HashAlgorithmName.SHA256, 32);
                taiKhoan.MatKhauHash = "PBKDF2-SHA256$210000$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
            }
            if (taiKhoan.MaTaiKhoan == 0)
            {
                taiKhoan.NgayTao = DateTime.Now;
                BUS_KiemTra.KetQua(bus.ThemTK(taiKhoan));
            }
            else BUS_KiemTra.KetQua(bus.SuaTK(taiKhoan));
            DialogResult = true;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}
