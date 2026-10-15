using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class KhuVucChiTietWindow : Window
{
    private readonly KhuVucMau _item;
    private bool _editing;
    private readonly bool _creating;
    private readonly bool _lockParent;
    private readonly IEnumerable<KhuVucMau>? _siblings;

    private bool CanSave => DemoSession.CanUseButton("DanhMucPhongView", _creating ? "btnKhuVucThemMoi" : "btnKhuVucSuaThongTin");

    public KhuVucChiTietWindow(KhuVucMau? item = null)
        : this(item ?? new KhuVucMau(), false, false, null) { }

    public KhuVucChiTietWindow(KhuVucMau item, bool creating, bool lockParent, IEnumerable<KhuVucMau>? siblings)
    {
        InitializeComponent();
        _creating = creating;
        _lockParent = lockParent;
        _siblings = siblings;
        _item = item ?? new KhuVucMau();
        DataContext = _item;
        btnKhuVucSuaThongTin.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnKhuVucSuaThongTin");
        btnKhuVucXoa.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnKhuVucXoa");
        btnLuuSua.IsEnabled = CanSave;
        btnHuySua.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnKhuVucSuaThongTin");
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
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnKhuVucSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnKhuVucSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtMa.IsReadOnly = !(_creating && editing);
        txtTenKhuVuc.IsReadOnly = !editing;
        txtMoTa.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnKhuVucSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnKhuVucXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !CanSave) return;
        if (string.IsNullOrWhiteSpace(txtTenKhuVuc.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        if (string.IsNullOrWhiteSpace(txtMa.Text))
        {
            txtThongBao.Text = "Vui lòng nhập mã.";
            return;
        }
        if (_siblings?.Any(sibling => !ReferenceEquals(sibling, _item) &&
            (string.Equals(sibling.Ma, txtMa.Text.Trim(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(sibling.TenKhuVuc, txtTenKhuVuc.Text.Trim(), StringComparison.CurrentCultureIgnoreCase))) == true)
        {
            txtThongBao.Text = "Thông tin bị trùng với một dòng đang có.";
            return;
        }
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"{(_creating ? "Thêm" : "Lưu thay đổi")} — Khu vực: {(_creating ? txtMa.Text.Trim() : _item.Ma)}?";
        if (confirm.ShowDialog() != true) return;
        if (_creating) _item.Ma = txtMa.Text.Trim();
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenKhuVuc = txtTenKhuVuc.Text.Trim();
        _item.MoTa = txtMoTa.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();
        foreach (var floor in _item.Tangs) floor.KhuVuc = _item.TenKhuVuc;

        DataContext = null;
        DataContext = _item;
        if (_creating) { DialogResult = true; return; }
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnKhuVucXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Khu vực: {_item.Ma}?";
        if (_item.Tangs.Count > 0) confirm.txtNoiDung.Text += $" Các tầng thuộc khu vực này ({_item.Tangs.Count}) cũng sẽ bị xóa.";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

}
