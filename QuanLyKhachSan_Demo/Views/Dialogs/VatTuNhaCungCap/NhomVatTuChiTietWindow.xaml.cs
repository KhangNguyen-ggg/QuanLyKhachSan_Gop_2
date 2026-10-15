using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhomVatTuChiTietWindow : Window
{
    private readonly NhomVatTuMau _item;
    private bool _editing;

    public NhomVatTuChiTietWindow(NhomVatTuMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new NhomVatTuMau();
        DataContext = _item;
        btnNhomVatTuThemMoi.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuThemMoi");
        btnNhomVatTuSuaThongTin.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuSuaThongTin");
        btnNhomVatTuXoa.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenNhom.IsReadOnly = !editing;
        txtMoTa.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnNhomVatTuThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnNhomVatTuSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnNhomVatTuXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenNhom.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Nhóm vật tư: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenNhom = txtTenNhom.Text.Trim();
        _item.MoTa = txtMoTa.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Nhóm vật tư: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnNhomVatTuThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuThemMoi")) return;
        NhomVatTuForm form = new NhomVatTuForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
