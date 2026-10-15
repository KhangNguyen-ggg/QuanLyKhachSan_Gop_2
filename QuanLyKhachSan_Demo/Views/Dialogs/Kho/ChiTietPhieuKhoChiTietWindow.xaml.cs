using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ChiTietPhieuKhoChiTietWindow : Window
{
    private readonly ChiTietPhieuKhoMau _item;
    private bool _editing;
    private readonly bool _creating;
    private readonly bool _lockParent;
    private readonly IEnumerable<ChiTietPhieuKhoMau>? _siblings;

    private bool CanSave => DemoSession.CanUseButton("KhoView", _creating ? "btnChiTietPhieuKhoThemMoi" : "btnChiTietPhieuKhoSuaThongTin");

    public ChiTietPhieuKhoChiTietWindow(ChiTietPhieuKhoMau? item = null)
        : this(item ?? new ChiTietPhieuKhoMau(), false, false, null) { }

    public ChiTietPhieuKhoChiTietWindow(ChiTietPhieuKhoMau item, bool creating, bool lockParent, IEnumerable<ChiTietPhieuKhoMau>? siblings)
    {
        InitializeComponent();
        _creating = creating;
        _lockParent = lockParent;
        _siblings = siblings;
        _item = item ?? new ChiTietPhieuKhoMau();
        DataContext = _item;
        btnChiTietPhieuKhoSuaThongTin.IsEnabled = DemoSession.CanUseButton("KhoView", "btnChiTietPhieuKhoSuaThongTin");
        btnChiTietPhieuKhoXoa.IsEnabled = DemoSession.CanUseButton("KhoView", "btnChiTietPhieuKhoXoa");
        btnLuuSua.IsEnabled = CanSave;
        btnHuySua.IsEnabled = DemoSession.CanUseButton("KhoView", "btnChiTietPhieuKhoSuaThongTin");
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
        if (!DemoSession.CanUseButton("KhoView", "btnChiTietPhieuKhoSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnChiTietPhieuKhoSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtPhieu.IsReadOnly = !editing || _lockParent;
        txtVatTu.IsReadOnly = !editing;
        txtSoLuong.IsReadOnly = !editing;
        txtDonVi.IsReadOnly = !editing;
        txtDonGia.IsReadOnly = !editing;
        btnChiTietPhieuKhoSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnChiTietPhieuKhoXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !CanSave) return;
        if (string.IsNullOrWhiteSpace(txtPhieu.Text) ||
            string.IsNullOrWhiteSpace(txtVatTu.Text) ||
            string.IsNullOrWhiteSpace(txtSoLuong.Text) ||
            string.IsNullOrWhiteSpace(txtDonVi.Text) ||
            string.IsNullOrWhiteSpace(txtDonGia.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtSoLuong.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueSoLuong) || valueSoLuong < 0)
        {
            txtThongBao.Text = "Số lượng: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtDonGia.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueDonGia) || valueDonGia < 0)
        {
            txtThongBao.Text = "Đơn giá: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        if (valueSoLuong == 0)
        {
            txtThongBao.Text = "Số lượng phải lớn hơn 0.";
            return;
        }
        decimal total;
        try { total = valueSoLuong * valueDonGia; }
        catch (OverflowException)
        {
            txtThongBao.Text = "Thành tiền vượt giới hạn cho phép.";
            return;
        }
        if (_lockParent && txtPhieu.Text.Trim() != _item.Phieu)
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
        confirm.txtNoiDung.Text = $"{(_creating ? "Thêm" : "Lưu thay đổi")} — Chi tiết phiếu: {_item.Phieu}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.Phieu = txtPhieu.Text.Trim();
        _item.VatTu = txtVatTu.Text.Trim();
        _item.SoLuong = txtSoLuong.Text.Trim();
        _item.DonVi = txtDonVi.Text.Trim();
        _item.DonGia = txtDonGia.Text.Trim();
        _item.ThanhTien = total.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " đ";
        DataContext = null;
        DataContext = _item;
        if (_creating) { DialogResult = true; return; }
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnChiTietPhieuKhoXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Chi tiết phiếu: {_item.Phieu}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

}
