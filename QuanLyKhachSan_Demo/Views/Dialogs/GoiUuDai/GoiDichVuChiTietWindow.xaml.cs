using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class GoiDichVuChiTietWindow : Window
{
    private readonly GoiDichVuMau _item;
    private bool _editing;

    public GoiDichVuChiTietWindow(GoiDichVuMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new GoiDichVuMau();
        DataContext = _item;
        dgvThanhPhanGoi.ItemsSource = _item.ThanhPhans;
        btnThanhPhanGoiThemMoi.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnThanhPhanGoiThemMoi");
        btnGoiDichVuThemMoi.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuThemMoi");
        btnGoiDichVuSuaThongTin.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuSuaThongTin");
        btnGoiDichVuXoa.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        pnlLienKet.IsEnabled = !editing;
        if (editing) tabChiTiet.SelectedItem = tabThongTin;
        txtTenGoi.IsReadOnly = !editing;
        txtGiaGoi.IsReadOnly = !editing;
        txtMoTa.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnGoiDichVuThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnGoiDichVuSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnGoiDichVuXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenGoi.Text) ||
            string.IsNullOrWhiteSpace(txtGiaGoi.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtGiaGoi.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueGiaGoi) || valueGiaGoi < 0)
        {
            txtThongBao.Text = "Giá gói: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Gói dịch vụ: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenGoi = txtTenGoi.Text.Trim();
        _item.GiaGoi = txtGiaGoi.Text.Trim();
        _item.MoTa = txtMoTa.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        foreach (var child in _item.ThanhPhans) child.Goi = _item.TenGoi;
        dgvThanhPhanGoi.Items.Refresh();
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Gói dịch vụ: {_item.Ma}?";
        if (_item.ThanhPhans.Count > 0) confirm.txtNoiDung.Text += $" Các dòng thành phần gói thuộc mục này ({_item.ThanhPhans.Count}) cũng sẽ bị xóa.";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnGoiDichVuThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuThemMoi")) return;
        GoiDichVuForm form = new GoiDichVuForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnThemLienKet_Click(object sender, RoutedEventArgs e)
    {
        if (_editing || !DemoSession.CanUseButton("GoiUuDaiView", "btnThanhPhanGoiThemMoi")) return;
        var item = new ThanhPhanGoiMau { Goi = _item.TenGoi, SoLuong = "1", DonGiaThanhPhan = "0" };
        var form = new ThanhPhanGoiChiTietWindow(item, true, true, _item.ThanhPhans) { Owner = this };
        if (form.ShowDialog() == true) _item.ThanhPhans.Add(item);
    }

    private void LienKet_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_editing || !DemoSession.CanOpen("GoiUuDaiView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvThanhPhanGoi, source) is not DataGridRow row || row.Item is not ThanhPhanGoiMau item) return;
        item.Goi = _item.TenGoi;
        var form = new ThanhPhanGoiChiTietWindow(item, false, true, _item.ThanhPhans) { Owner = this };
        if (form.ShowDialog() == true) _item.ThanhPhans.Remove(item);
        dgvThanhPhanGoi.Items.Refresh();
    }
}
