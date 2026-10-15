using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class DichVuChiTietWindow : Window
{
    private readonly DichVuMau _item;
    private bool _editing;

    public DichVuChiTietWindow(DichVuMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new DichVuMau();
        DataContext = _item;
        btnDichVuThemMoi.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnDichVuThemMoi");
        btnDichVuSuaThongTin.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnDichVuSuaThongTin");
        btnDichVuXoa.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnDichVuXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnDichVuSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnDichVuSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnDichVuSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnDichVuSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenDichVu.IsReadOnly = !editing;
        txtLoaiDichVu.IsReadOnly = !editing;
        txtDonVi.IsReadOnly = !editing;
        txtGia.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnDichVuThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnDichVuSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnDichVuXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("DichVuView", "btnDichVuSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenDichVu.Text) ||
            string.IsNullOrWhiteSpace(txtLoaiDichVu.Text) ||
            string.IsNullOrWhiteSpace(txtDonVi.Text) ||
            string.IsNullOrWhiteSpace(txtGia.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtGia.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueGia) || valueGia < 0)
        {
            txtThongBao.Text = "Giá: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Dịch vụ: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenDichVu = txtTenDichVu.Text.Trim();
        _item.LoaiDichVu = txtLoaiDichVu.Text.Trim();
        _item.DonVi = txtDonVi.Text.Trim();
        _item.Gia = txtGia.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnDichVuXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Dịch vụ: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnDichVuThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnDichVuThemMoi")) return;
        DichVuForm form = new DichVuForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
