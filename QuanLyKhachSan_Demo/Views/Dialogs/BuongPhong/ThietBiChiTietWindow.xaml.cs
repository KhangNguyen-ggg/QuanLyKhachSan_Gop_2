using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ThietBiChiTietWindow : Window
{
    private readonly ThietBiMau _item;
    private bool _editing;

    public ThietBiChiTietWindow(ThietBiMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new ThietBiMau();
        DataContext = _item;
        btnThietBiThemMoi.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnThietBiThemMoi");
        btnThietBiSuaThongTin.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnThietBiSuaThongTin");
        btnThietBiXoa.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnThietBiXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnThietBiSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnThietBiSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnThietBiSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnThietBiSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenThietBi.IsReadOnly = !editing;
        txtPhong.IsReadOnly = !editing;
        txtNhaCungCap.IsReadOnly = !editing;
        txtNgayMua.IsReadOnly = !editing;
        txtHanBaoHanh.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnThietBiThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnThietBiSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnThietBiXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("BuongPhongView", "btnThietBiSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenThietBi.Text) ||
            string.IsNullOrWhiteSpace(txtPhong.Text) ||
            string.IsNullOrWhiteSpace(txtNhaCungCap.Text) ||
            string.IsNullOrWhiteSpace(txtNgayMua.Text) ||
            string.IsNullOrWhiteSpace(txtHanBaoHanh.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        if (!DateTime.TryParseExact(txtNgayMua.Text.Trim(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out var dateNgayMua))
        {
            txtThongBao.Text = "Ngày mua: nhập ngày hợp lệ theo dd/MM/yyyy.";
            return;
        }
        if (!DateTime.TryParseExact(txtHanBaoHanh.Text.Trim(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out var dateHanBaoHanh))
        {
            txtThongBao.Text = "Hạn bảo hành: nhập ngày hợp lệ theo dd/MM/yyyy.";
            return;
        }

        if (dateHanBaoHanh < dateNgayMua)
        {
            txtThongBao.Text = "Hạn bảo hành phải từ ngày mua trở đi.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Thiết bị: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenThietBi = txtTenThietBi.Text.Trim();
        _item.Phong = txtPhong.Text.Trim();
        _item.NhaCungCap = txtNhaCungCap.Text.Trim();
        _item.NgayMua = txtNgayMua.Text.Trim();
        _item.HanBaoHanh = txtHanBaoHanh.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnThietBiXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Thiết bị: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnThietBiThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnThietBiThemMoi")) return;
        ThietBiForm form = new ThietBiForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
