using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TienNghiChiTietWindow : Window
{
    private readonly TienNghiMau _item;
    private bool _editing;

    public TienNghiChiTietWindow(TienNghiMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new TienNghiMau();
        DataContext = _item;
        btnTienNghiThemMoi.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiThemMoi");
        btnTienNghiSuaThongTin.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiSuaThongTin");
        btnTienNghiXoa.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenTienNghi.IsReadOnly = !editing;
        txtMoTa.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnTienNghiThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnTienNghiSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnTienNghiXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenTienNghi.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Tiện nghi: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenTienNghi = txtTenTienNghi.Text.Trim();
        _item.MoTa = txtMoTa.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Tiện nghi: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnTienNghiThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiThemMoi")) return;
        TienNghiForm form = new TienNghiForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
