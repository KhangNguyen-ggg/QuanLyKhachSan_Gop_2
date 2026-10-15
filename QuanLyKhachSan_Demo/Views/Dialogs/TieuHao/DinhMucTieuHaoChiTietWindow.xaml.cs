using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class DinhMucTieuHaoChiTietWindow : Window
{
    private readonly DinhMucTieuHaoMau _item;
    private bool _editing;

    public DinhMucTieuHaoChiTietWindow(DinhMucTieuHaoMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new DinhMucTieuHaoMau();
        DataContext = _item;
        btnDinhMucTieuHaoThemMoi.IsEnabled = DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoThemMoi");
        btnDinhMucTieuHaoSuaThongTin.IsEnabled = DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoSuaThongTin");
        btnDinhMucTieuHaoXoa.IsEnabled = DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtDichVu.IsReadOnly = !editing;
        txtVatTu.IsReadOnly = !editing;
        txtSoLuongLan.IsReadOnly = !editing;
        txtDonVi.IsReadOnly = !editing;
        txtGhiChu.IsReadOnly = !editing;
        btnDinhMucTieuHaoThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnDinhMucTieuHaoSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnDinhMucTieuHaoXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtDichVu.Text) ||
            string.IsNullOrWhiteSpace(txtVatTu.Text) ||
            string.IsNullOrWhiteSpace(txtSoLuongLan.Text) ||
            string.IsNullOrWhiteSpace(txtDonVi.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtSoLuongLan.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueSoLuongLan) || valueSoLuongLan < 0)
        {
            txtThongBao.Text = "Số lượng / lần: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Định mức tiêu hao: {_item.DichVu}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.DichVu = txtDichVu.Text.Trim();
        _item.VatTu = txtVatTu.Text.Trim();
        _item.SoLuongLan = txtSoLuongLan.Text.Trim();
        _item.DonVi = txtDonVi.Text.Trim();
        _item.GhiChu = txtGhiChu.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Định mức tiêu hao: {_item.DichVu}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnDinhMucTieuHaoThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoThemMoi")) return;
        DinhMucTieuHaoForm form = new DinhMucTieuHaoForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
