using System.Windows;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ChamCongChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;

    public ChamCongChiTietWindow(ChamCong duLieu)
    {
        InitializeComponent();
        ma = duLieu.MaChamCong;
        HienThi();
    }

    private void HienThi()
    {
        if (!DemoSession.CanOpen("NhanSuView")) { btnChamCongSuaThongTin.IsEnabled = btnChamCongXoa.IsEnabled = false; return; }
        var duLieu = bus.ChamCong().FirstOrDefault(x => x.MaChamCong == ma)
            ?? throw new InvalidOperationException("Chấm công không còn tồn tại.");
        DataContext = duLieu;
        btnChamCongSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnChamCongSuaThongTin");
        btnChamCongXoa.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnChamCongXoa");
        txtThongBao.Text = "Nhấn Sửa để mở form chỉnh sửa.";
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnChamCongSuaThongTin")) return;
        try
        {
            // Khác Phòng ban: dùng lại form Thêm/Sửa, không lặp phần đọc ô nhập và kiểm tra dữ liệu.
            var form = new ChamCongForm(bus.TheoId<ChamCong>(ma)) { Owner = this };
            if (form.ShowDialog() == true) { HienThi(); txtThongBao.Text = "Đã lưu thông tin chấm công."; }
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnChamCongXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa chấm công {ma}")) return;
        try { bus.Xoa<ChamCong>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được chấm công. " + ex.Message; }
    }
}
