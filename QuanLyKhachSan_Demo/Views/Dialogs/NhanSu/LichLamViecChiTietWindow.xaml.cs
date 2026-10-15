using System.Windows;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class LichLamViecChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;

    public LichLamViecChiTietWindow(LichLamViec duLieu)
    {
        InitializeComponent();
        ma = duLieu.MaLich;
        HienThi();
    }

    private void HienThi()
    {
        if (!DemoSession.CanOpen("NhanSuView")) { btnLichLamViecSuaThongTin.IsEnabled = btnLichLamViecXoa.IsEnabled = false; return; }
        var duLieu = bus.LichLamViec().FirstOrDefault(x => x.MaLich == ma)
            ?? throw new InvalidOperationException("Lịch làm việc không còn tồn tại.");
        DataContext = duLieu;
        btnLichLamViecSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnLichLamViecSuaThongTin");
        btnLichLamViecXoa.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnLichLamViecXoa");
        txtThongBao.Text = "Nhấn Sửa để mở form chỉnh sửa.";
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnLichLamViecSuaThongTin")) return;
        try
        {
            // Khác Phòng ban: dùng lại form Thêm/Sửa, không lặp phần đọc ô nhập và kiểm tra dữ liệu.
            var form = new LichLamViecForm(bus.TheoId<LichLamViec>(ma)) { Owner = this };
            if (form.ShowDialog() == true) { HienThi(); txtThongBao.Text = "Đã lưu thông tin lịch làm việc."; }
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnLichLamViecXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa lịch làm việc {ma}")) return;
        try { bus.Xoa<LichLamViec>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được lịch làm việc. " + ex.Message; }
    }
}
