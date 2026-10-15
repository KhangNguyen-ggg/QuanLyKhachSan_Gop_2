using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class DonPhongChiTietWindow : Window
{
    private readonly DonPhongMau _item;

    public DonPhongChiTietWindow(DonPhongMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new DonPhongMau();
        DataContext = _item;
        btnDonPhongThemCongViec.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnDonPhongThemCongViec");
        btnDonPhongPhanCong.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnDonPhongPhanCong");
        btnDonPhongBatDauDon.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnDonPhongBatDauDon");
        btnDonPhongHoanThanhDon.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnDonPhongHoanThanhDon");

    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnDonPhongThemCongViec_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnDonPhongThemCongViec")) return;
        DonPhongForm form = new DonPhongForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnDonPhongPhanCong_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnDonPhongPhanCong")) return;
        DonPhongPhanCongWindow form = new DonPhongPhanCongWindow();
        form.Owner = this;
        form.DataContext = _item;
        form.Title += $" — {_item.Ma}";
        if (form.FindName("txtTieuDe") is TextBlock heading) heading.Text += $" — {_item.Ma}";
        var optionPhong = form.cboPhong.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, _item.Phong));
        if (optionPhong == null) { optionPhong = new ComboBoxItem { Content = _item.Phong }; form.cboPhong.Items.Add(optionPhong); }
        form.cboPhong.SelectedItem = optionPhong;
        var optionNhanVien = form.cboNhanVien.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, _item.NhanVien));
        if (optionNhanVien == null) { optionNhanVien = new ComboBoxItem { Content = _item.NhanVien }; form.cboNhanVien.Items.Add(optionNhanVien); }
        form.cboNhanVien.SelectedItem = optionNhanVien;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnDonPhongBatDauDon_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnDonPhongBatDauDon")) return;
        var form = new XacNhanDemoWindow { Owner = this };
        form.txtNoiDung.Text = $"Bắt đầu dọn — Dọn phòng: {_item.Ma}?";
        if (form.ShowDialog() != true) return;
        // shortcut: xác nhận demo chưa ghi nghiệp vụ, nối dịch vụ xử lý trước khi thay đổi dữ liệu thật.
        txtThongBao.Text = "Đã xác nhận trong bản demo; thao tác chưa được ghi vào hệ thống.";
    }

    private void btnDonPhongHoanThanhDon_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnDonPhongHoanThanhDon")) return;
        var form = new XacNhanDemoWindow { Owner = this };
        form.txtNoiDung.Text = $"Hoàn thành dọn — Dọn phòng: {_item.Ma}?";
        if (form.ShowDialog() != true) return;
        // shortcut: xác nhận demo chưa ghi nghiệp vụ, nối dịch vụ xử lý trước khi thay đổi dữ liệu thật.
        txtThongBao.Text = "Đã xác nhận trong bản demo; thao tác chưa được ghi vào hệ thống.";
    }
}
