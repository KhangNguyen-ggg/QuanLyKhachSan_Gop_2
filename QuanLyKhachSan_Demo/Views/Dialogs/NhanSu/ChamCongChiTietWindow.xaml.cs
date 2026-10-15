using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ChamCongChiTietWindow : Window
{
    private readonly ChamCongMau _item;
    private bool _editing;

    public ChamCongChiTietWindow(ChamCongMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new ChamCongMau();
        DataContext = _item;
        btnChamCongThemChamCong.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnChamCongThemChamCong");
        btnChamCongSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnChamCongSuaThongTin");
        btnChamCongXoa.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnChamCongXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnChamCongSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnChamCongSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnChamCongSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnChamCongSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtLich.IsReadOnly = !editing;
        txtNhanVien.IsReadOnly = !editing;
        txtNgay.IsReadOnly = !editing;
        txtGioVao.IsReadOnly = !editing;
        txtGioRa.IsReadOnly = !editing;
        txtGioThuong.IsReadOnly = !editing;
        txtTangCa.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnChamCongThemChamCong.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnChamCongSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnChamCongXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("NhanSuView", "btnChamCongSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtLich.Text) ||
            string.IsNullOrWhiteSpace(txtNhanVien.Text) ||
            string.IsNullOrWhiteSpace(txtNgay.Text) ||
            string.IsNullOrWhiteSpace(txtGioVao.Text) ||
            string.IsNullOrWhiteSpace(txtGioRa.Text) ||
            string.IsNullOrWhiteSpace(txtGioThuong.Text) ||
            string.IsNullOrWhiteSpace(txtTangCa.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtGioThuong.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueGioThuong) || valueGioThuong < 0)
        {
            txtThongBao.Text = "Giờ thường: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtTangCa.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueTangCa) || valueTangCa < 0)
        {
            txtThongBao.Text = "Tăng ca: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!DateTime.TryParseExact(txtNgay.Text.Trim(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out _))
        {
            txtThongBao.Text = "Ngày: nhập ngày hợp lệ theo dd/MM/yyyy.";
            return;
        }
        if (!TimeSpan.TryParseExact(txtGioVao.Text.Trim().Replace(" hôm sau", ""), @"hh\:mm", CultureInfo.InvariantCulture, out var valueGioVao) || valueGioVao.TotalHours >= 24)
        {
            txtThongBao.Text = "Giờ vào: nhập giờ hợp lệ theo HH:mm.";
            return;
        }
        if (!TimeSpan.TryParseExact(txtGioRa.Text.Trim().Replace(" hôm sau", ""), @"hh\:mm", CultureInfo.InvariantCulture, out var valueGioRa) || valueGioRa.TotalHours >= 24)
        {
            txtThongBao.Text = "Giờ ra: nhập giờ hợp lệ theo HH:mm.";
            return;
        }
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Chấm công: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.Lich = txtLich.Text.Trim();
        _item.NhanVien = txtNhanVien.Text.Trim();
        _item.Ngay = txtNgay.Text.Trim();
        _item.GioVao = txtGioVao.Text.Trim();
        _item.GioRa = txtGioRa.Text.Trim();
        _item.GioThuong = txtGioThuong.Text.Trim();
        _item.TangCa = txtTangCa.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnChamCongXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Chấm công: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnChamCongThemChamCong_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnChamCongThemChamCong")) return;
        ChamCongForm form = new ChamCongForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
