using System.Windows;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class KhenThuongChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;

    public KhenThuongChiTietWindow(KhenThuong duLieu)
    {
        InitializeComponent();
        ma = duLieu.MaKhenThuong;
        HienThi();
    }

    private void HienThi()
    {
        if (!DemoSession.CanOpen("NhanSuView")) { btnKhenThuongSuaThongTin.IsEnabled = btnKhenThuongXoa.IsEnabled = false; return; }
        var duLieu = bus.KhenThuong().FirstOrDefault(x => x.MaKhenThuong == ma)
            ?? throw new InvalidOperationException("Khen thưởng không còn tồn tại.");
        DataContext = duLieu;
        btnKhenThuongSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnKhenThuongSuaThongTin");
        btnKhenThuongXoa.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnKhenThuongXoa");
        txtThongBao.Text = "Nhấn Sửa để mở form chỉnh sửa.";
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnKhenThuongSuaThongTin")) return;
        try
        {
            // Khác Phòng ban: dùng lại form Thêm/Sửa, không lặp phần đọc ô nhập và kiểm tra dữ liệu.
            var form = new KhenThuongForm(bus.TheoId<KhenThuong>(ma)) { Owner = this };
            if (form.ShowDialog() == true) { HienThi(); txtThongBao.Text = "Đã lưu thông tin khen thưởng."; }
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnKhenThuongXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa khen thưởng {ma}")) return;
        try { bus.Xoa<KhenThuong>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được khen thưởng. " + ex.Message; }
    }
}
