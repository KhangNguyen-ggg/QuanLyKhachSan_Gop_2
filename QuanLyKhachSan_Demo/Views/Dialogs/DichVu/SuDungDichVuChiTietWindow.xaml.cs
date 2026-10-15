using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class SuDungDichVuChiTietWindow : Window
{
    private readonly SuDungDichVuMau _item;
    private bool _editing;

    public SuDungDichVuChiTietWindow(SuDungDichVuMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new SuDungDichVuMau();
        DataContext = _item;
        btnSuDungDichVuThemLuotSuDung.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuThemLuotSuDung");
        btnSuDungDichVuSuaLuotSuDung.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuSuaLuotSuDung");
        btnSuDungDichVuHuyLuotSuDung.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuHuyLuotSuDung");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuSuaLuotSuDung");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuSuaLuotSuDung");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuSuaLuotSuDung")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuSuaLuotSuDung")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtDatPhong.IsReadOnly = !editing;
        txtKhachHang.IsReadOnly = !editing;
        txtDichVuGoi.IsReadOnly = !editing;
        txtSoLuong.IsReadOnly = !editing;
        txtDonGia.IsReadOnly = !editing;
        txtThoiDiem.IsReadOnly = !editing;
        btnSuDungDichVuThemLuotSuDung.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnSuDungDichVuSuaLuotSuDung.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnSuDungDichVuHuyLuotSuDung.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuSuaLuotSuDung")) return;
        if (string.IsNullOrWhiteSpace(txtDatPhong.Text) ||
            string.IsNullOrWhiteSpace(txtKhachHang.Text) ||
            string.IsNullOrWhiteSpace(txtDichVuGoi.Text) ||
            string.IsNullOrWhiteSpace(txtSoLuong.Text) ||
            string.IsNullOrWhiteSpace(txtDonGia.Text) ||
            string.IsNullOrWhiteSpace(txtThoiDiem.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }
        if (!decimal.TryParse(txtSoLuong.Text.Trim(), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueSoLuong) || valueSoLuong <= 0)
        {
            txtThongBao.Text = "Số lượng: nhập số lớn hơn 0 theo định dạng Việt Nam.";
            return;
        }
        if (!decimal.TryParse(txtDonGia.Text.Trim().Replace(" đ", "").Replace(" m²", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueDonGia) || valueDonGia < 0)
        {
            txtThongBao.Text = "Đơn giá: nhập số không âm theo định dạng Việt Nam.";
            return;
        }

        if (!DateTime.TryParseExact(txtThoiDiem.Text.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out _))
        {
            txtThongBao.Text = "Thời điểm: nhập theo dd/MM/yyyy HH:mm.";
            return;
        }
        decimal total;
        try { total = checked(valueSoLuong * valueDonGia); }
        catch (OverflowException)
        {
            txtThongBao.Text = "Thành tiền vượt giới hạn cho phép.";
            return;
        }
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Khách sử dụng dịch vụ: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.DatPhong = txtDatPhong.Text.Trim();
        _item.KhachHang = txtKhachHang.Text.Trim();
        _item.DichVuGoi = txtDichVuGoi.Text.Trim();
        _item.SoLuong = txtSoLuong.Text.Trim();
        _item.DonGia = txtDonGia.Text.Trim();
        _item.ThoiDiem = txtThoiDiem.Text.Trim();
        _item.ThanhTien = total.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " đ";

        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuHuyLuotSuDung") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Hủy lượt sử dụng — Khách sử dụng dịch vụ: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnSuDungDichVuThemLuotSuDung_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuThemLuotSuDung")) return;
        SuDungDichVuForm form = new SuDungDichVuForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
