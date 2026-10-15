using System.Windows;
using System.Windows.Controls;

namespace QuanLyKhachSan.Views;

public partial class PhongBuongGroupView : UserControl
{
    public PhongBuongGroupView()
    {
        InitializeComponent();
        tabPhongView.Visibility = DemoSession.CanOpen("PhongView") ? Visibility.Visible : Visibility.Collapsed;
        viewBuongPhongView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabBuongPhongView.Visibility = DemoSession.CanOpen("BuongPhongView") ? Visibility.Visible : Visibility.Collapsed;
        viewDanhMucPhongView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabDanhMucPhongView.Visibility = DemoSession.CanOpen("DanhMucPhongView") ? Visibility.Visible : Visibility.Collapsed;
        tabNhom.SelectedItem = tabNhom.Items.OfType<TabItem>().FirstOrDefault(t => t.Visibility == Visibility.Visible);
        UpdateContextTabs();
    }

    private void TabNhom_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source != tabNhom || viewPhongView == null || viewBuongPhongView == null || viewDanhMucPhongView == null) return;
        UpdateContextTabs();
    }

    private void UpdateContextTabs()
    {
        var selection = tabNhom.SelectedItem switch
        {
            TabItem item when item == tabPhongView => (View: (UserControl)viewPhongView, Index: 0),
            TabItem item when item == tabBuongPhongView => (View: (UserControl)viewBuongPhongView, Index: 0),
            TabItem item when item == tabDonPhong => (View: (UserControl)viewBuongPhongView, Index: 0),
            TabItem item when item == tabBaoTri => (View: (UserControl)viewBuongPhongView, Index: 1),
            TabItem item when item == tabThietBi => (View: (UserControl)viewBuongPhongView, Index: 2),
            TabItem item when item == tabDanhMucPhongView => (View: (UserControl)viewDanhMucPhongView, Index: 0),
            TabItem item when item == tabLoaiPhong => (View: (UserControl)viewDanhMucPhongView, Index: 1),
            TabItem item when item == tabTienNghi => (View: (UserControl)viewDanhMucPhongView, Index: 2),
            TabItem item when item == tabCauTrucKhachSan => (View: (UserControl)viewDanhMucPhongView, Index: 0),
            _ => (View: (UserControl?)null, Index: 0)
        };
        if (selection.View == null) return;
        viewPhongView.Visibility = selection.View == viewPhongView && DemoSession.CanOpen("PhongView") ? Visibility.Visible : Visibility.Collapsed;
        viewBuongPhongView.Visibility = selection.View == viewBuongPhongView && DemoSession.CanOpen("BuongPhongView") ? Visibility.Visible : Visibility.Collapsed;
        viewDanhMucPhongView.Visibility = selection.View == viewDanhMucPhongView && DemoSession.CanOpen("DanhMucPhongView") ? Visibility.Visible : Visibility.Collapsed;
        if (selection.View.FindName("tabChinh") is TabControl tabs) tabs.SelectedIndex = selection.Index;
        tabDonPhong.Visibility = selection.View == viewBuongPhongView && DemoSession.CanOpen("BuongPhongView") ? Visibility.Visible : Visibility.Collapsed;
        tabBaoTri.Visibility = selection.View == viewBuongPhongView && DemoSession.CanOpen("BuongPhongView") ? Visibility.Visible : Visibility.Collapsed;
        tabThietBi.Visibility = selection.View == viewBuongPhongView && DemoSession.CanOpen("BuongPhongView") ? Visibility.Visible : Visibility.Collapsed;
        tabLoaiPhong.Visibility = selection.View == viewDanhMucPhongView && DemoSession.CanOpen("DanhMucPhongView") ? Visibility.Visible : Visibility.Collapsed;
        tabTienNghi.Visibility = selection.View == viewDanhMucPhongView && DemoSession.CanOpen("DanhMucPhongView") ? Visibility.Visible : Visibility.Collapsed;
        if (tabNhom.SelectedItem == tabBuongPhongView) tabNhom.SelectedItem = tabDonPhong;
        tabCauTrucKhachSan.Visibility = selection.View == viewDanhMucPhongView && DemoSession.CanOpen("DanhMucPhongView") ? Visibility.Visible : Visibility.Collapsed;
        if (tabNhom.SelectedItem == tabDanhMucPhongView) tabNhom.SelectedItem = tabCauTrucKhachSan;
    }
}
