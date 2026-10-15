using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class CapVatTuNoiBoChiTietWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private readonly long ma;
    private CapNoiBoHienThi duLieu;
    private const string View = "TieuHaoView";
    public CapVatTuNoiBoChiTietWindow(CapNoiBoHienThi x)
    {
        InitializeComponent(); duLieu = x; ma = x.MaPhieuCap;
        Tai();
    }
    private void Tai()
    {
        if (!DemoSession.CanOpen(View)) return;
        try
        {
            duLieu = bus.Cap().Single(x => x.MaPhieuCap == ma);
            DataContext = null; DataContext = duLieu;
            dgvChiTiet.ItemsSource = bus.ChiTietCap(ma);
            bool moi = duLieu.TrangThai == "MOI";
            btnCapVatTuNoiBoThemVatTu.IsEnabled = moi && DemoSession.CanUseButton(View, "btnCapVatTuNoiBoTaoPhieuCap");
            btnCapVatTuNoiBoXacNhan.IsEnabled = moi && DemoSession.CanUseButton(View, "btnCapVatTuNoiBoXacNhan");
            btnCapVatTuNoiBoHuyPhieuCap.IsEnabled = moi && DemoSession.CanUseButton(View, "btnCapVatTuNoiBoHuyPhieuCap");

            txtThongBao.Text = moi ? "Phiếu Mới chưa thay đổi tồn kho." : "Phiếu đã xử lý: chỉ xem; không sửa hoặc hủy chứng từ đã ghi tồn.";
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnThemDong_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(View, "btnCapVatTuNoiBoTaoPhieuCap") || duLieu.TrangThai != "MOI") return;
        if (new ChiTietPhieuKhoForm(ma, null, true) { Owner = this }.ShowDialog() == true) Tai();
    }
    private void ChiTiet_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not DependencyObject source || ItemsControl.ContainerFromElement(dgvChiTiet, source) is not DataGridRow row || row.Item is not ChiTietCapHienThi x) return;
        new ChiTietPhieuKhoChiTietWindow(x) { Owner = this }.ShowDialog(); Tai();
    }
    private void XuLy(bool huy)
    {
        string quyen = huy ? "btnCapVatTuNoiBoHuyPhieuCap" : "btnCapVatTuNoiBoXacNhan";
        if (!DemoSession.CanUseButton(View, quyen) || duLieu.TrangThai != "MOI") return;
        var hoi = new XacNhanDemoWindow { Owner = this };
        hoi.txtNoiDung.Text = huy ? "Hủy phiếu Mới này?" : "Xác nhận phiếu và cập nhật tồn? Phiếu đã xác nhận sẽ không sửa hoặc hủy được.";
        if (hoi.ShowDialog() != true) return;
        try { bus.XuLyPhieu(ma, true, huy); Tai(); }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
    private void btnXacNhan_Click(object sender, RoutedEventArgs e) => XuLy(false);
    private void btnHuy_Click(object sender, RoutedEventArgs e) => XuLy(true);
}
