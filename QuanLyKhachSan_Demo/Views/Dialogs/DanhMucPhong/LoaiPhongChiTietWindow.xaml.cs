using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class LoaiPhongChiTietWindow : Window
{
    private readonly LoaiPhongMau _item;
    private bool _editing;

    public LoaiPhongChiTietWindow(LoaiPhongMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new LoaiPhongMau();
        DataContext = _item;
        dgvTienNghiTheoLoai.ItemsSource = _item.TienNghis;
        btnTienNghiTheoLoaiThemMoi.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiTheoLoaiThemMoi");
        btnLoaiPhongThemMoi.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongThemMoi");
        btnLoaiPhongSuaThongTin.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongSuaThongTin");
        btnLoaiPhongXoa.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        pnlLienKet.IsEnabled = !editing;
        if (editing) tabChiTiet.SelectedItem = tabThongTin;
        txtTenLoai.IsReadOnly = !editing;
        txtGiaDem.IsReadOnly = !editing;
        txtNguoiLon.IsReadOnly = !editing;
        txtTreEm.IsReadOnly = !editing;
        txtDienTich.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnLoaiPhongThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLoaiPhongSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLoaiPhongXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenLoai.Text) ||
            string.IsNullOrWhiteSpace(txtGiaDem.Text) ||
            string.IsNullOrWhiteSpace(txtNguoiLon.Text) ||
            string.IsNullOrWhiteSpace(txtTreEm.Text) ||
            string.IsNullOrWhiteSpace(txtDienTich.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtGiaDem.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueGiaDem) || valueGiaDem < 0)
        {
            txtThongBao.Text = "Giá / đêm: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtNguoiLon.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueNguoiLon) || valueNguoiLon < 0 || valueNguoiLon == 0 || decimal.Truncate(valueNguoiLon) != valueNguoiLon)
        {
            txtThongBao.Text = "Người lớn: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtTreEm.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueTreEm) || valueTreEm < 0 || decimal.Truncate(valueTreEm) != valueTreEm)
        {
            txtThongBao.Text = "Trẻ em: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtDienTich.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueDienTich) || valueDienTich < 0 || valueDienTich == 0)
        {
            txtThongBao.Text = "Diện tích: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Loại phòng: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenLoai = txtTenLoai.Text.Trim();
        _item.GiaDem = txtGiaDem.Text.Trim();
        _item.NguoiLon = txtNguoiLon.Text.Trim();
        _item.TreEm = txtTreEm.Text.Trim();
        _item.DienTich = txtDienTich.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        foreach (var child in _item.TienNghis) child.LoaiPhong = _item.TenLoai;
        dgvTienNghiTheoLoai.Items.Refresh();
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Loại phòng: {_item.Ma}?";
        if (_item.TienNghis.Count > 0) confirm.txtNoiDung.Text += $" Các dòng tiện nghi thuộc mục này ({_item.TienNghis.Count}) cũng sẽ bị xóa.";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnLoaiPhongThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongThemMoi")) return;
        LoaiPhongForm form = new LoaiPhongForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnThemLienKet_Click(object sender, RoutedEventArgs e)
    {
        if (_editing || !DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiTheoLoaiThemMoi")) return;
        var item = new TienNghiTheoLoaiMau { LoaiPhong = _item.TenLoai, SoLuong = "1" };
        var form = new TienNghiTheoLoaiChiTietWindow(item, true, true, _item.TienNghis) { Owner = this };
        if (form.ShowDialog() == true) _item.TienNghis.Add(item);
    }

    private void LienKet_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_editing || !DemoSession.CanOpen("DanhMucPhongView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvTienNghiTheoLoai, source) is not DataGridRow row || row.Item is not TienNghiTheoLoaiMau item) return;
        item.LoaiPhong = _item.TenLoai;
        var form = new TienNghiTheoLoaiChiTietWindow(item, false, true, _item.TienNghis) { Owner = this };
        if (form.ShowDialog() == true) _item.TienNghis.Remove(item);
        dgvTienNghiTheoLoai.Items.Refresh();
    }
}
