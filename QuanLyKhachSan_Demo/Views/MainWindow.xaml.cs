using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace QuanLyKhachSan.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        lblUser.Text = DemoSession.Current?.Username;
        lblRole.Text = DemoSession.Current?.RoleName;
        var menus = new (RadioButton Button, string View)[]
        {
            (btnTongQuan, nameof(DashboardView)),
            (btnLeTan, nameof(LeTanGroupView)),
            (btnPhongBuong, nameof(PhongBuongGroupView)),
            (btnDichVuKhachSan, nameof(DichVuKhachSanGroupView)),
            (btnKhoVatTu, nameof(KhoVatTuGroupView)),
            (btnQuanLyNhanSu, nameof(QuanLyNhanSuGroupView)),
            (btnHeThong, nameof(HeThongGroupView)),
            (btnBaoCao, nameof(BaoCaoView))
        };
        foreach (var (button, view) in menus)
        {
            button.Visibility = DemoSession.CanOpen(view) ? Visibility.Visible : Visibility.Collapsed;
            button.IsEnabled = DemoSession.CanOpen(view);
            button.IsChecked = false;
        }
        var first = menus.FirstOrDefault(m => m.Button.IsEnabled).Button;
        if (first != null)
        {
            first.IsChecked = true;
            first.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
    }

    private void Navigate<T>(string title) where T : UserControl, new()
    {
        if (!DemoSession.CanOpen(typeof(T).Name)) return;
        var view = new T();
        DemoSession.ApplyAccess(view);
        contentArea.Content = view;
        lblTitle.Text = title;
    }

    private void btnTongQuan_Click(object sender, RoutedEventArgs e) => Navigate<DashboardView>("Tổng quan");
    private void btnLeTan_Click(object sender, RoutedEventArgs e) => Navigate<LeTanGroupView>("Lễ tân & lưu trú");
    private void btnPhongBuong_Click(object sender, RoutedEventArgs e) => Navigate<PhongBuongGroupView>("Phòng");
    private void btnDichVuKhachSan_Click(object sender, RoutedEventArgs e) => Navigate<DichVuKhachSanGroupView>("Dịch vụ & ưu đãi");
    private void btnKhoVatTu_Click(object sender, RoutedEventArgs e) => Navigate<KhoVatTuGroupView>("Kho & vật tư");
    private void btnQuanLyNhanSu_Click(object sender, RoutedEventArgs e) => Navigate<QuanLyNhanSuGroupView>("Nhân sự");
    private void btnHeThong_Click(object sender, RoutedEventArgs e) => Navigate<HeThongGroupView>("Hệ thống");
    private void btnBaoCao_Click(object sender, RoutedEventArgs e) => Navigate<BaoCaoView>("Báo cáo");

    private void btnDangXuat_Click(object sender, RoutedEventArgs e)
    {
        DemoSession.SignOut();
        // Đăng xuất quay về cùng loại form đã chọn tại App.
        var login = App.TaoFormDangNhap();
        Application.Current.MainWindow = login;
        login.Show();
        Close();
    }
}
