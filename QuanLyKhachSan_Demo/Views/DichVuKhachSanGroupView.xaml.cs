using System.Windows;
using System.Windows.Controls;

namespace QuanLyKhachSan.Views;

public partial class DichVuKhachSanGroupView : UserControl
{
    public DichVuKhachSanGroupView()
    {
        InitializeComponent();
        viewDichVuView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabDichVuView.Visibility = DemoSession.CanOpen("DichVuView") ? Visibility.Visible : Visibility.Collapsed;
        viewGoiUuDaiView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabGoiUuDaiView.Visibility = DemoSession.CanOpen("GoiUuDaiView") ? Visibility.Visible : Visibility.Collapsed;
        tabNhom.SelectedItem = tabNhom.Items.OfType<TabItem>().FirstOrDefault(t => t.Visibility == Visibility.Visible);
        UpdateContextTabs();
    }

    private void TabNhom_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source != tabNhom || viewDichVuView == null || viewGoiUuDaiView == null) return;
        UpdateContextTabs();
    }

    private void UpdateContextTabs()
    {
        var selection = tabNhom.SelectedItem switch
        {
            TabItem item when item == tabDichVuView => (View: (UserControl)viewDichVuView, Index: 0),
            TabItem item when item == tabDichVu => (View: (UserControl)viewDichVuView, Index: 0),
            TabItem item when item == tabSuDungDichVu => (View: (UserControl)viewDichVuView, Index: 1),
            TabItem item when item == tabLoaiDichVu => (View: (UserControl)viewDichVuView, Index: 2),
            TabItem item when item == tabGoiUuDaiView => (View: (UserControl)viewGoiUuDaiView, Index: 0),
            TabItem item when item == tabGoiDichVu => (View: (UserControl)viewGoiUuDaiView, Index: 0),
            TabItem item when item == tabUuDai => (View: (UserControl)viewGoiUuDaiView, Index: 1),
            _ => (View: (UserControl?)null, Index: 0)
        };
        if (selection.View == null) return;
        viewDichVuView.Visibility = selection.View == viewDichVuView && DemoSession.CanOpen("DichVuView") ? Visibility.Visible : Visibility.Collapsed;
        viewGoiUuDaiView.Visibility = selection.View == viewGoiUuDaiView && DemoSession.CanOpen("GoiUuDaiView") ? Visibility.Visible : Visibility.Collapsed;
        if (selection.View.FindName("tabChinh") is TabControl tabs) tabs.SelectedIndex = selection.Index;
        tabDichVu.Visibility = selection.View == viewDichVuView && DemoSession.CanOpen("DichVuView") ? Visibility.Visible : Visibility.Collapsed;
        tabSuDungDichVu.Visibility = selection.View == viewDichVuView && DemoSession.CanOpen("DichVuView") ? Visibility.Visible : Visibility.Collapsed;
        tabLoaiDichVu.Visibility = selection.View == viewDichVuView && DemoSession.CanOpen("DichVuView") ? Visibility.Visible : Visibility.Collapsed;
        tabGoiDichVu.Visibility = selection.View == viewGoiUuDaiView && DemoSession.CanOpen("GoiUuDaiView") ? Visibility.Visible : Visibility.Collapsed;
        tabUuDai.Visibility = selection.View == viewGoiUuDaiView && DemoSession.CanOpen("GoiUuDaiView") ? Visibility.Visible : Visibility.Collapsed;
        if (tabNhom.SelectedItem == tabDichVuView) tabNhom.SelectedItem = tabDichVu;
        if (tabNhom.SelectedItem == tabGoiUuDaiView) tabNhom.SelectedItem = tabGoiDichVu;
    }
}
