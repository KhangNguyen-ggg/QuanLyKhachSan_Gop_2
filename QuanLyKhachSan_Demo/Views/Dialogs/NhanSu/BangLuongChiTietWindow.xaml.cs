using System.Windows;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class BangLuongChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;

    public BangLuongChiTietWindow(BangLuong duLieu)
    {
        InitializeComponent();
        ma = duLieu.MaBangLuong;
        HienThi();
    }

    private void HienThi()
    {
        if (!DemoSession.CanOpen("NhanSuView")) { btnBangLuongSuaThongTin.IsEnabled = btnBangLuongXoa.IsEnabled = false; return; }
        var duLieu = bus.BangLuong().FirstOrDefault(x => x.MaBangLuong == ma)
            ?? throw new InvalidOperationException("Bảng lương không còn tồn tại.");
        DataContext = duLieu;
        btnBangLuongSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnBangLuongSuaThongTin") && duLieu.TrangThai != "DaThanhToan";
        btnBangLuongXoa.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnBangLuongXoa") && duLieu.TrangThai != "DaThanhToan";
        txtThongBao.Text = "Bảng lương đã thanh toán chỉ xem. Nhấn Sửa để mở form chỉnh sửa.";
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnBangLuongSuaThongTin")) return;
        try
        {
            // Khác Phòng ban: dùng lại form Thêm/Sửa, không lặp phần đọc ô nhập và kiểm tra dữ liệu.
            var form = new BangLuongForm(bus.TheoId<BangLuong>(ma)) { Owner = this };
            if (form.ShowDialog() == true) { HienThi(); txtThongBao.Text = "Đã lưu thông tin bảng lương."; }
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnBangLuongXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa bảng lương {ma}")) return;
        try { bus.Xoa<BangLuong>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được bảng lương. " + ex.Message; }
    }
}
