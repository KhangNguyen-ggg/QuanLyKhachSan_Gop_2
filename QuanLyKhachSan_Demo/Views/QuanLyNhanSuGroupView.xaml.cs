using System.Windows;
using System.Windows.Controls;

namespace QuanLyKhachSan.Views;

public partial class QuanLyNhanSuGroupView : UserControl
{
    public QuanLyNhanSuGroupView()
    {
        InitializeComponent();
        viewNhanVienView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabNhanVienView.Visibility = DemoSession.CanOpen("NhanVienView") ? Visibility.Visible : Visibility.Collapsed;
        viewNhanSuView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabNhanSuView.Visibility = DemoSession.CanOpen("NhanSuView") ? Visibility.Visible : Visibility.Collapsed;
        tabNhom.SelectedItem = tabNhom.Items.OfType<TabItem>().FirstOrDefault(t => t.Visibility == Visibility.Visible);
        UpdateContextTabs();
    }

    private void TabNhom_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source != tabNhom || viewNhanVienView == null || viewNhanSuView == null) return;
        UpdateContextTabs();
    }

    private void UpdateContextTabs()
    {
        var selection = tabNhom.SelectedItem switch
        {
            TabItem item when item == tabNhanVienView => (View: (UserControl)viewNhanVienView, Index: 0),
            TabItem item when item == tabNhanVien => (View: (UserControl)viewNhanVienView, Index: 0),
            TabItem item when item == tabPhongBan => (View: (UserControl)viewNhanVienView, Index: 1),
            TabItem item when item == tabChucVu => (View: (UserControl)viewNhanVienView, Index: 2),
            TabItem item when item == tabNhanSuView => (View: (UserControl)viewNhanSuView, Index: 0),
            TabItem item when item == tabCaLamViec => (View: (UserControl)viewNhanSuView, Index: 0),
            TabItem item when item == tabLichLamViec => (View: (UserControl)viewNhanSuView, Index: 1),
            TabItem item when item == tabChamCong => (View: (UserControl)viewNhanSuView, Index: 2),
            TabItem item when item == tabBangLuong => (View: (UserControl)viewNhanSuView, Index: 3),
            TabItem item when item == tabKhenThuong => (View: (UserControl)viewNhanSuView, Index: 4),
            _ => (View: (UserControl?)null, Index: 0)
        };
        if (selection.View == null) return;
        viewNhanVienView.Visibility = selection.View == viewNhanVienView && DemoSession.CanOpen("NhanVienView") ? Visibility.Visible : Visibility.Collapsed;
        viewNhanSuView.Visibility = selection.View == viewNhanSuView && DemoSession.CanOpen("NhanSuView") ? Visibility.Visible : Visibility.Collapsed;
        ((TabControl)selection.View.FindName("tabChinh")).SelectedIndex = selection.Index;
        tabCaLamViec.Visibility = selection.View == viewNhanSuView && DemoSession.CanOpen("NhanSuView") ? Visibility.Visible : Visibility.Collapsed;
        tabNhanVien.Visibility = selection.View == viewNhanVienView && DemoSession.CanOpen("NhanVienView") ? Visibility.Visible : Visibility.Collapsed;
        tabPhongBan.Visibility = selection.View == viewNhanVienView && DemoSession.CanOpen("NhanVienView") ? Visibility.Visible : Visibility.Collapsed;
        tabChucVu.Visibility = selection.View == viewNhanVienView && DemoSession.CanOpen("NhanVienView") ? Visibility.Visible : Visibility.Collapsed;
        tabLichLamViec.Visibility = selection.View == viewNhanSuView && DemoSession.CanOpen("NhanSuView") ? Visibility.Visible : Visibility.Collapsed;
        tabChamCong.Visibility = selection.View == viewNhanSuView && DemoSession.CanOpen("NhanSuView") ? Visibility.Visible : Visibility.Collapsed;
        tabBangLuong.Visibility = selection.View == viewNhanSuView && DemoSession.CanOpen("NhanSuView") ? Visibility.Visible : Visibility.Collapsed;
        tabKhenThuong.Visibility = selection.View == viewNhanSuView && DemoSession.CanOpen("NhanSuView") ? Visibility.Visible : Visibility.Collapsed;
        if (tabNhom.SelectedItem == tabNhanVienView) tabNhom.SelectedItem = tabNhanVien;
        if (tabNhom.SelectedItem == tabNhanSuView) tabNhom.SelectedItem = tabCaLamViec;
    }
}
