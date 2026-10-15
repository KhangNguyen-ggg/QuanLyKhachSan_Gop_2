using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ThanhPhanGoiChiTietWindow : Window
{
    private readonly ThanhPhanGoiMau _item;
    private bool _editing;
    private readonly bool _creating;
    private readonly bool _lockParent;
    private readonly IEnumerable<ThanhPhanGoiMau>? _siblings;

    private bool CanSave => DemoSession.CanUseButton("GoiUuDaiView", _creating ? "btnThanhPhanGoiThemMoi" : "btnThanhPhanGoiSuaThongTin");

    public ThanhPhanGoiChiTietWindow(ThanhPhanGoiMau? item = null)
        : this(item ?? new ThanhPhanGoiMau(), false, false, null) { }

    public ThanhPhanGoiChiTietWindow(ThanhPhanGoiMau item, bool creating, bool lockParent, IEnumerable<ThanhPhanGoiMau>? siblings)
    {
        InitializeComponent();
        _creating = creating;
        _lockParent = lockParent;
        _siblings = siblings;
        _item = item ?? new ThanhPhanGoiMau();
        DataContext = _item;
        btnThanhPhanGoiSuaThongTin.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnThanhPhanGoiSuaThongTin");
        btnThanhPhanGoiXoa.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnThanhPhanGoiXoa");
        btnLuuSua.IsEnabled = CanSave;
        btnHuySua.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnThanhPhanGoiSuaThongTin");
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
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnThanhPhanGoiSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnThanhPhanGoiSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtGoi.IsReadOnly = !editing || _lockParent;
        txtDichVu.IsReadOnly = !editing;
        txtSoLuong.IsReadOnly = !editing;
        txtDonGiaThanhPhan.IsReadOnly = !editing;
        txtGhiChu.IsReadOnly = !editing;
        btnThanhPhanGoiSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnThanhPhanGoiXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !CanSave) return;
        if (string.IsNullOrWhiteSpace(txtGoi.Text) ||
            string.IsNullOrWhiteSpace(txtDichVu.Text) ||
            string.IsNullOrWhiteSpace(txtSoLuong.Text) ||
            string.IsNullOrWhiteSpace(txtDonGiaThanhPhan.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtSoLuong.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueSoLuong) || valueSoLuong < 0 || valueSoLuong == 0 || decimal.Truncate(valueSoLuong) != valueSoLuong)
        {
            txtThongBao.Text = "Số lượng: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtDonGiaThanhPhan.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueDonGiaThanhPhan) || valueDonGiaThanhPhan < 0)
        {
            txtThongBao.Text = "Đơn giá thành phần: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        if (_lockParent && txtGoi.Text.Trim() != _item.Goi)
        {
            txtThongBao.Text = "Không thể chuyển dòng sang mục khác tại đây.";
            return;
        }
        if (_siblings?.Any(sibling => !ReferenceEquals(sibling, _item) &&
            (string.Equals(sibling.DichVu, txtDichVu.Text.Trim(), StringComparison.CurrentCultureIgnoreCase))) == true)
        {
            txtThongBao.Text = "Thông tin bị trùng với một dòng đang có.";
            return;
        }
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"{(_creating ? "Thêm" : "Lưu thay đổi")} — Thành phần gói: {_item.Goi} / {_item.DichVu}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.Goi = txtGoi.Text.Trim();
        _item.DichVu = txtDichVu.Text.Trim();
        _item.SoLuong = txtSoLuong.Text.Trim();
        _item.DonGiaThanhPhan = txtDonGiaThanhPhan.Text.Trim();
        _item.GhiChu = txtGhiChu.Text.Trim();

        DataContext = null;
        DataContext = _item;
        if (_creating) { DialogResult = true; return; }
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnThanhPhanGoiXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Thành phần gói: {_item.Goi} / {_item.DichVu}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

}
