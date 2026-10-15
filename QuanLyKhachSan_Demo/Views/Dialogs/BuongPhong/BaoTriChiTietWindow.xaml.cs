using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class BaoTriChiTietWindow : Window
{
    private readonly BaoTriMau _item;

    public BaoTriChiTietWindow(BaoTriMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new BaoTriMau();
        DataContext = _item;
        btnBaoTriTaoYeuCau.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnBaoTriTaoYeuCau");
        btnBaoTriPhanCongXuLy.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnBaoTriPhanCongXuLy");
        btnBaoTriBatDauXuLy.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnBaoTriBatDauXuLy");
        btnBaoTriHoanThanhBaoTri.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnBaoTriHoanThanhBaoTri");

    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnBaoTriTaoYeuCau_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnBaoTriTaoYeuCau")) return;
        BaoTriForm form = new BaoTriForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnBaoTriPhanCongXuLy_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnBaoTriPhanCongXuLy")) return;
        BaoTriPhanCongXuLyWindow form = new BaoTriPhanCongXuLyWindow();
        form.Owner = this;
        form.DataContext = _item;
        form.Title += $" — {_item.Ma}";
        if (form.FindName("txtTieuDe") is TextBlock heading) heading.Text += $" — {_item.Ma}";
        var optionNguoiPhuTrach = form.cboNhanVien.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, _item.NguoiPhuTrach));
        if (optionNguoiPhuTrach == null) { optionNguoiPhuTrach = new ComboBoxItem { Content = _item.NguoiPhuTrach }; form.cboNhanVien.Items.Add(optionNguoiPhuTrach); }
        form.cboNhanVien.SelectedItem = optionNguoiPhuTrach;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnBaoTriBatDauXuLy_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnBaoTriBatDauXuLy")) return;
        var form = new XacNhanDemoWindow { Owner = this };
        form.txtNoiDung.Text = $"Bắt đầu xử lý — Bảo trì: {_item.Ma}?";
        if (form.ShowDialog() != true) return;
        // shortcut: xác nhận demo chưa ghi nghiệp vụ, nối dịch vụ xử lý trước khi thay đổi dữ liệu thật.
        txtThongBao.Text = "Đã xác nhận trong bản demo; thao tác chưa được ghi vào hệ thống.";
    }

    private void btnBaoTriHoanThanhBaoTri_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("BuongPhongView", "btnBaoTriHoanThanhBaoTri")) return;
        BaoTriHoanThanhBaoTriWindow form = new BaoTriHoanThanhBaoTriWindow();
        form.Owner = this;
        form.DataContext = _item;
        form.Title += $" — {_item.Ma}";
        if (form.FindName("txtTieuDe") is TextBlock heading) heading.Text += $" — {_item.Ma}";
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
