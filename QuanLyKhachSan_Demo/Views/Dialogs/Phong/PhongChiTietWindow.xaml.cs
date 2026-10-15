using System.Windows;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class PhongChiTietWindow : Window
{
    private readonly PhongMau _room;
    public PhongChiTietWindow(PhongMau? room = null)
    {
        InitializeComponent();
        DataContext = _room = room ?? new PhongMau();
        Title = $"Chi tiết phòng {_room.SoPhong}";
        btnPhongSuaThongTin.IsEnabled = DemoSession.CanUseButton("PhongView", "btnPhongSuaThongTin");
        btnPhongXoa.IsEnabled = DemoSession.CanUseButton("PhongView", "btnPhongXoa");
        btnTinhTrangPhongCapNhatTinhTrang.IsEnabled = DemoSession.CanUseButton("PhongView", "btnTinhTrangPhongCapNhatTinhTrang");
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnPhongSuaThongTin_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("PhongView", "btnPhongSuaThongTin")) return;
        if (new PhongForm(_room) { Owner = this }.ShowDialog() != true) return;
        DataContext = null;
        DataContext = _room;
    }
    private void btnTinhTrangPhongCapNhatTinhTrang_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("PhongView", "btnTinhTrangPhongCapNhatTinhTrang")) return;
        if (new TinhTrangPhongForm(_room) { Owner = this }.ShowDialog() != true) return;
        DataContext = null;
        DataContext = _room;
    }
    private void btnPhongXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("PhongView", "btnPhongXoa")) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa phòng {_room.SoPhong} ({_room.Ma})?";
        if (confirm.ShowDialog() == true) DialogResult = true;
    }
}
