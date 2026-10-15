using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class VatTuChiTietWindow : Window
{
    private readonly VatTuMau _item;
    private bool _editing;

    public VatTuChiTietWindow(VatTuMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new VatTuMau();
        DataContext = _item;
        btnVatTuThemMoi.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuThemMoi");
        btnVatTuSuaThongTin.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuSuaThongTin");
        btnVatTuXoa.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenVatTu.IsReadOnly = !editing;
        txtNhom.IsReadOnly = !editing;
        txtDonVi.IsReadOnly = !editing;
        txtKhoMacDinh.IsReadOnly = !editing;
        txtTonToiThieu.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnVatTuThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnVatTuSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnVatTuXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenVatTu.Text) ||
            string.IsNullOrWhiteSpace(txtNhom.Text) ||
            string.IsNullOrWhiteSpace(txtDonVi.Text) ||
            string.IsNullOrWhiteSpace(txtKhoMacDinh.Text) ||
            string.IsNullOrWhiteSpace(txtTonToiThieu.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtTonToiThieu.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueTonToiThieu) || valueTonToiThieu < 0)
        {
            txtThongBao.Text = "Tồn tối thiểu: nhập số không âm theo định dạng Việt Nam.";
            return;
        }


        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Vật tư: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenVatTu = txtTenVatTu.Text.Trim();
        _item.Nhom = txtNhom.Text.Trim();
        _item.DonVi = txtDonVi.Text.Trim();
        _item.KhoMacDinh = txtKhoMacDinh.Text.Trim();
        _item.TonToiThieu = txtTonToiThieu.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Vật tư: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnVatTuThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuThemMoi")) return;
                        VatTuForm form = new VatTuForm();
            form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
