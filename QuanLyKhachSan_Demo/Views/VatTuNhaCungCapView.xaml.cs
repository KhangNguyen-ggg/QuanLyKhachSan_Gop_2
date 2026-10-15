using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Views.Dialogs;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views;

public partial class VatTuNhaCungCapView : UserControl
{
    private readonly BUS_KhoVatTu bus = new();
    public VatTuNhaCungCapView()
    {
        InitializeComponent();
        DanhMuc(cboLocVatTu, new Dictionary<string,string> { [""] = "Tất cả" }.Concat(BUS_KhoVatTu.TrangThai), "");
        GanTimKiem(txtTimVatTu, txtLoiTimVatTu);
        txtTimVatTu.KeyDown += (_, e) => { if (e.Key == Key.Enter) TaiVatTu(); };
        cboLocVatTu.SelectionChanged += (_, _) => { if (IsLoaded) TaiVatTu(); };
        DanhMuc(cboLocNhomVatTu, new Dictionary<string,string> { [""] = "Tất cả" }.Concat(BUS_KhoVatTu.TrangThai), "");
        GanTimKiem(txtTimNhomVatTu, txtLoiTimNhomVatTu);
        txtTimNhomVatTu.KeyDown += (_, e) => { if (e.Key == Key.Enter) TaiNhomVatTu(); };
        cboLocNhomVatTu.SelectionChanged += (_, _) => { if (IsLoaded) TaiNhomVatTu(); };
        DanhMuc(cboLocNhaCungCap, new Dictionary<string,string> { [""] = "Tất cả" }.Concat(BUS_KhoVatTu.TrangThai), "");
        GanTimKiem(txtTimNhaCungCap, txtLoiTimNhaCungCap);
        txtTimNhaCungCap.KeyDown += (_, e) => { if (e.Key == Key.Enter) TaiNhaCungCap(); };
        cboLocNhaCungCap.SelectionChanged += (_, _) => { if (IsLoaded) TaiNhaCungCap(); };
        Loaded += (_, _) => TaiLai();
        IsVisibleChanged += (_, _) => { if (IsLoaded && IsVisible) TaiLai(); };
    }
    private void TaiLai()
    {
        TaiVatTu();
        TaiNhomVatTu();
        TaiNhaCungCap();
    }

    private void TaiVatTu()
    {
        if (!DemoSession.CanOpen("VatTuNhaCungCapView")) return;
        try
        {
            var ds = BUS_KhoVatTu.Loc(() => bus.VatTu(), txtTimVatTu.Text, TrangThai(cboLocVatTu), x => $"{x.MaLoaiVatTu} {x.TenVatTu} {x.Nhom} {x.DonViTinh} {x.KhoMacDinh} {x.MoTa}", x => x.TrangThai.ToString());
            dgvVatTu.ItemsSource = ds;
            txtThongBaoVatTu.Text = $"{ds.Count} dòng · Nhấp đúp để xem chi tiết.";
        }
        catch (Exception ex) { dgvVatTu.ItemsSource = null; txtThongBaoVatTu.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void btnTimVatTu_Click(object sender, RoutedEventArgs e) => TaiVatTu();
    private void btnMoiVatTu_Click(object sender, RoutedEventArgs e)
    {
        MoiTimKiem(txtTimVatTu, txtLoiTimVatTu, cboLocVatTu); TaiLai();
    }
    private void VatTu_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen("VatTuNhaCungCapView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvVatTu, source) is not DataGridRow row || row.Item is not VatTuHienThi x) return;
        new VatTuChiTietWindow(x) { Owner = Window.GetWindow(this) }.ShowDialog();
        TaiLai();
    }
    private void btnVatTuThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuThemMoi")) return;
        var form = new VatTuForm { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true)
        {
            TaiLai();

        }
    }

    private void TaiNhomVatTu()
    {
        if (!DemoSession.CanOpen("VatTuNhaCungCapView")) return;
        try
        {
            var ds = BUS_KhoVatTu.Loc(() => bus.DanhMuc<NhomVatTu>(), txtTimNhomVatTu.Text, TrangThai(cboLocNhomVatTu), x => $"{x.MaNhomVatTu} {x.TenNhomVatTu} {x.MoTa}", x => x.TrangThai.ToString());
            dgvNhomVatTu.ItemsSource = ds;
            txtThongBaoNhomVatTu.Text = $"{ds.Count} dòng · Nhấp đúp để xem chi tiết.";
        }
        catch (Exception ex) { dgvNhomVatTu.ItemsSource = null; txtThongBaoNhomVatTu.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void btnTimNhomVatTu_Click(object sender, RoutedEventArgs e) => TaiNhomVatTu();
    private void btnMoiNhomVatTu_Click(object sender, RoutedEventArgs e)
    {
        MoiTimKiem(txtTimNhomVatTu, txtLoiTimNhomVatTu, cboLocNhomVatTu); TaiLai();
    }
    private void NhomVatTu_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen("VatTuNhaCungCapView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvNhomVatTu, source) is not DataGridRow row || row.Item is not NhomVatTu x) return;
        new NhomVatTuChiTietWindow(x) { Owner = Window.GetWindow(this) }.ShowDialog();
        TaiLai();
    }
    private void btnNhomVatTuThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuThemMoi")) return;
        var form = new NhomVatTuForm { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true)
        {
            TaiLai();

        }
    }

    private void TaiNhaCungCap()
    {
        if (!DemoSession.CanOpen("VatTuNhaCungCapView")) return;
        try
        {
            var ds = BUS_KhoVatTu.Loc(() => bus.DanhMuc<NhaCungCap>(), txtTimNhaCungCap.Text, TrangThai(cboLocNhaCungCap), x => $"{x.MaNhaCungCap} {x.TenNhaCungCap} {x.NguoiDaiDien} {x.SoDienThoai} {x.Email} {x.DiaChi} {x.LoaiCungCap}", x => x.TrangThai.ToString());
            dgvNhaCungCap.ItemsSource = ds;
            txtThongBaoNhaCungCap.Text = $"{ds.Count} dòng · Nhấp đúp để xem chi tiết.";
        }
        catch (Exception ex) { dgvNhaCungCap.ItemsSource = null; txtThongBaoNhaCungCap.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void btnTimNhaCungCap_Click(object sender, RoutedEventArgs e) => TaiNhaCungCap();
    private void btnMoiNhaCungCap_Click(object sender, RoutedEventArgs e)
    {
        MoiTimKiem(txtTimNhaCungCap, txtLoiTimNhaCungCap, cboLocNhaCungCap); TaiLai();
    }
    private void NhaCungCap_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen("VatTuNhaCungCapView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvNhaCungCap, source) is not DataGridRow row || row.Item is not NhaCungCap x) return;
        new NhaCungCapChiTietWindow(x) { Owner = Window.GetWindow(this) }.ShowDialog();
        TaiLai();
    }
    private void btnNhaCungCapThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapThemMoi")) return;
        var form = new NhaCungCapForm { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true)
        {
            TaiLai();

        }
    }
}
