using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class KhenThuongChiTietWindow : Window
{
    private readonly KhenThuongMau _item;
    private bool _editing;

    public KhenThuongChiTietWindow(KhenThuongMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new KhenThuongMau();
        DataContext = _item;
        btnKhenThuongThemKhenThuong.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnKhenThuongThemKhenThuong");
        btnKhenThuongSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnKhenThuongSuaThongTin");
        btnKhenThuongXoa.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnKhenThuongXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnKhenThuongSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnKhenThuongSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnKhenThuongSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnKhenThuongSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtNhanVien.IsReadOnly = !editing;
        txtNgay.IsReadOnly = !editing;
        txtLyDo.IsReadOnly = !editing;
        txtHinhThuc.IsReadOnly = !editing;
        txtSoTien.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnKhenThuongThemKhenThuong.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnKhenThuongSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnKhenThuongXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("NhanSuView", "btnKhenThuongSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtNhanVien.Text) ||
            string.IsNullOrWhiteSpace(txtNgay.Text) ||
            string.IsNullOrWhiteSpace(txtLyDo.Text) ||
            string.IsNullOrWhiteSpace(txtHinhThuc.Text) ||
            string.IsNullOrWhiteSpace(txtSoTien.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtSoTien.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueSoTien) || valueSoTien < 0)
        {
            txtThongBao.Text = "Số tiền: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!DateTime.TryParseExact(txtNgay.Text.Trim(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out _))
        {
            txtThongBao.Text = "Ngày: nhập ngày hợp lệ theo dd/MM/yyyy.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Khen thưởng: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.NhanVien = txtNhanVien.Text.Trim();
        _item.Ngay = txtNgay.Text.Trim();
        _item.LyDo = txtLyDo.Text.Trim();
        _item.HinhThuc = txtHinhThuc.Text.Trim();
        _item.SoTien = txtSoTien.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnKhenThuongXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Khen thưởng: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnKhenThuongThemKhenThuong_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnKhenThuongThemKhenThuong")) return;
        KhenThuongForm form = new KhenThuongForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
