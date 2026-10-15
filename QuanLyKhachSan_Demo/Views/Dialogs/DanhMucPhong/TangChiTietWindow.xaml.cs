using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TangChiTietWindow : Window
{
    private readonly TangMau _item;
    private bool _editing;
    private readonly bool _creating;
    private readonly bool _lockParent;
    private readonly IEnumerable<TangMau>? _siblings;

    private bool CanSave => DemoSession.CanUseButton("DanhMucPhongView", _creating ? "btnTangThemMoi" : "btnTangSuaThongTin");

    public TangChiTietWindow(TangMau? item = null)
        : this(item ?? new TangMau(), false, false, null) { }

    public TangChiTietWindow(TangMau item, bool creating, bool lockParent, IEnumerable<TangMau>? siblings)
    {
        InitializeComponent();
        _creating = creating;
        _lockParent = lockParent;
        _siblings = siblings;
        _item = item ?? new TangMau();
        DataContext = _item;
        btnTangSuaThongTin.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTangSuaThongTin");
        btnTangXoa.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTangXoa");
        btnLuuSua.IsEnabled = CanSave;
        btnHuySua.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTangSuaThongTin");
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
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTangSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTangSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtMa.IsReadOnly = !(_creating && editing);
        txtTenTang.IsReadOnly = !editing;
        txtSoTang.IsReadOnly = !editing;
        txtKhuVuc.IsReadOnly = !editing || _lockParent;
        txtTrangThai.IsReadOnly = !editing;
        btnTangSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnTangXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !CanSave) return;
        if (string.IsNullOrWhiteSpace(txtTenTang.Text) ||
            string.IsNullOrWhiteSpace(txtSoTang.Text) ||
            string.IsNullOrWhiteSpace(txtKhuVuc.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtSoTang.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueSoTang) || valueSoTang < 0 || decimal.Truncate(valueSoTang) != valueSoTang)
        {
            txtThongBao.Text = "Số tầng: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        if (_lockParent && txtKhuVuc.Text.Trim() != _item.KhuVuc)
        {
            txtThongBao.Text = "Không thể chuyển dòng sang mục khác tại đây.";
            return;
        }
        if (string.IsNullOrWhiteSpace(txtMa.Text))
        {
            txtThongBao.Text = "Vui lòng nhập mã.";
            return;
        }
        if (_siblings?.Any(sibling => !ReferenceEquals(sibling, _item) &&
            (string.Equals(sibling.Ma, txtMa.Text.Trim(), StringComparison.CurrentCultureIgnoreCase) ||
             (string.Equals(sibling.KhuVuc, _item.KhuVuc, StringComparison.CurrentCultureIgnoreCase) &&
              string.Equals(sibling.SoTang, txtSoTang.Text.Trim(), StringComparison.CurrentCultureIgnoreCase)))) == true)
        {
            txtThongBao.Text = "Thông tin bị trùng với một dòng đang có.";
            return;
        }
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"{(_creating ? "Thêm" : "Lưu thay đổi")} — Tầng: {(_creating ? txtMa.Text.Trim() : _item.Ma)}?";
        if (confirm.ShowDialog() != true) return;
        if (_creating) _item.Ma = txtMa.Text.Trim();
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenTang = txtTenTang.Text.Trim();
        _item.SoTang = txtSoTang.Text.Trim();
        _item.KhuVuc = txtKhuVuc.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        if (_creating) { DialogResult = true; return; }
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTangXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Tầng: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

}
