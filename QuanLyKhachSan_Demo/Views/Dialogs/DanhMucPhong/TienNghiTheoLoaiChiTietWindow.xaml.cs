using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TienNghiTheoLoaiChiTietWindow : Window
{
    private readonly TienNghiTheoLoaiMau _item;
    private bool _editing;
    private readonly bool _creating;
    private readonly bool _lockParent;
    private readonly IEnumerable<TienNghiTheoLoaiMau>? _siblings;

    private bool CanSave => DemoSession.CanUseButton("DanhMucPhongView", _creating ? "btnTienNghiTheoLoaiThemMoi" : "btnTienNghiTheoLoaiSuaThongTin");

    public TienNghiTheoLoaiChiTietWindow(TienNghiTheoLoaiMau? item = null)
        : this(item ?? new TienNghiTheoLoaiMau(), false, false, null) { }

    public TienNghiTheoLoaiChiTietWindow(TienNghiTheoLoaiMau item, bool creating, bool lockParent, IEnumerable<TienNghiTheoLoaiMau>? siblings)
    {
        InitializeComponent();
        _creating = creating;
        _lockParent = lockParent;
        _siblings = siblings;
        _item = item ?? new TienNghiTheoLoaiMau();
        DataContext = _item;
        btnTienNghiTheoLoaiSuaThongTin.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiTheoLoaiSuaThongTin");
        btnTienNghiTheoLoaiXoa.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiTheoLoaiXoa");
        btnLuuSua.IsEnabled = CanSave;
        btnHuySua.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiTheoLoaiSuaThongTin");
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
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiTheoLoaiSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiTheoLoaiSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtLoaiPhong.IsReadOnly = !editing || _lockParent;
        txtTienNghi.IsReadOnly = !editing;
        txtSoLuong.IsReadOnly = !editing;
        txtGhiChu.IsReadOnly = !editing;
        btnTienNghiTheoLoaiSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnTienNghiTheoLoaiXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !CanSave) return;
        if (string.IsNullOrWhiteSpace(txtLoaiPhong.Text) ||
            string.IsNullOrWhiteSpace(txtTienNghi.Text) ||
            string.IsNullOrWhiteSpace(txtSoLuong.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtSoLuong.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueSoLuong) || valueSoLuong < 0 || valueSoLuong == 0 || decimal.Truncate(valueSoLuong) != valueSoLuong)
        {
            txtThongBao.Text = "Số lượng: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        if (_lockParent && txtLoaiPhong.Text.Trim() != _item.LoaiPhong)
        {
            txtThongBao.Text = "Không thể chuyển dòng sang mục khác tại đây.";
            return;
        }
        if (_siblings?.Any(sibling => !ReferenceEquals(sibling, _item) &&
            (string.Equals(sibling.TienNghi, txtTienNghi.Text.Trim(), StringComparison.CurrentCultureIgnoreCase))) == true)
        {
            txtThongBao.Text = "Thông tin bị trùng với một dòng đang có.";
            return;
        }
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"{(_creating ? "Thêm" : "Lưu thay đổi")} — Tiện nghi theo loại: {_item.LoaiPhong} / {_item.TienNghi}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.LoaiPhong = txtLoaiPhong.Text.Trim();
        _item.TienNghi = txtTienNghi.Text.Trim();
        _item.SoLuong = txtSoLuong.Text.Trim();
        _item.GhiChu = txtGhiChu.Text.Trim();

        DataContext = null;
        DataContext = _item;
        if (_creating) { DialogResult = true; return; }
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiTheoLoaiXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Tiện nghi theo loại: {_item.LoaiPhong} / {_item.TienNghi}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

}
