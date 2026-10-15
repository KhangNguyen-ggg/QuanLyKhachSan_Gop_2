using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class UuDaiChiTietWindow : Window
{
    private readonly UuDaiMau _item;
    private bool _editing;

    public UuDaiChiTietWindow(UuDaiMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new UuDaiMau();
        DataContext = _item;
        btnUuDaiThemMoi.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiThemMoi");
        btnUuDaiSuaThongTin.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiSuaThongTin");
        btnUuDaiXoa.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiSuaThongTin");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiSuaThongTin")) return;
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenUuDai.IsReadOnly = !editing;
        txtMaApDung.IsReadOnly = !editing;
        txtLoaiGiam.IsReadOnly = !editing;
        txtGiaTri.IsReadOnly = !editing;
        txtTuNgay.IsReadOnly = !editing;
        txtDenNgay.IsReadOnly = !editing;
        txtTrangThai.IsReadOnly = !editing;
        btnUuDaiThemMoi.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnUuDaiSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnUuDaiXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiSuaThongTin")) return;
        if (string.IsNullOrWhiteSpace(txtTenUuDai.Text) ||
            string.IsNullOrWhiteSpace(txtMaApDung.Text) ||
            string.IsNullOrWhiteSpace(txtLoaiGiam.Text) ||
            string.IsNullOrWhiteSpace(txtGiaTri.Text) ||
            string.IsNullOrWhiteSpace(txtTuNgay.Text) ||
            string.IsNullOrWhiteSpace(txtDenNgay.Text) ||
            string.IsNullOrWhiteSpace(txtTrangThai.Text))
        {
            txtThongBao.Text = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            return;
        }

        if (!DateTime.TryParseExact(txtTuNgay.Text.Trim(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out var dateTuNgay))
        {
            txtThongBao.Text = "Từ ngày: nhập ngày hợp lệ theo dd/MM/yyyy.";
            return;
        }
        if (!DateTime.TryParseExact(txtDenNgay.Text.Trim(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out var dateDenNgay))
        {
            txtThongBao.Text = "Đến ngày: nhập ngày hợp lệ theo dd/MM/yyyy.";
            return;
        }

        if (dateDenNgay < dateTuNgay)
        {
            txtThongBao.Text = "Ngày kết thúc phải từ ngày bắt đầu trở đi.";
            return;
        }
        var percent = txtLoaiGiam.Text.Trim() == "Phần trăm";
        if ((!percent && txtLoaiGiam.Text.Trim() != "Số tiền") ||
            (!percent && txtGiaTri.Text.Contains('%')) ||
            !decimal.TryParse(txtGiaTri.Text.Trim().Replace("%", "").Replace(" đ", ""), NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out var valueGiaTri) ||
            valueGiaTri < 0 || (percent && valueGiaTri > 100))
        {
            txtThongBao.Text = "Chọn Phần trăm (0–100%) hoặc Số tiền (không âm), rồi nhập giá trị hợp lệ.";
            return;
        }

        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Lưu thay đổi — Ưu đãi: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        // shortcut: sửa dữ liệu demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _item.TenUuDai = txtTenUuDai.Text.Trim();
        _item.MaApDung = txtMaApDung.Text.Trim();
        _item.LoaiGiam = txtLoaiGiam.Text.Trim();
        _item.GiaTri = txtGiaTri.Text.Trim();
        _item.TuNgay = txtTuNgay.Text.Trim();
        _item.DenNgay = txtDenNgay.Text.Trim();
        _item.TrangThai = txtTrangThai.Text.Trim();

        _item.GiaTri = valueGiaTri.ToString("0.##", CultureInfo.GetCultureInfo("vi-VN")) + (percent ? "%" : " đ");
        DataContext = null;
        DataContext = _item;
        SetEditMode(false);
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiXoa") || _editing) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa — Ưu đãi: {_item.Ma}?";
        if (confirm.ShowDialog() != true) return;
        DialogResult = true;
    }

    private void btnUuDaiThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiThemMoi")) return;
        UuDaiForm form = new UuDaiForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
