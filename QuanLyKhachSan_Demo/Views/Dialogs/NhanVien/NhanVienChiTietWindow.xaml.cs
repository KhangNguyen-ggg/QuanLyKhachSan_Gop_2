using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhanVienChiTietWindow : Window
{
    private readonly NhanVienMau _item;
    private bool _editing;

    public NhanVienChiTietWindow(NhanVienMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new NhanVienMau();
        DataContext = _item;
        btnNhanVienThemMoi.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnNhanVienThemMoi");
        btnNhanVienSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnNhanVienSuaThongTin");
        btnNhanVienXoa.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnNhanVienXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnNhanVienSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnNhanVienSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnNhanVienSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnNhanVienSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtHoTen.IsReadOnly = !editing;
        txtPhongBan.IsReadOnly = !editing;
        txtChucVu.IsReadOnly = !editing;
        txtDienThoai.IsReadOnly = !editing;
        txtLuongCoBan.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnNhanVienThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnNhanVienSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnNhanVienXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("NhanVienView", "btnNhanVienSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
            string.IsNullOrWhiteSpace(txtPhongBan.Text) ||
            string.IsNullOrWhiteSpace(txtChucVu.Text) ||
            string.IsNullOrWhiteSpace(txtLuongCoBan.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtLuongCoBan.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueLuongCoBan) || valueLuongCoBan < 0)
        {
            txtThongBao.Text = "Lương cơ bản: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Nhân viên: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.HoTen = txtHoTen.Text.Trim();
        _item.PhongBan = txtPhongBan.Text.Trim();
        _item.ChucVu = txtChucVu.Text.Trim();
        _item.DienThoai = txtDienThoai.Text.Trim();
        _item.LuongCoBan = txtLuongCoBan.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnNhanVienXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Nhân viên: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnNhanVienThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnNhanVienThemMoi")) return;
        NhanVienForm form = new NhanVienForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
