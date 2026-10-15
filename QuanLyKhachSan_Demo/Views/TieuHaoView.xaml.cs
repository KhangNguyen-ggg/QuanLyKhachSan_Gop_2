using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Views.Dialogs;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views;

public partial class TieuHaoView : UserControl
{
    private readonly BUS_KhoVatTu bus = new();
    public TieuHaoView()
    {
        InitializeComponent();
        DanhMuc(cboLocDinhMucTieuHao, new Dictionary<string,string> { [""] = "Tất cả" }.Concat(new Dictionary<string,string>()), "");
        GanTimKiem(txtTimDinhMucTieuHao, txtLoiTimDinhMucTieuHao);
        txtTimDinhMucTieuHao.KeyDown += (_, e) => { if (e.Key == Key.Enter) TaiDinhMucTieuHao(); };
        cboLocDinhMucTieuHao.SelectionChanged += (_, _) => { if (IsLoaded) TaiDinhMucTieuHao(); };
        DanhMuc(cboLocCapVatTuNoiBo, new Dictionary<string,string> { [""] = "Tất cả" }.Concat(BUS_KhoVatTu.TrangThaiPhieu), "");
        GanTimKiem(txtTimCapVatTuNoiBo, txtLoiTimCapVatTuNoiBo);
        txtTimCapVatTuNoiBo.KeyDown += (_, e) => { if (e.Key == Key.Enter) TaiCapVatTuNoiBo(); };
        cboLocCapVatTuNoiBo.SelectionChanged += (_, _) => { if (IsLoaded) TaiCapVatTuNoiBo(); };
        Loaded += (_, _) => { TaiBoLocDichVu(); TaiLai(); };
        IsVisibleChanged += (_, _) => { if (IsLoaded && IsVisible) { TaiBoLocDichVu(); TaiLai(); } };
    }
    private void TaiBoLocDichVu()
    {
        if (!DemoSession.CanOpen("TieuHaoView")) return;
        try
        {
            string ma = TrangThai(cboLocDinhMucTieuHao);
            var ds = new Dictionary<string, string> { [""] = "Tất cả dịch vụ" };
            foreach (var d in bus.DanhMuc<DichVu>()) ds[d.MaDichVu.ToString()] = d.TenDichVu;
            DanhMuc(cboLocDinhMucTieuHao, ds, ds.ContainsKey(ma) ? ma : "");
        }
        catch (Exception ex) { txtThongBaoDinhMucTieuHao.Text = ex.Message; }
    }
    private void TaiLai()
    {
        TaiDinhMucTieuHao();
        TaiCapVatTuNoiBo();
    }

    private void TaiDinhMucTieuHao()
    {
        if (!DemoSession.CanOpen("TieuHaoView")) return;
        try
        {
            var ds = BUS_KhoVatTu.Loc(() => bus.DinhMuc(), txtTimDinhMucTieuHao.Text, TrangThai(cboLocDinhMucTieuHao), x => $"{x.DichVu} {x.VatTu} {x.DonVi} {x.GhiChu}", x => x.MaDichVu.ToString());
            dgvDinhMucTieuHao.ItemsSource = ds;
            txtThongBaoDinhMucTieuHao.Text = $"{ds.Count} dòng · Nhấp đúp để xem chi tiết.";
        }
        catch (Exception ex) { dgvDinhMucTieuHao.ItemsSource = null; txtThongBaoDinhMucTieuHao.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void btnTimDinhMucTieuHao_Click(object sender, RoutedEventArgs e) => TaiDinhMucTieuHao();
    private void btnMoiDinhMucTieuHao_Click(object sender, RoutedEventArgs e)
    {
        MoiTimKiem(txtTimDinhMucTieuHao, txtLoiTimDinhMucTieuHao, cboLocDinhMucTieuHao); TaiLai();
    }
    private void DinhMucTieuHao_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen("TieuHaoView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvDinhMucTieuHao, source) is not DataGridRow row || row.Item is not DinhMucHienThi x) return;
        new DinhMucTieuHaoChiTietWindow(x) { Owner = Window.GetWindow(this) }.ShowDialog();
        TaiLai();
    }
    private void btnDinhMucTieuHaoThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoThemMoi")) return;
        var form = new DinhMucTieuHaoForm { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true)
        {
            TaiLai();

        }
    }

    private void TaiCapVatTuNoiBo()
    {
        if (!DemoSession.CanOpen("TieuHaoView")) return;
        try
        {
            var ds = BUS_KhoVatTu.Loc(() => bus.Cap(), txtTimCapVatTuNoiBo.Text, TrangThai(cboLocCapVatTuNoiBo), x => $"{x.MaPhieuCap} {x.Kho} {x.NhanVien} {x.ThamChieu} {x.GhiChu} {BUS_KhoVatTu.MucDichCap.GetValueOrDefault(x.MucDich)}", x => x.TrangThai);
            dgvCapVatTuNoiBo.ItemsSource = ds;
            txtThongBaoCapVatTuNoiBo.Text = $"{ds.Count} dòng · Nhấp đúp để xem chi tiết.";
        }
        catch (Exception ex) { dgvCapVatTuNoiBo.ItemsSource = null; txtThongBaoCapVatTuNoiBo.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void btnTimCapVatTuNoiBo_Click(object sender, RoutedEventArgs e) => TaiCapVatTuNoiBo();
    private void btnMoiCapVatTuNoiBo_Click(object sender, RoutedEventArgs e)
    {
        MoiTimKiem(txtTimCapVatTuNoiBo, txtLoiTimCapVatTuNoiBo, cboLocCapVatTuNoiBo); TaiLai();
    }
    private void CapVatTuNoiBo_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen("TieuHaoView") || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvCapVatTuNoiBo, source) is not DataGridRow row || row.Item is not CapNoiBoHienThi x) return;
        new CapVatTuNoiBoChiTietWindow(x) { Owner = Window.GetWindow(this) }.ShowDialog();
        TaiLai();
    }
    private void btnCapVatTuNoiBoTaoPhieuCap_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TieuHaoView", "btnCapVatTuNoiBoTaoPhieuCap")) return;
        var form = new CapVatTuNoiBoForm { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true)
        {
            TaiLai();
            try
            {
                var x = bus.Cap().SingleOrDefault(x => x.MaPhieuCap == form.MaPhieuCap);
                if (x != null) new CapVatTuNoiBoChiTietWindow(x) { Owner = Window.GetWindow(this) }.ShowDialog();
            }
            catch (Exception ex) { txtThongBaoCapVatTuNoiBo.Text = "Phiếu đã lưu nhưng chưa mở được chi tiết. " + ex.Message; }
            TaiLai();
        }
    }
}
