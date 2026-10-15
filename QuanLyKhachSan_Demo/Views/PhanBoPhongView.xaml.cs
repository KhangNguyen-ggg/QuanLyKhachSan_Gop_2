using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;
using DAL;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;

public partial class PhanBoPhongView : UserControl
{
    private readonly Bus_PhanBoPhong? bus;
    private DataRowView? order;
    public event EventHandler? PhanBoChanged;

    public PhanBoPhongView()
    {
        InitializeComponent();
        if (CauHinhKetNoi.DaCauHinh) bus = new Bus_PhanBoPhong();
        SetDatPhong(null);
    }

    public void SetDatPhong(DataRowView? selected)
    {
        order = selected;
        txtDonDangChon.Text = order == null ? "Chọn một đơn trong Đặt phòng để xem phân bổ." :
            $"{order["MaDatPhong"]} — {order["HoTen"]}";
        btnPhanBoPhongThemPhong.IsEnabled = order != null &&
            Convert.ToString(order["TrangThai"]) is "ChoXacNhan" or "DaXacNhan" or "DaCheckIn" &&
            DemoSession.CanUseButton(nameof(PhanBoPhongView), "btnPhanBoPhongThemPhong");
        TaiLai();
    }

    private void TaiLai()
    {
        dgvPhanBoPhong.ItemsSource = null;
        if (bus == null || order == null || !DemoSession.CanOpen(nameof(PhanBoPhongView))) return;
        try
        {
            var rows = bus.HienThiPBP(txtTimPhanBoPhong.Text, Convert.ToString(cboLocPhanBoPhong.SelectedValue));
            rows.RowFilter = $"MaDatPhong = {Convert.ToInt64(order["MaDatPhong"])}";
            dgvPhanBoPhong.ItemsSource = rows;
            if (rows.Count > 0) dgvPhanBoPhong.SelectedIndex = 0;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnPhanBoPhongThemPhong_Click(object sender, RoutedEventArgs e)
    {
        if (order == null || !btnPhanBoPhongThemPhong.IsEnabled ||
            !DemoSession.CanUseButton(nameof(PhanBoPhongView), "btnPhanBoPhongThemPhong")) return;
        try
        {
            var form = new PhanBoPhongForm(0, Convert.ToInt64(order["MaDatPhong"])) { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            PhanBoChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void PhanBoPhong_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen(nameof(PhanBoPhongView)) || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvPhanBoPhong, source) is not DataGridRow row) return;
        try
        {
            var form = new PhanBoPhongChiTietWindow { DataContext = row.Item, Owner = Window.GetWindow(this) };
            form.ShowDialog();
            PhanBoChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnTimPhanBoPhong_Click(object sender, RoutedEventArgs e) => TaiLai();
    private void btnMoiPhanBoPhong_Click(object sender, RoutedEventArgs e)
    {
        txtTimPhanBoPhong.Clear();
        cboLocPhanBoPhong.SelectedIndex = 0;
        TaiLai();
    }
}