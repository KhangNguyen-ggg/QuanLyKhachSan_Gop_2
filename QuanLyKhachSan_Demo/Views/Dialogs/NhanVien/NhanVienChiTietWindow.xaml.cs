using System.Windows;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhanVienChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;

    public NhanVienChiTietWindow(NhanVien duLieu)
    {
        InitializeComponent();
        ma = duLieu.MaNhanVien;
        HienThi();
    }

    private void HienThi()
    {
        if (!DemoSession.CanOpen("NhanVienView")) { btnNhanVienSuaThongTin.IsEnabled = btnNhanVienXoa.IsEnabled = false; return; }
        var duLieu = bus.NhanVien().FirstOrDefault(x => x.MaNhanVien == ma)
            ?? throw new InvalidOperationException("Nhân viên không còn tồn tại.");
        DataContext = duLieu;
        btnNhanVienSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnNhanVienSuaThongTin");
        btnNhanVienXoa.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnNhanVienXoa");
        txtThongBao.Text = "Nhấn Sửa để mở form chỉnh sửa.";
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnNhanVienSuaThongTin")) return;
        try
        {
            // Khác Phòng ban: dùng lại form Thêm/Sửa, không lặp phần đọc ô nhập và kiểm tra dữ liệu.
            var form = new NhanVienForm(bus.TheoId<NhanVien>(ma)) { Owner = this };
            if (form.ShowDialog() == true) { HienThi(); txtThongBao.Text = "Đã lưu thông tin nhân viên."; }
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnNhanVienXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa nhân viên {ma}")) return;
        try { bus.Xoa<NhanVien>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được nhân viên. " + ex.Message; }
    }
}
