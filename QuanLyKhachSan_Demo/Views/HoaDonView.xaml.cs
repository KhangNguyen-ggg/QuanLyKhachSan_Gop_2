using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;
using DAL;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;
public partial class HoaDonView : UserControl
{
    private readonly BUS_HoaDon? bus;

    public HoaDonView()
    {
        InitializeComponent();
        if (CauHinhKetNoi.DaCauHinh) bus = new BUS_HoaDon();
        Loaded += (_, _) => TaiLai();
    }
    private void TaiLai()
    {
        if (bus == null || !DemoSession.CanOpen(nameof(HoaDonView))) return;
        var selected = dgvHoaDon.SelectedValue;
        dgvHoaDon.ItemsSource = null;
        try
        {
            dgvHoaDon.ItemsSource = bus.hienThiHDAll(txtTimHoaDon.Text, Convert.ToString(cboLocHoaDon.SelectedValue));
            dgvHoaDon.SelectedValue = selected;
            if (dgvHoaDon.SelectedIndex < 0 && dgvHoaDon.Items.Count > 0) dgvHoaDon.SelectedIndex = 0;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }

    }
    private void HoaDon_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen(nameof(HoaDonView)) || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvHoaDon, source) is not DataGridRow row) return;
        dgvHoaDon.SelectedItem = row.Item;
        try
        {
            var form = new HoaDonChiTietWindow(BUS_KiemTra.MaDuocChon(dgvHoaDon.SelectedValue)) { DataContext = row.Item, Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnTimHoaDon_Click(object sender, RoutedEventArgs e) => TaiLai();
    private void btnMoiHoaDon_Click(object sender, RoutedEventArgs e)
    {
        txtTimHoaDon.Clear();
        cboLocHoaDon.SelectedIndex = 0;
        TaiLai();
    }

    private void btnHoaDonTaoHoaDon_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(HoaDonView), "btnHoaDonTaoHoaDon")) return;
        try
        {
            var form = new HoaDonForm() { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}