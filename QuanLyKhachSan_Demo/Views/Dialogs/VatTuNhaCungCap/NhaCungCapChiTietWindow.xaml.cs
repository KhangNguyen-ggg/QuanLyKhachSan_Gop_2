using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhaCungCapChiTietWindow : Window
{
    private readonly NhaCungCapMau _item;
    private bool _editing;

    public NhaCungCapChiTietWindow(NhaCungCapMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new NhaCungCapMau();
        DataContext = _item;
        dgvVatTuNhaCungCap.ItemsSource = _item.VatTus;
        btnVatTuNhaCungCapThemMoi.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuNhaCungCapThemMoi");
        btnNhaCungCapThemMoi.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapThemMoi");
        btnNhaCungCapSuaThongTin.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapSuaThongTin");
        btnNhaCungCapXoa.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        pnlLienKet.IsEnabled = !editing;
        if (editing) tabChiTiet.SelectedItem = tabThongTin;
        txtTenNhaCungCap.IsReadOnly = !editing;
        txtNguoiDaiDien.IsReadOnly = !editing;
        txtDienThoai.IsReadOnly = !editing;
        txtEmail.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnNhaCungCapThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnNhaCungCapSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnNhaCungCapXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenNhaCungCap.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Nhà cung cấp: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenNhaCungCap = txtTenNhaCungCap.Text.Trim();
        _item.NguoiDaiDien = txtNguoiDaiDien.Text.Trim();
        _item.DienThoai = txtDienThoai.Text.Trim();
        _item.Email = txtEmail.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        foreach (var child in _item.VatTus) child.NhaCungCap = _item.TenNhaCungCap;
        dgvVatTuNhaCungCap.Items.Refresh();
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Nhà cung cấp: {_item.Ma}?";
        if (_item.VatTus.Count > 0) confirm.txtNoiDung.Text += $" Các dòng vật tư cung cấp thuộc mục này ({_item.VatTus.Count}) cũng sẽ bị xóa.";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnNhaCungCapThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapThemMoi")) return;
        NhaCungCapForm form = new NhaCungCapForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnThemLienKet_Click(object sender, RoutedEventArgs e)
    {
        if (_editing || !DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuNhaCungCapThemMoi")) return;
        var item = new VatTuNhaCungCapMau { NhaCungCap = _item.TenNhaCungCap, GiaNhap = "0", TrangThai = "Đang hoạt động" };
        var form = new VatTuNhaCungCapChiTietWindow(item, true, true, _item.VatTus) { Owner = this };
        if (form.ShowDialog() == true) _item.VatTus.Add(item);
    }

    private void LienKet_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_editing || !DemoSession.CanOpen("VatTuNhaCungCapView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvVatTuNhaCungCap, source) is not DataGridRow row || row.Item is not VatTuNhaCungCapMau item) return;
        item.NhaCungCap = _item.TenNhaCungCap;
        var form = new VatTuNhaCungCapChiTietWindow(item, false, true, _item.VatTus) { Owner = this };
        if (form.ShowDialog() == true) _item.VatTus.Remove(item);
        dgvVatTuNhaCungCap.Items.Refresh();
    }
}
