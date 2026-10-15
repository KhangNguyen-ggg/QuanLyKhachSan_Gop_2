using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class BangLuongChiTietWindow : Window
{
    private readonly BangLuongMau _item;
    private bool _editing;

    public BangLuongChiTietWindow(BangLuongMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new BangLuongMau();
        DataContext = _item;
        btnBangLuongLapBangLuong.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnBangLuongLapBangLuong");
        btnBangLuongSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnBangLuongSuaThongTin");
        btnBangLuongXoa.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnBangLuongXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnBangLuongSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("NhanSuView", "btnBangLuongSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnBangLuongSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnBangLuongSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtNhanVien.IsReadOnly = !editing;
        txtKy.IsReadOnly = !editing;
        txtTienCong.IsReadOnly = !editing;
        txtTangCa.IsReadOnly = !editing;
        txtThuong.IsReadOnly = !editing;
        txtPhuCap.IsReadOnly = !editing;
        txtKhauTru.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnBangLuongLapBangLuong.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnBangLuongSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnBangLuongXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("NhanSuView", "btnBangLuongSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtNhanVien.Text) ||
            string.IsNullOrWhiteSpace(txtKy.Text) ||
            string.IsNullOrWhiteSpace(txtTienCong.Text) ||
            string.IsNullOrWhiteSpace(txtTangCa.Text) ||
            string.IsNullOrWhiteSpace(txtThuong.Text) ||
            string.IsNullOrWhiteSpace(txtPhuCap.Text) ||
            string.IsNullOrWhiteSpace(txtKhauTru.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtTienCong.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueTienCong) || valueTienCong < 0)
        {
            txtThongBao.Text = "Tiền công: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtTangCa.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueTangCa) || valueTangCa < 0)
        {
            txtThongBao.Text = "Tăng ca: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtThuong.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueThuong) || valueThuong < 0)
        {
            txtThongBao.Text = "Thưởng: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtPhuCap.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valuePhuCap) || valuePhuCap < 0)
        {
            txtThongBao.Text = "Phụ cấp: nhập số không âm theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtKhauTru.Text.Trim().Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueKhauTru) || valueKhauTru < 0)
        {
            txtThongBao.Text = "Khấu trừ: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        decimal total;
        try { total = valueTienCong + valueTangCa + valueThuong + valuePhuCap - valueKhauTru; }
        catch (OverflowException)
        {
            txtThongBao.Text = "Các khoản tiền vượt giới hạn cho phép.";
            return;
        }
        if (total < 0)
        {
            txtThongBao.Text = "Khấu trừ không được lớn hơn tổng thu nhập.";
            return;
        }
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Bảng lương: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.NhanVien = txtNhanVien.Text.Trim();
        _item.Ky = txtKy.Text.Trim();
        _item.TienCong = txtTienCong.Text.Trim();
        _item.TangCa = txtTangCa.Text.Trim();
        _item.Thuong = txtThuong.Text.Trim();
        _item.PhuCap = txtPhuCap.Text.Trim();
        _item.KhauTru = txtKhauTru.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();
        _item.ThucLinh = total.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " đ";
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnBangLuongXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Bảng lương: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnBangLuongLapBangLuong_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", "btnBangLuongLapBangLuong")) return;
        BangLuongForm form = new BangLuongForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
