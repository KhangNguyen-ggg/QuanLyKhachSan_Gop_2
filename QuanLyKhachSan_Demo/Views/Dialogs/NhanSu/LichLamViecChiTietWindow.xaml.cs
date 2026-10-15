using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class LichLamViecChiTietWindow : Window
{
    private readonly LichLamViecMau _item;
    private bool _editing;

    public LichLamViecChiTietWindow(LichLamViecMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new LichLamViecMau();
        DataContext = _item;
        btnLichLamViecThemLich.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnLichLamViecThemLich");
        btnLichLamViecSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnLichLamViecSuaThongTin");
        btnLichLamViecXoa.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnLichLamViecXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnLichLamViecSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnLichLamViecSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnLichLamViecSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnLichLamViecSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtNhanVien.IsReadOnly = !editing;
        txtNgayLam.IsReadOnly = !editing;
        txtCa.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        txtGhiChu.IsReadOnly = !editing;
        btnLichLamViecThemLich.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLichLamViecSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLichLamViecXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("NhanSuView", "btnLichLamViecSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtNhanVien.Text) ||
            string.IsNullOrWhiteSpace(txtNgayLam.Text) ||
            string.IsNullOrWhiteSpace(txtCa.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        if (!DateTime.TryParseExact(txtNgayLam.Text.Trim(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out _))
        {
            txtThongBao.Text = "Ngày làm: nhập ngày hợp lệ theo dd/MM/yyyy.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Lịch làm việc: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.NhanVien = txtNhanVien.Text.Trim();
        _item.NgayLam = txtNgayLam.Text.Trim();
        _item.Ca = txtCa.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();
        _item.GhiChu = txtGhiChu.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnLichLamViecXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Lịch làm việc: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnLichLamViecThemLich_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnLichLamViecThemLich")) return;
        LichLamViecForm form = new LichLamViecForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
