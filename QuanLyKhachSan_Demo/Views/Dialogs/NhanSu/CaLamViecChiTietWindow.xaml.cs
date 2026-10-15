using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class CaLamViecChiTietWindow : Window
{
    private readonly CaLamViecMau _item;
    private bool _editing;

    public CaLamViecChiTietWindow(CaLamViecMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new CaLamViecMau();
        DataContext = _item;
        btnCaLamViecThemMoi.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnCaLamViecThemMoi");
        btnCaLamViecSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnCaLamViecSuaThongTin");
        btnCaLamViecXoa.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnCaLamViecXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnCaLamViecSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnCaLamViecSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnCaLamViecSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnCaLamViecSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenCa.IsReadOnly = !editing;
        txtGioBatDau.IsReadOnly = !editing;
        txtGioKetThuc.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnCaLamViecThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnCaLamViecSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnCaLamViecXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("NhanSuView", "btnCaLamViecSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenCa.Text) ||
            string.IsNullOrWhiteSpace(txtGioBatDau.Text) ||
            string.IsNullOrWhiteSpace(txtGioKetThuc.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        if (!TimeSpan.TryParseExact(txtGioBatDau.Text.Trim().Replace(" hôm sau", ""), @"hh\:mm", CultureInfo.InvariantCulture, out var valueGioBatDau) || valueGioBatDau.TotalHours >= 24)
        {
            txtThongBao.Text = "Giờ bắt đầu: nhập giờ hợp lệ theo HH:mm.";
            return;
        }
        if (!TimeSpan.TryParseExact(txtGioKetThuc.Text.Trim().Replace(" hôm sau", ""), @"hh\:mm", CultureInfo.InvariantCulture, out var valueGioKetThuc) || valueGioKetThuc.TotalHours >= 24)
        {
            txtThongBao.Text = "Giờ kết thúc: nhập giờ hợp lệ theo HH:mm.";
            return;
        }
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Ca làm việc: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenCa = txtTenCa.Text.Trim();
        _item.GioBatDau = txtGioBatDau.Text.Trim();
        _item.GioKetThuc = txtGioKetThuc.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();
        var duration = valueGioKetThuc - valueGioBatDau;
        if (duration < TimeSpan.Zero) duration += TimeSpan.FromDays(1);
        _item.SoGio = duration.TotalHours.ToString("0.##", CultureInfo.GetCultureInfo("vi-VN"));
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnCaLamViecXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Ca làm việc: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnCaLamViecThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnCaLamViecThemMoi")) return;
        CaLamViecForm form = new CaLamViecForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
