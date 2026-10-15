using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Views.Dialogs;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views;

public partial class KhoView : UserControl
{
    private readonly BUS_KhoVatTu bus = new();
    public KhoView()
    {
        InitializeComponent();
        DanhMuc(cboLocTonKho, new Dictionary<string,string> { [""] = "Tất cả" }.Concat(new Dictionary<string,string> { ["THIEU"] = "Dưới tối thiểu", ["DU"] = "Đủ tồn" }), "");
        GanTimKiem(txtTimTonKho, txtLoiTimTonKho);
        txtTimTonKho.KeyDown += (_, e) => { if (e.Key == Key.Enter) TaiTonKho(); };
        cboLocTonKho.SelectionChanged += (_, _) => { if (IsLoaded) TaiTonKho(); };
        DanhMuc(cboLocPhieuNhapXuat, new Dictionary<string,string> { [""] = "Tất cả" }.Concat(BUS_KhoVatTu.TrangThaiPhieu), "");
        GanTimKiem(txtTimPhieuNhapXuat, txtLoiTimPhieuNhapXuat);
        txtTimPhieuNhapXuat.KeyDown += (_, e) => { if (e.Key == Key.Enter) TaiPhieuNhapXuat(); };
        cboLocPhieuNhapXuat.SelectionChanged += (_, _) => { if (IsLoaded) TaiPhieuNhapXuat(); };
        DanhMuc(cboLocKho, new Dictionary<string,string> { [""] = "Tất cả" }.Concat(BUS_KhoVatTu.TrangThai), "");
        GanTimKiem(txtTimKho, txtLoiTimKho);
        txtTimKho.KeyDown += (_, e) => { if (e.Key == Key.Enter) TaiKho(); };
        cboLocKho.SelectionChanged += (_, _) => { if (IsLoaded) TaiKho(); };
        Loaded += (_, _) => TaiLai();
        IsVisibleChanged += (_, _) => { if (IsLoaded && IsVisible) TaiLai(); };
    }
    private void TaiLai()
    {
        TaiTonKho();
        TaiPhieuNhapXuat();
        TaiKho();
    }

    private void TaiTonKho()
    {
        if (!DemoSession.CanOpen("KhoView")) return;
        try
        {
            var ds = BUS_KhoVatTu.Loc(() => bus.Ton(), txtTimTonKho.Text, TrangThai(cboLocTonKho), x => $"{x.MaVatTu} {x.VatTu} {x.Nhom} {x.Kho} {x.CanhBao}", x => x.Ton < x.ToiThieu ? "THIEU" : "DU");
            dgvTonKho.ItemsSource = ds;
            txtThongBaoTonKho.Text = $"{ds.Count} dòng · Nhấp đúp để xem chi tiết.";
        }
        catch (Exception ex) { dgvTonKho.ItemsSource = null; txtThongBaoTonKho.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void btnTimTonKho_Click(object sender, RoutedEventArgs e) => TaiTonKho();
    private void btnMoiTonKho_Click(object sender, RoutedEventArgs e)
    {
        MoiTimKiem(txtTimTonKho, txtLoiTimTonKho, cboLocTonKho); TaiLai();
    }
    private void TonKho_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen("KhoView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvTonKho, source) is not DataGridRow row || row.Item is not TonKhoHienThi x) return;
        new TonKhoChiTietWindow(x) { Owner = Window.GetWindow(this) }.ShowDialog();
        TaiLai();
    }
    private void btnTonKhoNhapKho_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnTonKhoNhapKho")) return;
        var form = new TonKhoNhapKhoWindow { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true)
        {
            TaiLai();

        }
    }
    private void btnTonKhoXuatKho_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnTonKhoXuatKho")) return;
        var form = new TonKhoXuatKhoWindow { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true)
        {
            TaiLai();

        }
    }
    private void btnTonKhoDieuChinhTon_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnTonKhoDieuChinhTon")) return;
        var form = new TonKhoForm { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true)
        {
            TaiLai();

        }
    }

    private void TaiPhieuNhapXuat()
    {
        if (!DemoSession.CanOpen("KhoView")) return;
        try
        {
            var ds = BUS_KhoVatTu.Loc(() => bus.Phieu(), txtTimPhieuNhapXuat.Text, TrangThai(cboLocPhieuNhapXuat), x => $"{x.MaPhieu} {x.Loai} {x.Kho} {x.NhaCungCap} {x.NhanVien} {x.MucDich} {x.GhiChu}", x => x.TrangThai);
            dgvPhieuNhapXuat.ItemsSource = ds;
            txtThongBaoPhieuNhapXuat.Text = $"{ds.Count} dòng · Nhấp đúp để xem chi tiết.";
        }
        catch (Exception ex) { dgvPhieuNhapXuat.ItemsSource = null; txtThongBaoPhieuNhapXuat.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void btnTimPhieuNhapXuat_Click(object sender, RoutedEventArgs e) => TaiPhieuNhapXuat();
    private void btnMoiPhieuNhapXuat_Click(object sender, RoutedEventArgs e)
    {
        MoiTimKiem(txtTimPhieuNhapXuat, txtLoiTimPhieuNhapXuat, cboLocPhieuNhapXuat); TaiLai();
    }
    private void PhieuNhapXuat_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen("KhoView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvPhieuNhapXuat, source) is not DataGridRow row || row.Item is not PhieuKhoHienThi x) return;
        new PhieuNhapXuatChiTietWindow(x) { Owner = Window.GetWindow(this) }.ShowDialog();
        TaiLai();
    }
    private void btnPhieuNhapXuatTaoPhieu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatTaoPhieu")) return;
        var form = new PhieuNhapXuatForm { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true)
        {
            TaiLai();
            try
            {
                var x = bus.Phieu().SingleOrDefault(x => x.MaPhieu == form.MaPhieu);
                if (x != null) new PhieuNhapXuatChiTietWindow(x) { Owner = Window.GetWindow(this) }.ShowDialog();
            }
            catch (Exception ex) { txtThongBaoPhieuNhapXuat.Text = "Phiếu đã lưu nhưng chưa mở được chi tiết. " + ex.Message; }
            TaiLai();
        }
    }

    private void TaiKho()
    {
        if (!DemoSession.CanOpen("KhoView")) return;
        try
        {
            var ds = BUS_KhoVatTu.Loc(() => bus.DanhMuc<Kho>(), txtTimKho.Text, TrangThai(cboLocKho), x => $"{x.MaKho} {x.TenKho} {x.GhiChu}", x => x.TrangThai.ToString());
            dgvKho.ItemsSource = ds;
            txtThongBaoKho.Text = $"{ds.Count} dòng · Nhấp đúp để xem chi tiết.";
        }
        catch (Exception ex) { dgvKho.ItemsSource = null; txtThongBaoKho.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void btnTimKho_Click(object sender, RoutedEventArgs e) => TaiKho();
    private void btnMoiKho_Click(object sender, RoutedEventArgs e)
    {
        MoiTimKiem(txtTimKho, txtLoiTimKho, cboLocKho); TaiLai();
    }
    private void Kho_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen("KhoView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvKho, source) is not DataGridRow row || row.Item is not Kho x) return;
        new KhoChiTietWindow(x) { Owner = Window.GetWindow(this) }.ShowDialog();
        TaiLai();
    }
    private void btnKhoThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnKhoThemMoi")) return;
        var form = new KhoForm { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true)
        {
            TaiLai();

        }
    }
}
