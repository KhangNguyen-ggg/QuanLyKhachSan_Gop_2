using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class KhoChiTietWindow : Window
{
    private readonly KhoMau _item;
    private bool _editing;

    public KhoChiTietWindow(KhoMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new KhoMau();
        DataContext = _item;
        btnKhoThemMoi.IsEnabled = DemoSession.CanUseButton("KhoView", "btnKhoThemMoi");
        btnKhoSuaThongTin.IsEnabled = DemoSession.CanUseButton("KhoView", "btnKhoSuaThongTin");
        btnKhoXoa.IsEnabled = DemoSession.CanUseButton("KhoView", "btnKhoXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("KhoView", "btnKhoSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("KhoView", "btnKhoSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnKhoSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnKhoSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenKho.IsReadOnly = !editing;
        txtGhiChu.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnKhoThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnKhoSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnKhoXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("KhoView", "btnKhoSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenKho.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }



        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Kho: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenKho = txtTenKho.Text.Trim();
        _item.GhiChu = txtGhiChu.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnKhoXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Kho: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnKhoThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnKhoThemMoi")) return;
                        KhoForm form = new KhoForm();
            form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
