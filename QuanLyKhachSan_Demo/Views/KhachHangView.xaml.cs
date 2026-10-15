using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;
using DAL;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;
public partial class KhachHangView : UserControl
{
    private readonly BUS_KhachHang? bus;

    public KhachHangView()
    {
        InitializeComponent();
        if (CauHinhKetNoi.DaCauHinh) bus = new BUS_KhachHang();
        Loaded += (_, _) => TaiLai();
    }
    private void TaiLai()
    {
        if (bus == null || !DemoSession.CanOpen(nameof(KhachHangView))) return;
        var selected = dgvKhachHang.SelectedValue;
        dgvKhachHang.ItemsSource = null;
        try
        {
            dgvKhachHang.ItemsSource = bus.hienThiKHAll(txtTimKhachHang.Text, Convert.ToString(cboLocKhachHang.SelectedValue));
            dgvKhachHang.SelectedValue = selected;
            if (dgvKhachHang.SelectedIndex < 0 && dgvKhachHang.Items.Count > 0) dgvKhachHang.SelectedIndex = 0;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }

    }
    private void KhachHang_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen(nameof(KhachHangView)) || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvKhachHang, source) is not DataGridRow row) return;
        dgvKhachHang.SelectedItem = row.Item;
        try
        {
            var form = new KhachHangChiTietWindow() { DataContext = row.Item, Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnTimKhachHang_Click(object sender, RoutedEventArgs e) => TaiLai();
    private void btnMoiKhachHang_Click(object sender, RoutedEventArgs e)
    {
        txtTimKhachHang.Clear();
        cboLocKhachHang.SelectedIndex = 0;
        TaiLai();
    }

    private void btnKhachHangThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(KhachHangView), "btnKhachHangThemMoi")) return;
        try
        {
            var form = new KhachHangForm() { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}