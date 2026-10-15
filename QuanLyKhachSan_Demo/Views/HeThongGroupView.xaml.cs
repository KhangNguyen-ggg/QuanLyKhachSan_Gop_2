using System.Windows;
using System.Windows.Controls;

namespace QuanLyKhachSan.Views;

public partial class HeThongGroupView : UserControl
{
    public HeThongGroupView()
    {
        InitializeComponent();
        viewTaiKhoanView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabTaiKhoanView.Visibility = DemoSession.CanOpen("TaiKhoanView") ? Visibility.Visible : Visibility.Collapsed;
        viewNhatKyView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabNhatKyView.Visibility = DemoSession.CanOpen("NhatKyView") ? Visibility.Visible : Visibility.Collapsed;
        tabNhom.SelectedItem = tabNhom.Items.OfType<TabItem>().FirstOrDefault(t => t.Visibility == Visibility.Visible);
        UpdateContextTabs();
    }

    private void TabNhom_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source != tabNhom || viewTaiKhoanView == null || viewNhatKyView == null) return;
        UpdateContextTabs();
    }

    private void UpdateContextTabs()
    {
        var selection = tabNhom.SelectedItem switch
        {
            TabItem item when item == tabTaiKhoanView => (View: (UserControl)viewTaiKhoanView, Index: 0),
            TabItem item when item == tabTaiKhoan => (View: (UserControl)viewTaiKhoanView, Index: 0),
            TabItem item when item == tabVaiTroQuyen => (View: (UserControl)viewTaiKhoanView, Index: 1),
            TabItem item when item == tabNhatKyView => (View: (UserControl)viewNhatKyView, Index: 0),
            _ => (View: (UserControl?)null, Index: 0)
        };
        if (selection.View == null) return;
        viewTaiKhoanView.Visibility = selection.View == viewTaiKhoanView && DemoSession.CanOpen("TaiKhoanView") ? Visibility.Visible : Visibility.Collapsed;
        viewNhatKyView.Visibility = selection.View == viewNhatKyView && DemoSession.CanOpen("NhatKyView") ? Visibility.Visible : Visibility.Collapsed;
        ((TabControl)selection.View.FindName("tabChinh")).SelectedIndex = selection.Index;
        tabTaiKhoan.Visibility = selection.View == viewTaiKhoanView && DemoSession.CanOpen("TaiKhoanView") ? Visibility.Visible : Visibility.Collapsed;
        tabVaiTroQuyen.Visibility = selection.View == viewTaiKhoanView && DemoSession.CanOpen("TaiKhoanView") ? Visibility.Visible : Visibility.Collapsed;
        if (tabNhom.SelectedItem == tabTaiKhoanView) tabNhom.SelectedItem = tabTaiKhoan;
    }
}
