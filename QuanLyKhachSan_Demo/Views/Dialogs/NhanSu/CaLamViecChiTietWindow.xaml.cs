using System.Windows;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class CaLamViecChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;

    public CaLamViecChiTietWindow(CaLamViec duLieu)
    {
        InitializeComponent();
        ma = duLieu.MaCa;
        HienThi();
    }

    private void HienThi()
    {
        if (!DemoSession.CanOpen("NhanSuView")) { btnCaLamViecSuaThongTin.IsEnabled = btnCaLamViecXoa.IsEnabled = false; return; }
        var duLieu = bus.CaLamViec().FirstOrDefault(x => x.MaCa == ma)
            ?? throw new InvalidOperationException("Ca làm việc không còn tồn tại.");
        DataContext = duLieu;
        btnCaLamViecSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnCaLamViecSuaThongTin");
        btnCaLamViecXoa.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnCaLamViecXoa");
        txtThongBao.Text = "Nhấn Sửa để mở form chỉnh sửa.";
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnCaLamViecSuaThongTin")) return;
        try
        {
            // Khác Phòng ban: dùng lại form Thêm/Sửa, không lặp phần đọc ô nhập và kiểm tra dữ liệu.
            var form = new CaLamViecForm(bus.TheoId<CaLamViec>(ma)) { Owner = this };
            if (form.ShowDialog() == true) { HienThi(); txtThongBao.Text = "Đã lưu thông tin ca làm việc."; }
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnCaLamViecXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa ca làm việc {ma}")) return;
        try { bus.Xoa<CaLamViec>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được ca làm việc. " + ex.Message; }
    }
}
