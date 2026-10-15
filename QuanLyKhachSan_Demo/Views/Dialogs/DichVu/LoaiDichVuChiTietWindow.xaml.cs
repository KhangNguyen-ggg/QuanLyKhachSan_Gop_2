using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class LoaiDichVuChiTietWindow : Window
{
    private readonly LoaiDichVuMau _item;
    private bool _editing;

    public LoaiDichVuChiTietWindow(LoaiDichVuMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new LoaiDichVuMau();
        DataContext = _item;
        btnLoaiDichVuThemMoi.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuThemMoi");
        btnLoaiDichVuSuaThongTin.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuSuaThongTin");
        btnLoaiDichVuXoa.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenLoai.IsReadOnly = !editing;
        txtMoTa.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnLoaiDichVuThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLoaiDichVuSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLoaiDichVuXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenLoai.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Loại dịch vụ: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenLoai = txtTenLoai.Text.Trim();
        _item.MoTa = txtMoTa.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Loại dịch vụ: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnLoaiDichVuThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuThemMoi")) return;
        LoaiDichVuForm form = new LoaiDichVuForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
