using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class PhongBanChiTietWindow : Window
{
    private readonly PhongBanMau _item;
    private bool _editing;

    public PhongBanChiTietWindow(PhongBanMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new PhongBanMau();
        DataContext = _item;
        btnPhongBanThemMoi.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnPhongBanThemMoi");
        btnPhongBanSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin");
        btnPhongBanXoa.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnPhongBanXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenPhongBan.IsReadOnly = !editing;
        txtMoTa.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnPhongBanThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnPhongBanSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnPhongBanXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("NhanVienView", "btnPhongBanSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenPhongBan.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Phòng ban: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenPhongBan = txtTenPhongBan.Text.Trim();
        _item.MoTa = txtMoTa.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnPhongBanXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Phòng ban: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnPhongBanThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnPhongBanThemMoi")) return;
        PhongBanForm form = new PhongBanForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
