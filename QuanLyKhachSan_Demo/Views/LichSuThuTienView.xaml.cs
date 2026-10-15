using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;
using DAL;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;
public partial class LichSuThuTienView : UserControl
{
    private readonly BUS_LichSuThuTien? bus;

    public LichSuThuTienView()
    {
        InitializeComponent();
        if (CauHinhKetNoi.DaCauHinh) bus = new BUS_LichSuThuTien();
        Loaded += (_, _) => TaiLai();
    }
    private void TaiLai()
    {
        if (bus == null || !DemoSession.CanOpen(nameof(LichSuThuTienView))) return;
        var selected = dgvLichSuThuTien.SelectedValue;
        dgvLichSuThuTien.ItemsSource = null;
        try
        {
            dgvLichSuThuTien.ItemsSource = bus.hienThiLSTTAll(txtTimLichSuThuTien.Text);
            dgvLichSuThuTien.SelectedValue = selected;
            if (dgvLichSuThuTien.SelectedIndex < 0 && dgvLichSuThuTien.Items.Count > 0) dgvLichSuThuTien.SelectedIndex = 0;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }

    }
    private void LichSuThuTien_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen(nameof(LichSuThuTienView)) || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvLichSuThuTien, source) is not DataGridRow row) return;
        dgvLichSuThuTien.SelectedItem = row.Item;
        try
        {
            var form = new LichSuThuTienChiTietWindow() { DataContext = row.Item, Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnTimLichSuThuTien_Click(object sender, RoutedEventArgs e) => TaiLai();
    private void btnMoiLichSuThuTien_Click(object sender, RoutedEventArgs e)
    {
        txtTimLichSuThuTien.Clear();
        cboLocLichSuThuTien.SelectedIndex = 0;
        TaiLai();
    }

}