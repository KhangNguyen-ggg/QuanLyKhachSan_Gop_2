using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class PhieuNhapXuatChiTietWindow : Window
{
    private readonly PhieuNhapXuatMau _item;
    private bool _editing;

    public PhieuNhapXuatChiTietWindow(PhieuNhapXuatMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new PhieuNhapXuatMau();
        DataContext = _item;
        dgvChiTietPhieuKho.ItemsSource = _item.ChiTiets;
        btnChiTietPhieuKhoThemMoi.IsEnabled = DemoSession.CanUseButton("KhoView", "btnChiTietPhieuKhoThemMoi");
        btnPhieuNhapXuatTaoPhieu.IsEnabled = DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatTaoPhieu");
        btnPhieuNhapXuatSuaPhieu.IsEnabled = DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatSuaPhieu");
        btnPhieuNhapXuatXacNhanPhieu.IsEnabled = DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatXacNhanPhieu");
        btnPhieuNhapXuatHuyPhieu.IsEnabled = DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatHuyPhieu");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatSuaPhieu");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatSuaPhieu");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatSuaPhieu")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatSuaPhieu")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        pnlLienKet.IsEnabled = !editing;
        if (editing) tabChiTiet.SelectedItem = tabThongTin;
        txtLoai.IsReadOnly = !editing;
        txtKho.IsReadOnly = !editing;
        txtNhaCungCap.IsReadOnly = !editing;
        txtNgay.IsReadOnly = !editing;
        txtNhanVien.IsReadOnly = !editing;
        txtGhiChu.IsReadOnly = !editing;
        btnPhieuNhapXuatTaoPhieu.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnPhieuNhapXuatSuaPhieu.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnPhieuNhapXuatXacNhanPhieu.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnPhieuNhapXuatHuyPhieu.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatSuaPhieu")) return;
        if (string.IsNullOrWhiteSpace(txtLoai.Text) ||
            string.IsNullOrWhiteSpace(txtKho.Text) ||
            string.IsNullOrWhiteSpace(txtNhaCungCap.Text) ||
            string.IsNullOrWhiteSpace(txtNgay.Text) ||
            string.IsNullOrWhiteSpace(txtNhanVien.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        if (!DateTime.TryParseExact(txtNgay.Text.Trim(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out _))
        {
            txtThongBao.Text = "Ngày: nhập ngày hợp lệ theo dd/MM/yyyy.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Phiếu nhập / xuất: {_item.MaPhieu}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.Loai = txtLoai.Text.Trim();
        _item.Kho = txtKho.Text.Trim();
        _item.NhaCungCap = txtNhaCungCap.Text.Trim();
        _item.Ngay = txtNgay.Text.Trim();
        _item.NhanVien = txtNhanVien.Text.Trim();
        _item.GhiChu = txtGhiChu.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnPhieuNhapXuatTaoPhieu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatTaoPhieu")) return;
        PhieuNhapXuatForm form = new PhieuNhapXuatForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnPhieuNhapXuatXacNhanPhieu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatXacNhanPhieu")) return;
        var form = new XacNhanDemoWindow { Owner = this };
        form.txtNoiDung.Text = $"Xác nhận phiếu — Phiếu nhập / xuất: {_item.MaPhieu}?";
        if (form.ShowDialog() != true) return;
        // shortcut: xác nhận demo chưa ghi nghiệp vụ, nối dịch vụ xử lý trước khi thay đổi dữ liệu thật.
        txtThongBao.Text = "Đã xác nhận trong bản demo; thao tác chưa được ghi vào hệ thống.";
    }

    private void btnPhieuNhapXuatHuyPhieu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatHuyPhieu")) return;
        var form = new XacNhanDemoWindow { Owner = this };
        form.txtNoiDung.Text = $"Hủy phiếu — Phiếu nhập / xuất: {_item.MaPhieu}?";
        if (form.ShowDialog() != true) return;
        // shortcut: xác nhận demo chưa ghi nghiệp vụ, nối dịch vụ xử lý trước khi thay đổi dữ liệu thật.
        txtThongBao.Text = "Đã xác nhận trong bản demo; thao tác chưa được ghi vào hệ thống.";
    }

    private void btnThemLienKet_Click(object sender, RoutedEventArgs e)
    {
        if (_editing || !DemoSession.CanUseButton("KhoView", "btnChiTietPhieuKhoThemMoi")) return;
        var item = new ChiTietPhieuKhoMau { Phieu = _item.MaPhieu, SoLuong = "1", DonGia = "0", DonVi = "Cái" };
        var form = new ChiTietPhieuKhoChiTietWindow(item, true, true, _item.ChiTiets) { Owner = this };
        if (form.ShowDialog() == true) _item.ChiTiets.Add(item);
    }

    private void LienKet_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_editing || !DemoSession.CanOpen("KhoView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvChiTietPhieuKho, source) is not DataGridRow row || row.Item is not ChiTietPhieuKhoMau item) return;
        item.Phieu = _item.MaPhieu;
        var form = new ChiTietPhieuKhoChiTietWindow(item, false, true, _item.ChiTiets) { Owner = this };
        if (form.ShowDialog() == true) _item.ChiTiets.Remove(item);
        dgvChiTietPhieuKho.Items.Refresh();
    }
}
