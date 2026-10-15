using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;
using DAL;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;
public partial class LichSuLuuTruView : UserControl
{
    private readonly BUS_LichSuXepPhong? bus;

    public LichSuLuuTruView()
    {
        InitializeComponent();
        if (CauHinhKetNoi.DaCauHinh) bus = new BUS_LichSuXepPhong();
        Loaded += (_, _) => TaiLai();
    }
    private void TaiLai()
    {
        if (bus == null || !DemoSession.CanOpen(nameof(LichSuLuuTruView))) return;
        var selected = dgvLichSuLuuTru.SelectedValue;
        dgvLichSuLuuTru.ItemsSource = null;
        try
        {
            dgvLichSuLuuTru.ItemsSource = bus.hienThiLSXPAll(txtTimLichSuLuuTru.Text);
            dgvLichSuLuuTru.SelectedValue = selected;
            if (dgvLichSuLuuTru.SelectedIndex < 0 && dgvLichSuLuuTru.Items.Count > 0) dgvLichSuLuuTru.SelectedIndex = 0;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }

    }
    private void LichSuLuuTru_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen(nameof(LichSuLuuTruView)) || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvLichSuLuuTru, source) is not DataGridRow row) return;
        dgvLichSuLuuTru.SelectedItem = row.Item;
        try
        {
            var form = new LichSuLuuTruChiTietWindow() { DataContext = row.Item, Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnTimLichSuLuuTru_Click(object sender, RoutedEventArgs e) => TaiLai();
    private void btnMoiLichSuLuuTru_Click(object sender, RoutedEventArgs e)
    {
        txtTimLichSuLuuTru.Clear();
        cboLocLichSuLuuTru.SelectedIndex = 0;
        TaiLai();
    }

}