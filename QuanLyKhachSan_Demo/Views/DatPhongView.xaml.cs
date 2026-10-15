using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;
using DAL;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;
public partial class DatPhongView : UserControl
{
    private readonly BUS_DatPhong? bus;
    public event EventHandler? DatPhongChanged;
    public DatPhongView()
    {
        InitializeComponent();
        if (CauHinhKetNoi.DaCauHinh) bus = new BUS_DatPhong();
        Loaded += (_, _) => TaiLai();
    }
    private void TaiLai()
    {
        if (bus == null || !DemoSession.CanOpen(nameof(DatPhongView))) return;
        var selected = dgvDatPhong.SelectedValue;
        dgvDatPhong.ItemsSource = null;
        try
        {
            dgvDatPhong.ItemsSource = bus.hienThiDPAll(txtTimDatPhong.Text, Convert.ToString(cboLocDatPhong.SelectedValue));
            dgvDatPhong.SelectedValue = selected;
            if (dgvDatPhong.SelectedIndex < 0 && dgvDatPhong.Items.Count > 0) dgvDatPhong.SelectedIndex = 0;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
        DatPhongChanged?.Invoke(this, EventArgs.Empty);
    }
    private void DatPhong_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen(nameof(DatPhongView)) || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvDatPhong, source) is not DataGridRow row) return;
        dgvDatPhong.SelectedItem = row.Item;
        try
        {
            var form = new DatPhongChiTietWindow(BUS_KiemTra.MaDuocChon(dgvDatPhong.SelectedValue)) { DataContext = row.Item, Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnTimDatPhong_Click(object sender, RoutedEventArgs e) => TaiLai();
    private void btnMoiDatPhong_Click(object sender, RoutedEventArgs e)
    {
        txtTimDatPhong.Clear();
        cboLocDatPhong.SelectedIndex = 0;
        TaiLai();
    }
    internal void RefreshSqlFromGroup() => TaiLai(); private void dgvDatPhong_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
    private void btnDatPhongDatPhongMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(DatPhongView), "btnDatPhongDatPhongMoi")) return;
        try
        {
            var form = new DatPhongForm() { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnDatPhongNhanPhong_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(DatPhongView), "btnDatPhongNhanPhong")) return;
        try
        {
            var form = new KhachLuuTruNhanPhongWindow() { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnDatPhongTraPhong_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(DatPhongView), "btnDatPhongTraPhong")) return;
        try
        {
            var form = new KhachLuuTruTraPhongWindow() { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}