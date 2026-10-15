using System.Windows;
using System.Windows.Controls;

namespace QuanLyKhachSan.Views;

public partial class KhoVatTuGroupView : UserControl
{
    public KhoVatTuGroupView()
    {
        InitializeComponent();
        viewKhoView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabKhoView.Visibility = DemoSession.CanOpen("KhoView") ? Visibility.Visible : Visibility.Collapsed;
        viewVatTuNhaCungCapView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabVatTuNhaCungCapView.Visibility = DemoSession.CanOpen("VatTuNhaCungCapView") ? Visibility.Visible : Visibility.Collapsed;
        viewTieuHaoView.tabChinh.Template = (ControlTemplate)Resources["ContentOnlyTabs"];
        tabTieuHaoView.Visibility = DemoSession.CanOpen("TieuHaoView") ? Visibility.Visible : Visibility.Collapsed;
        tabNhom.SelectedItem = tabNhom.Items.OfType<TabItem>().FirstOrDefault(t => t.Visibility == Visibility.Visible);
        UpdateContextTabs();
    }

    private void TabNhom_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source != tabNhom || viewKhoView == null || viewVatTuNhaCungCapView == null || viewTieuHaoView == null) return;
        UpdateContextTabs();
    }

    private void UpdateContextTabs()
    {
        var selection = tabNhom.SelectedItem switch
        {
            TabItem item when item == tabKhoView => (View: (UserControl)viewKhoView, Index: 0),
            TabItem item when item == tabTonKho => (View: (UserControl)viewKhoView, Index: 0),
            TabItem item when item == tabPhieuNhapXuat => (View: (UserControl)viewKhoView, Index: 1),
            TabItem item when item == tabKho => (View: (UserControl)viewKhoView, Index: 2),
            TabItem item when item == tabVatTuNhaCungCapView => (View: (UserControl)viewVatTuNhaCungCapView, Index: 0),
            TabItem item when item == tabVatTu => (View: (UserControl)viewVatTuNhaCungCapView, Index: 0),
            TabItem item when item == tabNhomVatTu => (View: (UserControl)viewVatTuNhaCungCapView, Index: 1),
            TabItem item when item == tabNhaCungCap => (View: (UserControl)viewVatTuNhaCungCapView, Index: 2),
            TabItem item when item == tabTieuHaoView => (View: (UserControl)viewTieuHaoView, Index: 0),
            TabItem item when item == tabDinhMucTieuHao => (View: (UserControl)viewTieuHaoView, Index: 0),
            TabItem item when item == tabCapVatTuNoiBo => (View: (UserControl)viewTieuHaoView, Index: 1),
            _ => (View: (UserControl?)null, Index: 0)
        };
        if (selection.View == null) return;
        viewKhoView.Visibility = selection.View == viewKhoView && DemoSession.CanOpen("KhoView") ? Visibility.Visible : Visibility.Collapsed;
        viewVatTuNhaCungCapView.Visibility = selection.View == viewVatTuNhaCungCapView && DemoSession.CanOpen("VatTuNhaCungCapView") ? Visibility.Visible : Visibility.Collapsed;
        viewTieuHaoView.Visibility = selection.View == viewTieuHaoView && DemoSession.CanOpen("TieuHaoView") ? Visibility.Visible : Visibility.Collapsed;
        ((TabControl)selection.View.FindName("tabChinh")).SelectedIndex = selection.Index;
        tabDinhMucTieuHao.Visibility = selection.View == viewTieuHaoView && DemoSession.CanOpen("TieuHaoView") ? Visibility.Visible : Visibility.Collapsed;
        tabVatTu.Visibility = selection.View == viewVatTuNhaCungCapView && DemoSession.CanOpen("VatTuNhaCungCapView") ? Visibility.Visible : Visibility.Collapsed;
        tabTonKho.Visibility = selection.View == viewKhoView && DemoSession.CanOpen("KhoView") ? Visibility.Visible : Visibility.Collapsed;
        tabPhieuNhapXuat.Visibility = selection.View == viewKhoView && DemoSession.CanOpen("KhoView") ? Visibility.Visible : Visibility.Collapsed;
        tabKho.Visibility = selection.View == viewKhoView && DemoSession.CanOpen("KhoView") ? Visibility.Visible : Visibility.Collapsed;
        tabNhomVatTu.Visibility = selection.View == viewVatTuNhaCungCapView && DemoSession.CanOpen("VatTuNhaCungCapView") ? Visibility.Visible : Visibility.Collapsed;
        tabNhaCungCap.Visibility = selection.View == viewVatTuNhaCungCapView && DemoSession.CanOpen("VatTuNhaCungCapView") ? Visibility.Visible : Visibility.Collapsed;
        tabCapVatTuNoiBo.Visibility = selection.View == viewTieuHaoView && DemoSession.CanOpen("TieuHaoView") ? Visibility.Visible : Visibility.Collapsed;
        if (tabNhom.SelectedItem == tabKhoView) tabNhom.SelectedItem = tabTonKho;
        if (tabNhom.SelectedItem == tabVatTuNhaCungCapView) tabNhom.SelectedItem = tabVatTu;
        if (tabNhom.SelectedItem == tabTieuHaoView) tabNhom.SelectedItem = tabDinhMucTieuHao;
    }
}
