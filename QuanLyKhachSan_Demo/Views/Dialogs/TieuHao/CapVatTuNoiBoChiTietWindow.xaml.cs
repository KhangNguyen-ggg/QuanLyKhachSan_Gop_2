using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class CapVatTuNoiBoChiTietWindow : Window
{
    private readonly CapVatTuNoiBoMau _item;

    public CapVatTuNoiBoChiTietWindow(CapVatTuNoiBoMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new CapVatTuNoiBoMau();
        DataContext = _item;
        btnCapVatTuNoiBoTaoPhieuCap.IsEnabled = DemoSession.CanUseButton("TieuHaoView", "btnCapVatTuNoiBoTaoPhieuCap");
        btnCapVatTuNoiBoHuyPhieuCap.IsEnabled = DemoSession.CanUseButton("TieuHaoView", "btnCapVatTuNoiBoHuyPhieuCap");

    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnCapVatTuNoiBoTaoPhieuCap_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TieuHaoView", "btnCapVatTuNoiBoTaoPhieuCap")) return;
        CapVatTuNoiBoForm form = new CapVatTuNoiBoForm();
        form.Owner = this;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnCapVatTuNoiBoHuyPhieuCap_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TieuHaoView", "btnCapVatTuNoiBoHuyPhieuCap")) return;
        var form = new XacNhanDemoWindow { Owner = this };
        form.txtNoiDung.Text = $"Hủy phiếu cấp — Cấp vật tư nội bộ: {_item.Ma}?";
        if (form.ShowDialog() != true) return;
        // shortcut: xác nhận demo chưa ghi nghiệp vụ, nối dịch vụ xử lý trước khi thay đổi dữ liệu thật.
        txtThongBao.Text = "Đã xác nhận trong bản demo; thao tác chưa được ghi vào hệ thống.";
    }
}
