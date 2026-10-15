using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class VatTuNhaCungCapChiTietWindow : Window
{
    private readonly VatTuNhaCungCapMau _item;
    private bool _editing;
    private readonly bool _creating;
    private readonly bool _lockParent;
    private readonly IEnumerable<VatTuNhaCungCapMau>? _siblings;

    private bool CanSave => DemoSession.CanUseButton("VatTuNhaCungCapView", _creating ? "btnVatTuNhaCungCapThemMoi" : "btnVatTuNhaCungCapSuaThongTin");

    public VatTuNhaCungCapChiTietWindow(VatTuNhaCungCapMau? item = null)
        : this(item ?? new VatTuNhaCungCapMau(), false, false, null) { }

    public VatTuNhaCungCapChiTietWindow(VatTuNhaCungCapMau item, bool creating, bool lockParent, IEnumerable<VatTuNhaCungCapMau>? siblings)
    {
        InitializeComponent();
        _creating = creating;
        _lockParent = lockParent;
        _siblings = siblings;
        _item = item ?? new VatTuNhaCungCapMau();
        DataContext = _item;
        btnVatTuNhaCungCapSuaThongTin.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuNhaCungCapSuaThongTin");
        btnVatTuNhaCungCapXoa.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuNhaCungCapXoa");
        btnLuuSua.IsEnabled = CanSave;
        btnHuySua.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuNhaCungCapSuaThongTin");
        if (creating && CanSave)
        {
            Title = txtTieuDe.Text = "Thêm — " + Title.Replace("Chi tiết — ", "");
            SetEditMode(true);
            btnHuySua.Visibility = Visibility.Collapsed;
            btnLuuSua.Content = "Thêm";
        }
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuNhaCungCapSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuNhaCungCapSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtVatTu.IsReadOnly = !editing;
        txtNhaCungCap.IsReadOnly = !editing || _lockParent;
        txtMaSanPhamNCC.IsReadOnly = !editing;
        txtGiaNhap.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnVatTuNhaCungCapSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnVatTuNhaCungCapXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !CanSave) return;
        if (string.IsNullOrWhiteSpace(txtVatTu.Text) ||
            string.IsNullOrWhiteSpace(txtNhaCungCap.Text) ||
            string.IsNullOrWhiteSpace(txtGiaNhap.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtGiaNhap.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueGiaNhap) || valueGiaNhap < 0)
        {
            txtThongBao.Text = "Giá nhập: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        if (_lockParent && txtNhaCungCap.Text.Trim() != _item.NhaCungCap)
        {
            txtThongBao.Text = "Không thể chuyển dòng sang mục khác tại đây.";
            return;
        }
        if (_siblings?.Any(sibling => !ReferenceEquals(sibling, _item) &&
            (string.Equals(sibling.VatTu, txtVatTu.Text.Trim(), StringComparison.CurrentCultureIgnoreCase))) == true)
        {
            txtThongBao.Text = "Thông tin bị trùng với một dòng đang có.";
            return;
        }
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"{(_creating ? "Thêm" : "Lưu thay đổi")} — Vật tư – nhà cung cấp: {_item.VatTu}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.VatTu = txtVatTu.Text.Trim();
        _item.NhaCungCap = txtNhaCungCap.Text.Trim();
        _item.MaSanPhamNCC = txtMaSanPhamNCC.Text.Trim();
        _item.GiaNhap = txtGiaNhap.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        if (_creating) { DialogResult = true; return; }
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuNhaCungCapXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Vật tư – nhà cung cấp: {_item.VatTu}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

}
