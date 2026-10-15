using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ChucVuChiTietWindow : Window
{
    private readonly ChucVuMau _item;
    private bool _editing;

    public ChucVuChiTietWindow(ChucVuMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new ChucVuMau();
        DataContext = _item;
        btnChucVuSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin");
        btnChucVuXoa.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnChucVuXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenChucVu.IsReadOnly = !editing;
        txtMoTa.IsReadOnly = !editing;
        txtLuong.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnChucVuSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnChucVuXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenChucVu.Text) ||
            string.IsNullOrWhiteSpace(txtLuong.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtLuong.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueLuong) || valueLuong < 0)
        {
            txtThongBao.Text = "Mức lương: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Chức vụ: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenChucVu = txtTenChucVu.Text.Trim();
        _item.MoTa = txtMoTa.Text.Trim();
        _item.Luong = txtLuong.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnChucVuXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Chức vụ: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

}
