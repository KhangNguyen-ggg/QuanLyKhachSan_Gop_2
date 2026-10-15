using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;
using DAL;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;
public partial class LuuTruView : UserControl
{
    private readonly BUS_KhachTheoPhong? bus;

    public LuuTruView()
    {
        InitializeComponent();
        if (CauHinhKetNoi.DaCauHinh) bus = new BUS_KhachTheoPhong();
        Loaded += (_, _) => TaiLai();
    }
    private void TaiLai()
    {
        if (bus == null || !DemoSession.CanOpen(nameof(LuuTruView))) return;
        var selected = dgvKhachLuuTru.SelectedValue;
        dgvKhachLuuTru.ItemsSource = null;
        try
        {
            dgvKhachLuuTru.ItemsSource = bus.hienThiKTPAll(txtTimKhachLuuTru.Text);
            dgvKhachLuuTru.SelectedValue = selected;
            if (dgvKhachLuuTru.SelectedIndex < 0 && dgvKhachLuuTru.Items.Count > 0) dgvKhachLuuTru.SelectedIndex = 0;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }

    }
    private void KhachLuuTru_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen(nameof(LuuTruView)) || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvKhachLuuTru, source) is not DataGridRow row) return;
        dgvKhachLuuTru.SelectedItem = row.Item;
        try
        {
            var form = new KhachLuuTruChiTietWindow() { DataContext = row.Item, Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnTimKhachLuuTru_Click(object sender, RoutedEventArgs e) => TaiLai();
    private void btnMoiKhachLuuTru_Click(object sender, RoutedEventArgs e)
    {
        txtTimKhachLuuTru.Clear();
        cboLocKhachLuuTru.SelectedIndex = 0;
        TaiLai();
    }

    private void btnKhachLuuTruThemKhachLuuTru_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(LuuTruView), "btnKhachLuuTruThemKhachLuuTru")) return;
        try
        {
            var form = new KhachLuuTruNhanPhongWindow() { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}