using System.Globalization;
using System.Windows;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TonKhoChiTietWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private TonKhoHienThi duLieu;
    public TonKhoChiTietWindow(TonKhoHienThi x)
    {
        InitializeComponent(); duLieu = x; DataContext = x;
        btnTonKhoNhapKho.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoNhapKho");
        btnTonKhoXuatKho.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoXuatKho");
        btnTonKhoDieuChinhTon.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoDieuChinhTon");
        btnTonKhoChuyenKho.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoChuyenKho");
        if (DemoSession.CanOpen("KhoView")) try { dgvNhatKy.ItemsSource = bus.NhatKy(x.MaKho, x.MaVatTu); } catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void Mo(Window form)
    {
        form.Owner = this;
        if (form.ShowDialog() == true)
        {
            try
            {
                duLieu = bus.Ton().SingleOrDefault(x => x.MaKho == duLieu.MaKho && x.MaVatTu == duLieu.MaVatTu) ?? duLieu;
                DataContext = null; DataContext = duLieu;
                dgvNhatKy.ItemsSource = bus.NhatKy(duLieu.MaKho, duLieu.MaVatTu);
            }
            catch (Exception ex) { txtThongBao.Text = ex.Message; }
        }
    }
    private void btnNhap_Click(object sender, RoutedEventArgs e)
    {
        if (DemoSession.CanUseButton("KhoView", "btnTonKhoNhapKho")) Mo(new TonKhoNhapKhoWindow(duLieu));
    }
    private void btnXuat_Click(object sender, RoutedEventArgs e)
    {
        if (DemoSession.CanUseButton("KhoView", "btnTonKhoXuatKho")) Mo(new TonKhoXuatKhoWindow(duLieu));
    }
    private void btnKiemKe_Click(object sender, RoutedEventArgs e)
    {
        if (DemoSession.CanUseButton("KhoView", "btnTonKhoDieuChinhTon")) Mo(new TonKhoForm(duLieu));
    }
    private void btnChuyen_Click(object sender, RoutedEventArgs e)
    {
        if (DemoSession.CanUseButton("KhoView", "btnTonKhoChuyenKho")) Mo(new ChuyenKhoForm(duLieu));
    }
}
